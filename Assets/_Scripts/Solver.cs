using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solver
{
    public class Solver
    {
        public static void Solve(State initial)
        {

        }

        public static bool CheckFail(List<Transportable> transportables)
        {
            bool success = true;

            foreach (var a in transportables)
            {
                foreach (var b in transportables)
                {
                    if(a == null || b == null)
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

    public class State
    {

    }
}
