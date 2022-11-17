using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Solver
{
    public class Solver
    {
        public static int Solve(GameLogic game)
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

                if (current.boatOccupiedSeats < current.boatCapacity)
                {
                    //If boat has empty seats it will load new transportables
                    for (int i = 0; i < current.currentIsland.Transportables.Count; i++)
                    {
                        Transportable transportable = current.currentIsland.Transportables[i];
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
                    for (int i = 0; i < current.boatTransportables.Length; i++)
                    {
                        Transportable transportable = current.boatTransportables[i];
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
                    for (int i = current.islands.Count - 1; i >= 0; i--)
                    {
                        State.IslandState island = current.islands[i];
                        if (island.islandRef == current.currentIsland)
                            continue;

                        game.MoveBoatToIsland(island.islandRef, true);
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
                    next.steps = current.steps + 1;
                    next.previousState = current;
                    level++;
                }
                else
                {
                    //If can't do anythin, it will undo
                    game.Undo(true);
                    next = current.previousState;
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
                current = current.previousState;
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



        public static int SolveWidth(GameLogic game)
        {


            List<State> openList = new List<State>();
            List<State> closedList = new List<State>();

            State initial = game.GetCurrentState();
            State current = initial;
            State newState = initial;

            openList.Add(initial);

            int iter = 0;
            int maxIter = 1000;
            while (!game.Win && openList.Count > 0 && iter < maxIter)
            {
                iter++;

                //current = openList[0];
                SetState(openList[0]);
                openList.Remove(current);
                closedList.Add(current);

                if (game.GetCurrentState().ToString() != current.ToString())
                {
                    Debug.LogError("Fuck");
                }

                Debug.Log(iter + ": " + current);

                if (game.Win)
                {
                    break;
                }

                if (current.boatOccupiedSeats < current.boatCapacity)
                {
                    //If boat has empty seats it will load new transportables
                    for (int i = 0; i < current.currentIsland.Transportables.Count; i++)
                    {
                        Transportable transportable = current.currentIsland.Transportables[i];
                        if (transportable == null || !game.LoadOnBoat(transportable, true))
                        {
                            continue;
                        }

                        newState = game.GetCurrentState();
                        if (CheckValidStep(newState))
                        {
                            newState.previousState = current;
                            newState.steps = current.steps + 1;
                            openList.Add(newState);
                        }

                        game.Undo(true);
                    }
                }

                if (current.boatOccupiedSeats > 0)
                {
                    //If boat has transportables it will try to unload in current island
                    for (int i = 0; i < current.boatTransportables.Length; i++)
                    {
                        Transportable transportable = current.boatTransportables[i];
                        if (transportable == null || !game.UnloadFromBoat(transportable, true))
                        {
                            continue;
                        }

                        newState = game.GetCurrentState();
                        if (CheckValidStep(newState))
                        {
                            newState.previousState = current;
                            newState.steps = current.steps + 1;
                            openList.Add(newState);
                        }

                        game.Undo(true);
                    }
                }

                for (int i = current.islands.Count - 1; i >= 0; i--)
                {
                    State.IslandState island = current.islands[i];
                    if (island.islandRef == current.currentIsland)
                        continue;

                    game.MoveBoatToIsland(island.islandRef, true);

                    newState = game.GetCurrentState();
                    if (CheckValidStep(newState))
                    {
                        newState.previousState = current;
                        newState.steps = current.steps + 1;
                        openList.Add(newState);
                    }

                    game.Undo(true);
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
                current = current.previousState;
                requiredSteps++;
                game.Undo(true);
            }

            return requiredSteps;

            ///////////////////////////////////////////////////////////

            //Functions
            bool CheckValidStep(State next)
            {
                return !(game.Fail || openList.Contains(next) || closedList.Contains(next));
            }

            void SetState(State s)
            {
                int iter = 0;

                List<State> states = new List<State>();
                State aux = s;
                while (aux != null && iter < 100)
                {
                    states.Add(aux);
                    aux = aux.previousState;
                    iter++;
                }
                states.Reverse();

                game.Reset();

                for (int i = 1; i < states.Count; i++)
                {
                    if (!(states[i].command != null && game.AddCommand(states[i].command, true)))
                    {
                        Debug.Log("Fuck " + iter);
                    }
                }

                current = s;
            }
        }


        public static bool CheckFail(List<Transportable> transportables)
        {
            bool success = true;

            foreach (var a in transportables)
            {
                foreach (var b in transportables)
                {
                    if (a == null || b == null)
                    {
                        continue;
                    }

                    success = success && a.CheckCompatibility(b);
                    if (a == b)
                        break;
                }
            }

            return !success;
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

        public State previousState = null;
        public int steps = 0;

        public Island currentIsland;
        public List<IslandState> islands = new List<IslandState>();

        public Transportable[] boatTransportables;
        public int boatCapacity = 0;
        public int boatOccupiedSeats = 0;

        public Command command;

        public int F { get { return (2 * islands[islands.Count - 1].transportables.Length) - steps; } }

        public void AddIsland(Island island)
        {
            islands.Add(new IslandState(island));
        }

        public bool Equals(State other)
        {
            return ToString() == other.ToString();
        }

        string text = "";
        public override string ToString()
        {
            if (text != "")
                return text;

            string result = "";
            foreach (var island in islands)
            {
                result += "[ ";
                foreach (var transportable in island.transportables)
                {
                    result += transportable + " ";
                }
                result += "] ";
            }

            result += "< ";

            for (int i = 0; i < boatCapacity; i++)
            {
                if (i < boatTransportables.Length && boatTransportables[i] != null)
                {
                    result += "[" + boatTransportables[i] + "] ";
                }
                else
                {
                    result += "[ ] ";
                }
            }
            result += "> (" + currentIsland.Name + ")";

            return result;
        }
    }
}
