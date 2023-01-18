using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Solver
{
    public class Solver
    {
        public static State SolveWidth(GameLogic game, bool useHeuristic = false)
        {
            List<State> openList = new List<State>(); // List of states to explore 
            List<State> closedList = new List<State>(); // List of states that have already been explored

            State current = game.GetCurrentState();

            openList.Add(current); // Exploration starts with current state

            while (openList.Count > 0)
            {
                if (useHeuristic)
                    openList = openList.OrderByDescending(t => t.F).ToList();

                // Sets game state
                State previous = current;
                current = openList[0];

                SetState(current, previous, game);

                openList.Remove(current);
                closedList.Add(current);

                // Checks win and brakes loop if true
                if (game.Win) break;

                // Adds to the open list all of the new possible states derived from the current state
                ExpandNeighbours(current, game, openList, closedList);
            }

            if (!game.Win)
            {
                return null;
            }

            // Reset game to initial state
            State aux = current;
            while (aux.PreviousState != null)
            {
                aux = aux.PreviousState;
                game.Undo(true);
            }

            return current;
        }

        private static void SetState(State newState, State current, GameLogic game)
        {
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
                game.Undo(true);
                aux = aux.PreviousState;
                numberOfUndos++;
            }

            // The fisrt state found in the previous loop becomes the initial index of the next loop
            int startingPoint = aux == null ? 0 : states.IndexOf(aux);

            // Execute every command from every state to obtain the same state in the game
            for (int i = startingPoint + 1; i < states.Count; i++)
            {
                game.AddCommand(states[i].Command, true);
            }
        }

        private static void ExpandNeighbours(State current, GameLogic game, List<State> openList, List<State> closedList)
        {
            //If boat has transportables it will try to unload in current island
            if (current.BoatOccupiedSeats > 0)
            {
                for (int i = 0; i < current.BoatTransportables.Length; i++)
                {
                    Transportable transportable = current.BoatTransportables[i];
                    // Game logic is completely decoupled. The solver doesn't need to know how does the game work in order to work.
                    // It just tries to make every possible move and asks if it was valid.
                    if (transportable == null || !game.UnloadFromBoat(transportable, true))
                    {
                        continue;
                    }

                    TryAddNewState(current, game, openList, closedList);

                    if (game.Win)
                    {
                        openList.Insert(0, openList[openList.Count - 1]);
                    }

                    game.Undo(true);
                }
            }

            //If boat has empty seats it will load new transportables
            if (current.BoatOccupiedSeats < current.BoatCapacity)
            {
                for (int i = 0; i < current.CurrentIsland.Transportables.Count; i++)
                {
                    Transportable transportable = current.CurrentIsland.Transportables[i];
                    if (transportable == null || !game.LoadOnBoat(transportable, true))
                    {
                        continue;
                    }

                    TryAddNewState(current, game, openList, closedList);

                    game.Undo(true);
                }
            }

            //Boat will traver to another island if possible
            for (int i = current.Islands.Count - 1; i >= 0; i--)
            {
                State.IslandState island = current.Islands[i];
                if (island.islandRef == current.CurrentIsland || !game.MoveBoatToIsland(island.islandRef, true))
                    continue;

                TryAddNewState(current, game, openList, closedList);

                game.Undo(true);
            }
        }

        private static void TryAddNewState(State current, GameLogic game, List<State> openList, List<State> closedList)
        {
            State newState = game.GetCurrentState();

            // If the state is considered a fail or has already been explored, it gets omitted
            if (!(game.Fail || openList.Contains(newState) || closedList.Contains(newState)))
            {
                newState.PreviousState = current;
                openList.Add(newState);
            }
        }

        public static IEnumerator SolveWidthCoroutine(GameLogic game, bool useHeuristic = false)
        {
            List<State> openList = new List<State>();
            List<State> closedList = new List<State>();

            State current = game.GetCurrentState();

            openList.Add(current);

            int iter = 0;
            int maxIter = 5000;
            float elapsedTime = 0;
            while (openList.Count > 0 && iter < maxIter)
            {
                float startTime = Time.realtimeSinceStartup;

                iter++;

                if (useHeuristic)
                    openList = openList.OrderByDescending(t => t.F).ToList();

                SetState(openList[0], current, game);
                current = openList[0];
                openList.Remove(current);
                closedList.Add(current);

                if (game.Win) break;

                if (game.GetCurrentState().ToString() != current.ToString())
                {
                    Debug.LogError("FUCKKKKK");
                }

                ExpandNeighbours(current, game, openList, closedList);

                elapsedTime += Time.realtimeSinceStartup - startTime;
                if (elapsedTime > 1f / Application.targetFrameRate)
                {
                    elapsedTime = 0;
                    yield return null;
                }
            }

            if (!game.Win)
            {
                yield break;
            }

            while (current.PreviousState != null)
            {
                current = current.PreviousState;
                game.Undo(true);
            }

            Debug.Log("Done");

            yield return null;
        }

        [Obsolete("Method is obsolete.", false)]
        public static int SolveDepth(GameLogic game)
        {
            State initial = game.GetCurrentState();

            State current = null;
            State next = initial;

            Dictionary<string, State> dict = new Dictionary<string, State>();

            int level = 0;

            int iter = 0;
            int maxIter = 10000;
            while (!game.Win && iter < maxIter)
            {
                iter++;

                current = next;

                Debug.Log(iter + ": " + current);

                if (game.Win)
                {
                    break;
                }

                dict.TryAdd(current.ToString(), current);

                bool done = false;

                if (current.BoatOccupiedSeats < current.BoatCapacity)
                {
                    //If boat has empty seats it will load new transportables
                    for (int i = 0; i < current.CurrentIsland.Transportables.Count; i++)
                    {
                        Transportable transportable = current.CurrentIsland.Transportables[i];
                        if (transportable == null || !game.LoadOnBoat(transportable, true))
                        {
                            continue;
                        }

                        next = game.GetCurrentState();
                        if (CheckValidStep(next))
                        {
                            done = true;
                            break;
                        }
                        else
                        {
                            game.Undo(true);
                        }
                    }
                }
                else
                {
                    //If boat doesn't have empty seats it will try to unload in current island
                    for (int i = 0; i < current.BoatTransportables.Length; i++)
                    {
                        Transportable transportable = current.BoatTransportables[i];
                        if (transportable == null || !game.UnloadFromBoat(transportable, true))
                        {
                            continue;
                        }

                        next = game.GetCurrentState();
                        if (CheckValidStep(next))
                        {
                            done = true;
                            break;
                        }
                        else
                        {
                            game.Undo(true);
                        }
                    }
                }

                //If boat couldn't do anything in current island, it try to will travel to another island
                if (!done)
                {
                    for (int i = current.Islands.Count - 1; i >= 0; i--)
                    {
                        State.IslandState island = current.Islands[i];
                        if (island.islandRef == current.CurrentIsland || !game.MoveBoatToIsland(island.islandRef, true))
                            continue;


                        next = game.GetCurrentState();
                        if (CheckValidStep(next))
                        {
                            done = true;
                            break;
                        }
                        else
                        {
                            game.Undo(true);
                        }

                    }
                }

                if (done)
                {
                    next.Crossings = current.Crossings + 1;
                    next.PreviousState = current;
                    level++;
                }
                else
                {
                    //If can't do anythin, it will undo
                    game.Undo(true);
                    next = current.PreviousState;
                    level--;
                    //If it tries to undo beyond initial state, the search will fail
                    if (level < 0)
                    {
                        Debug.Log("Can't find a solution " + iter);
                        break;
                    }
                }
            }

            if (!game.Win)
            {
                if (iter == maxIter)
                    Debug.Log("Need more iter");

                return -1;
            }

            int requiredSteps = 0;
            while (current != null)
            {
                current = current.PreviousState;
                requiredSteps++;
                game.Undo(true);
            }

            return requiredSteps;

            ///////////////////////////////////////////////////////////

            //Functions
            bool CheckValidStep(State next)
            {
                return !(game.Fail || dict.ContainsKey(next.ToString()));
            }
        }
    }

    public class State : IEquatable<State>
    {
        public class IslandState
        {
            public Island islandRef;
            public Transportable[] transportables;
            public IslandState(Island island)
            {
                this.islandRef = island;
                transportables = island.Transportables.FindAll(t => t != null).OrderBy(t => t.ToString()).ToArray();
            }
        }

        public State PreviousState = null;

        public int Crossings = 0;

        public Island CurrentIsland;
        public List<IslandState> Islands = new List<IslandState>();

        public Transportable[] BoatTransportables;
        public int BoatCapacity = 0;
        public int BoatOccupiedSeats = 0;

        public Command Command;

        public int BoatMaxWeight { get; internal set; }
        public int BoatCurrentWeight { get; internal set; }
        public int BoatMaxTravelCost { get; internal set; }
        public int BoatTravelCost { get; internal set; }

        public int F { get { return (Islands[Islands.Count - 1].transportables.Length); } }//- Crossings

        public void AddIsland(Island island)
        {
            Islands.Add(new IslandState(island));
        }

        public bool Equals(State other)
        {
            return ToString() == other.ToString();
        }

        string _text = "";
        public override string ToString()
        {
            if (_text != "")
                return _text;

            string result = "";
            foreach (var island in Islands)
            {
                result += "[ ";
                foreach (var transportable in island.transportables)
                {
                    result += transportable + " ";
                }
                result += "] ";
            }

            result += "< ";

            for (int i = 0; i < BoatCapacity; i++)
            {
                if (i < BoatTransportables.Length && BoatTransportables[i] != null)
                {
                    result += "[" + BoatTransportables[i] + "] ";
                }
                else
                {
                    result += "[ ] ";
                }
            }
            result += "> (" + CurrentIsland.Name + ")";

            result += BoatMaxWeight != 0 ? " " + BoatCurrentWeight : "";
            result += BoatMaxTravelCost != 0 ? " " + BoatTravelCost : "";

            return result;
        }
    }
}
