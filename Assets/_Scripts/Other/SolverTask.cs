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
                State solution = Solver.Solve(game, useHeuristic);

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