using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Unity.VisualScripting;

namespace Solver
{
    public class Solver
    {
        public static bool SolveWidthAndReset(GameLogic game, bool useHeuristic = false)
        {
            State solution = Solve(game, useHeuristic);

            if (!game.Win)
            {
                return false;
            }

            ResetGame(game, solution);

            return true;
        }

        internal static State Solve(GameLogic game, bool useHeuristic = false, CancellationTokenSource cancellationToken = null)
        {
            State current = Initialize(game, useHeuristic, out PriorityQueue<State> nodeQueue, out HashSet<State> openList, out HashSet<State> closedList);

            while (openList.Count > 0 && (cancellationToken != null && !cancellationToken.IsCancellationRequested))
            {
                // Sets game state
                State previous = current;
                current = nodeQueue.Dequeue();

                SetState(current, previous, game);

                //Thread.Sleep(1000);

                openList.Remove(current);
                closedList.Add(current);

                // Checks win and brakes loop if true
                if (game.Win)
                    return current;

                // Adds to the open list all of the new possible states derived from the current state
                ExpandNeighbours(current, game, openList, closedList, nodeQueue, true);

            }

            return null;
        }

        protected static State Initialize(GameLogic game, bool useHeuristic, out PriorityQueue<State> nodeQueue, out HashSet<State> openList, out HashSet<State> closedList)
        {
            openList = new HashSet<State>(new StateComparer()); // List of states to explore 
            closedList = new HashSet<State>(new StateComparer()); // List of states that have already been explored

            if (useHeuristic)
                nodeQueue = new PriorityQueue<State>((n1, n2) => n2.F.CompareTo(n1.F));
            else
                nodeQueue = new PriorityQueue<State>((n1, n2) => 1);

            State current = game.GetCurrentState();

            openList.Add(current); // Exploration starts with current state
            nodeQueue.Enqueue(current);

            return current;
        }



        protected static void ResetGame(GameLogic game, State current)
        {
            // Reset game to initial state
            State aux = current;
            while (aux.PreviousState != null)
            {
                aux = aux.PreviousState;
                game.Undo(true);
            }
        }

        protected static void SetState(State newState, State current, GameLogic game, bool skipAnimation = true)
        {
            if (newState == current) { return; }

            // Retrieve all the states previous to the new one
            List<State> states = new List<State>();
            State aux = newState;
            while (aux != null)
            {
                states.Add(aux);
                aux = aux.PreviousState;
            }

            // Reverse order
            states.Reverse();

            // Undo game until it reaches a state contained in the previous list
            int numberOfUndos = 0;
            aux = current;
            while (aux != null && !states.Contains(aux))
            {
                game.Undo(skipAnimation);
                aux = aux.PreviousState;
                numberOfUndos++;
            }

            // The fisrt state found in the previous loop becomes the initial index of the next loop
            int startingPoint = aux == null ? 0 : states.IndexOf(aux);

            // Execute every command from every state to obtain the same state in the game
            for (int i = startingPoint + 1; i < states.Count; i++)
            {
                game.AddCommand(states[i].Command, skipAnimation);
            }
        }

        protected static void ExpandNeighbours(State current, GameLogic game, HashSet<State> openList, HashSet<State> closedList, PriorityQueue<State> queue, bool omitAnimation)
        {
            //If boat has transportables it will try to unload in current island
            if (current.BoatOccupiedSeats > 0)
            {
                for (int i = 0; i < current.BoatTransportables.Length; i++)
                {
                    Transportable transportable = current.BoatTransportables[i];
                    // Game logic is completely decoupled. The solver doesn't need to know how does the game work in order to work.
                    // It just tries to make every possible move and asks if it was valid.
                    if (transportable == null || !game.UnloadFromBoat(transportable, omitAnimation))
                    {
                        continue;
                    }

                    TryAddNewState(current, game, openList, closedList, queue);

                    /*if (game.Win)
                    {
                        openList.Add(current);
                        queue.Enqueue(current);
                    }*/

                    game.Undo(omitAnimation);
                }
            }

            //If boat has empty seats it will load new transportables
            if (current.BoatOccupiedSeats < current.BoatCapacity)
            {
                for (int i = 0; i < current.CurrentIsland.Transportables.Count; i++)
                {
                    Transportable transportable = current.CurrentIsland.Transportables[i];
                    if (transportable == null || !game.LoadOnBoat(transportable, omitAnimation))
                    {
                        continue;
                    }

                    TryAddNewState(current, game, openList, closedList, queue);

                    game.Undo(omitAnimation);
                }
            }

            //Boat will traver to another island if possible
            for (int i = current.Islands.Count - 1; i >= 0; i--)
            {
                State.IslandState island = current.Islands[i];
                if (island.islandRef == current.CurrentIsland || !game.MoveBoatToIsland(island.islandRef, omitAnimation))
                    continue;

                TryAddNewState(current, game, openList, closedList, queue);

                game.Undo(omitAnimation);
            }
        }

