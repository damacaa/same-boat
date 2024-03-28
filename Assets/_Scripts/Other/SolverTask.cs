using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Solver
{

    public class SolverTask : Solver
    {
        // Problem: inherits things we don't want
        


        private static SolverResult SolveWidthAndReset(GameLogic game, CancellationTokenSource cancellationToken, bool useHeuristic = true)
        {
            SolverResult result = new SolverResult();

            result.Task = Task.Run(() =>
            {
                State solution = Solver.Solve(game, useHeuristic, cancellationToken);

                result.Ready = true;

                if (!game.Win)
                {
                    return;
                }

                // Reset game to initial state
                ResetGame(game, solution);
            });

            return result;
        }

        public static IEnumerator SolveCoroutine(GameLogic game, CancellationTokenSource cancellationToken, bool useHeuristic = true)
        {
            var result = SolveWidthAndReset(game, cancellationToken, useHeuristic);

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
        public Task Task { get; internal set; }
    }

}