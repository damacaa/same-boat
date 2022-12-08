using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Solver
{
    public class Solver
    {
        /*public static int SolveDepth(GameLogic game)
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
        }*/

        public static State SolveWidth(GameLogic game, bool useHeuristic = false)
        {
            List<State> openList = new List<State>();
            List<State> closedList = new List<State>();

            State current = game.GetCurrentState();

            openList.Add(current);

            int iter = 0;
            int maxIter = 5000;
            while (openList.Count > 0 && iter < maxIter)
            {
                iter++;

                if (useHeuristic)
                    openList = openList.OrderByDescending(t => t.F).ToList();

                current = openList[0];
                SetState(current, game);
                openList.Remove(current);
                closedList.Add(current);

                if (game.Win) break;

                if (game.GetCurrentState().ToString() != current.ToString())
                {
                    Debug.LogError("FUCKKKKK");
                }

                ExpandNeighbours(current, game, openList, closedList);
            }

            if (!game.Win)
            {
                if (iter == maxIter)
                    Debug.Log("Need more iter");

                return null;
            }

            int requiredCrossings = game.Boat.Crossings;
            int requiredSteps = 0;

            State aux = current;
            while (aux.PreviousState != null)
            {
                aux = aux.PreviousState;
                requiredSteps++;
                game.Undo(true);
            }

            return current;
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

                current = openList[0];
                SetState(current, game);
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
                if (iter == maxIter)
                    Debug.Log("Need more iter");

                yield return null;
            }

            while (current.PreviousState != null)
            {
                current = current.PreviousState;
                game.Undo(true);
            }

            yield return null;


        }

        static void SetState(State state, GameLogic game)
        {
            int iter = 0;

            List<State> states = new List<State>();
            State aux = state;
            while (aux != null && iter < 100)
            {
                states.Add(aux);
                aux = aux.PreviousState;
                iter++;
            }
            states.Reverse();

            /* aux = current;
             while (aux.PreviousState != null && iter < 100)
             {
                 game.Undo(true);
                 aux = aux.PreviousState;
             }*/

            game.Reset(true);

            for (int i = 1; i < states.Count; i++)
            {
                if (!(states[i].Bommand != null && game.AddCommand(states[i].Bommand, true)))
                {
                    Debug.Log("Fuck " + iter);
                }
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

            //Move to another island
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
            if (!(game.Fail || openList.Contains(newState) || closedList.Contains(newState)))
            {
                newState.PreviousState = current;
                openList.Add(newState);
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

        public Command Bommand;

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
