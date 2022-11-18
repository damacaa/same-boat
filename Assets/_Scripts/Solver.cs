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
                        if (island.islandRef == current.CurrentIsland)
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
                    next.Steps = current.Steps + 1;
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

        public static int SolveWidth(GameLogic game)
        {
            List<State> openList = new List<State>();
            List<State> closedList = new List<State>();

            State current = game.GetCurrentState();
            State newState;

            openList.Add(current);

            int iter = 0;
            int maxIter = 5000;
            while (openList.Count > 0 && iter < maxIter)
            {
                iter++;

                //openList = openList.OrderByDescending(t => t.F).ToList();// Less iterations needed, potentially subotimal result

                SetCurrent(openList[0]);
                openList.Remove(current);
                closedList.Add(current);

                //Debug.Log(iter + ": " + current);
                if (game.GetCurrentState().ToString() != current.ToString())
                {
                    Debug.LogError("Fuck");
                }

                //If boat has empty seats it will load new transportables
                if (current.BoatOccupiedSeats < current.BoatCapacity && current.BoatCurrentWeight < current.BoatMaxWeight)
                {
                    for (int i = 0; i < current.CurrentIsland.Transportables.Count; i++)
                    {
                        Transportable transportable = current.CurrentIsland.Transportables[i];
                        if (transportable == null || (current.BoatCurrentWeight + transportable.Weight) > current.BoatMaxWeight || !game.LoadOnBoat(transportable, true))
                        {
                            continue;
                        }

                        newState = game.GetCurrentState();
                        if (CheckValidStep(newState))
                        {
                            newState.PreviousState = current;
                            newState.Steps = current.Steps + 1;
                            openList.Add(newState);
                        }

                        game.Undo(true);
                    }
                }

                //If boat has transportables it will try to unload in current island
                if (current.BoatOccupiedSeats > 0)
                {
                    for (int i = 0; i < current.BoatTransportables.Length; i++)
                    {
                        Transportable transportable = current.BoatTransportables[i];
                        if (transportable == null || !game.UnloadFromBoat(transportable, true))
                        {
                            continue;
                        }

                        newState = game.GetCurrentState();
                        if (CheckValidStep(newState))
                        {
                            newState.PreviousState = current;
                            newState.Steps = current.Steps + 1;
                            openList.Add(newState);

                            if (game.Win)
                            {
                                current = newState;
                                break;
                            }
                        }

                        game.Undo(true);
                    }
                }

                if (game.Win)
                {
                    break;
                }

                //Move to another island
                for (int i = current.Islands.Count - 1; i >= 0; i--)
                {
                    State.IslandState island = current.Islands[i];
                    if (island.islandRef == current.CurrentIsland || (!current.BoatCanMoveEmpty && current.BoatOccupiedSeats == 0))
                        continue;

                    game.MoveBoatToIsland(island.islandRef, true);

                    newState = game.GetCurrentState();
                    if (CheckValidStep(newState))
                    {
                        newState.PreviousState = current;
                        newState.Steps = current.Steps + 1;
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
            while (current.PreviousState != null)
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
                return !(game.Fail || openList.Contains(next) || closedList.Contains(next));
            }

            void SetCurrent(State s)
            {
                int iter = 0;

                List<State> states = new List<State>();
                State aux = s;
                while (aux != null && iter < 100)
                {
                    states.Add(aux);
                    aux = aux.PreviousState;
                    iter++;
                }
                states.Reverse();

                aux = current;
                while (aux.PreviousState != null && iter < 100)
                {
                    game.Undo(true);
                    aux = aux.PreviousState;
                }

                //game.Reset();

                for (int i = 1; i < states.Count; i++)
                {
                    if (!(states[i].Bommand != null && game.AddCommand(states[i].Bommand, true)))
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

        public State PreviousState = null;

        public int Steps = 0;

        public Island CurrentIsland;
        public List<IslandState> Islands = new List<IslandState>();

        public Transportable[] BoatTransportables;
        public int BoatCapacity = 0;
        public int BoatOccupiedSeats = 0;

        public Command Bommand;

        public int F { get { return (2 * Islands[Islands.Count - 1].transportables.Length) - Steps; } }

        public int BoatMaxWeight { get; internal set; }
        public int BoatCurrentWeight { get; internal set; }
        public bool BoatCanMoveEmpty { get; internal set; }

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

            return result;
        }
    }
}