        protected static void TryAddNewState(State current, GameLogic game, HashSet<State> openList, HashSet<State> closedList, PriorityQueue<State> queue)
        {
            State newState = game.GetCurrentState();

            bool isInOpenList = openList.Contains(newState);
            bool isInClosedList = closedList.Contains(newState);
            // If the state is considered a fail or has already been explored, it gets omitted
            if (game.Win || !(game.Fail || isInOpenList || isInClosedList))
            {
                newState.PreviousState = current;
                openList.Add(newState);
                queue.Enqueue(newState);
            }
        }

    }


    // Implement a custom comparer for MyClass
    class StateComparer : IEqualityComparer<State>
    {
        public bool Equals(State x, State y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(State obj)
        {
            unchecked
            {
                return 23 * obj.ToString().GetHashCode();
            }
        }
    }

    public class State : IEquatable<State>
    {
        public struct IslandState
        {
            public Island islandRef;
            public Transportable[] transportables;
            public IslandState(Island island)
            {
                islandRef = island;
                transportables = island.Transportables.FindAll(t => t != null).OrderBy(t => t.ToString()).ToArray();
            }
        }

        public int Crossings;
        public int Steps;

        public Island CurrentIsland;
        public List<IslandState> Islands;

        public Transportable[] BoatTransportables;
        public int BoatCapacity;
        public int BoatOccupiedSeats;

        public Command Command;

        private State _previousState;
        private string _text;

        public int BoatMaxWeight { get; internal set; }
        public int BoatCurrentWeight { get; internal set; }
        public int BoatMaxTravelCost { get; internal set; }
        public int BoatTravelCost { get; internal set; }

        public int F { get { return (Islands[Islands.Count - 1].transportables.Length) - Steps; } }//- Crossings

        public State PreviousState
        {
            get
            {
                return _previousState;
            }
            set
            {
                Steps = value.Steps + 1;
                _previousState = value;
            }
        }


        public State()
        {
            Crossings = 0;
            Steps = 0;
            BoatCapacity = 0;
            BoatOccupiedSeats = 0;

            _previousState = null;
            _text = null;

            Islands = new List<IslandState>();
        }

        public void AddIsland(Island island)
        {
            Islands.Add(new IslandState(island));
        }

        public bool Equals(State other)
        {
            return ToString().Equals(other.ToString());
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(_text))
                return _text;

            StringBuilder sb = new StringBuilder();

            foreach (var island in Islands)
            {
                sb.Append("[ ");
                var sortedIslandTransportables = island.transportables.ToList();
                sortedIslandTransportables = sortedIslandTransportables.OrderBy(x => x.ScripatableObject.name).ToList();
                foreach (var transportable in sortedIslandTransportables)
                {
                    sb.Append(transportable + " ");
                }
                sb.Append("] ");
            }

            sb.Append("< ");

            var sortedBoatTransportables = BoatTransportables.ToList();
            sortedBoatTransportables = sortedBoatTransportables.OrderBy(x => x.ScripatableObject.name).ToList();

            for (int i = 0; i < BoatCapacity; i++)
            {
                if (i < BoatTransportables.Length)
                {
                    var t = sortedBoatTransportables[i];
                    if (t != null)
                        sb.Append($"[{t}] ");
                    else
                        sb.Append($"[ ] ");
                }
                else
                {
                    sb.Append("[ ] ");
                }
            }

            sb.Append("> (" + CurrentIsland.Name + ")");

            sb.Append(BoatMaxWeight != 0 ? " " + BoatCurrentWeight : "");
            sb.Append(BoatMaxTravelCost != 0 ? " " + BoatTravelCost : "");

            //result += Steps;

            _text = sb.ToString();

            return _text;
        }
    }
}


