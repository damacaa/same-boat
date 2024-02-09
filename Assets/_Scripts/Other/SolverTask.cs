using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Solver
{

    public class SolverTask : Solver
    {
        public static new SolverResult SolveWidthAndReset(GameLogic game, bool useHeuristic = true)
        {
            SolverResult result = new SolverResult();

            Task.Run(() =>
            {

                State current = Initialize(game, useHeuristic, out PriorityQueue<State> nodeQueue, out HashSet<State> openList, out HashSet<State> closedList);

                while (openList.Count > 0)
                {
                    // Sets game state
                    State previous = current;
                    current = nodeQueue.Dequeue();

                    SetState(current, previous, game);

                    openList.Remove(current);
                    closedList.Add(current);

                    // Checks win and brakes loop if true
                    if (game.Win) break;

                    // Adds to the open list all of the new possible states derived from the current state
                    ExpandNeighbours(current, game, openList, closedList, nodeQueue, true);

                }

                result.Ready = true;

                if (!game.Win)
                {
                    return;
                }

                // Reset game to initial state
                ResetGame(game, current);
            });


            return result;
        }



        public static IEnumerator SolveCoroutine(GameLogic game, bool useHeuristic = true)
        {
            var result = SolveWidthAndReset(game, useHeuristic);

            while (!result.Ready)
            {
                yield return null;
            }

            yield return null;
        }
    }

    public class SolverResult
    {
        public bool Ready { get; internal set; }
    }

}