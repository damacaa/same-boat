using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                //Debug.Log(iter + ": " + current);

                next.previousState = current;
                current = next;
                if (game.Win)
                {
                    break;
                }

                dict.TryAdd(current.ToString(), current);

                bool done = false;

                if (current.boatOccupiedSeats < current.boatCapacity)
                {
                    //If boat has empty seats it will load new transportables
                    for (int i1 = 0; i1 < current.currentIsland.Transportables.Count; i1++)
                    {
                        Transportable transportable = current.currentIsland.Transportables[i1];


                        if (!game.LoadOnBoat(transportable, true))
                        {
                            continue;
                        }

                        next = game.GetCurrentState();
                        if (CheckValidStep(next))
                        {
                            done = true;
                            break;
                        }
                    }
                }
                else
                {
                    //If boat doesn't have empty seats it will try to unload in current island
                    for (int i1 = 0; i1 < current.boatTransportables.Count; i1++)
                    {
                        Transportable transportable = current.boatTransportables[i1];
                        if (transportable == null)
                            continue;

                        if (!game.UnloadFromBoat(transportable, true))
                        {
                            continue;
                        }

                        next = game.GetCurrentState();
                        if (CheckValidStep(next))
                        {
                            done = true;
                            break;
                        }
                    }
                }

                //If boat couldn't do anything in current island, it try to will travel to another island
                if (!done)
                {
                    foreach (var island in current.islands)
                    {
                        if (island.islandRef != current.currentIsland)
                        {
                            game.MoveBoatToIsland(island.islandRef, true);
                            next = game.GetCurrentState();
                            if (CheckValidStep(next))
                            {
                                done = true;
                                break;
                            }
                        }
                    }
                }

                if (done)
                {
                    next.steps = current.steps + 1;
                    level++;
                }
                else
                {
                    //If can't do anythin, it will undo
                    game.Undo(true);
                    current = current.previousState;
                    level--;
                    //If it tries to undo beyond initial state, the search will fail
                    if (level < 0)
                    {
                        Debug.Log("Can't find a solution");
                        break;
                    }
                }
            }

            int requiredSteps = 0;
            while (current.previousState != null)
            {
                current = current.previousState;
                requiredSteps++;
                game.Undo(true);
            }

            if (iter == maxIter)
            {
                Debug.Log("Need more iter");
            }

            if (!game.Win)
            {
                return -1;
            }

            return requiredSteps;

            ///////////////////////////////////////////////////////////

            //Functions
            bool CheckValidStep(State next)
            {
                if (game.Fail || dict.ContainsKey(next.ToString()))
                {
                    game.Undo(true);
                    return false;
                }

                return true;
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
                transportables = island.Transportables.ToArray();
            }
        }

        public State previousState = null;
        public int steps = 0;

        public Island currentIsland;
        public List<IslandState> islands = new List<IslandState>();

        public List<Transportable> boatTransportables = new List<Transportable>();
        public int boatCapacity = 0;
        public int boatOccupiedSeats = 0;

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
                if (i < boatTransportables.Count && boatTransportables[i] != null)
                {
                    result += "[" + boatTransportables[i] + "] ";
                }
                else
                {
                    result += "[ ] ";
                }
            }
            result += "> (" + currentIsland + ")";

            return result;
        }
    }
}
