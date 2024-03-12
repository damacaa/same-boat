using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using System.Threading;
using Unity.VisualScripting;

namespace Solver
{

    public class SolverCoroutine : Solver
    {
        public static new IEnumerator SolveWidthAndReset(GameLogic game, bool useHeuristic = true)
        {
            float maxTime = 1f / (2f * Application.targetFrameRate);

            State current = Initialize(game, useHeuristic, out PriorityQueue<State> nodeQueue, out HashSet<State> openList, out HashSet<State> closedList);

            while (openList.Count > 0)
            {
                float t0 = Time.realtimeSinceStartup;
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

                if (Time.realtimeSinceStartup - t0 < maxTime)
                {
                    yield return null;
                }
            }

            if (!game.Win)
            {
                yield break;
            }

            // Reset game to initial state
            ResetGame(game, current);

            yield return null;
        }
    }
}


