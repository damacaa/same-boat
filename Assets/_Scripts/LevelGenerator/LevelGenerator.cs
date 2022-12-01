using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    int _desiredSteps = 10;
    [SerializeField]
    TransportableSO[] _transportables;
    [SerializeField]
    Rule[] _rules;
    [SerializeField]
    Texture2D[] maps;

    public void Generate()
    {
        int maxIter = 100;
        int iter = 0;

        Level level = null;
        do
        {
            Dictionary<TransportableSO, int> count = new Dictionary<TransportableSO, int>();

            level = ScriptableObject.CreateInstance<Level>();
            level.name = "Random";
            level.Islands = new Level.Island[2];
            for (int i = 0; i < level.Islands.Length; i++)
            {
                level.Islands[i] = new Level.Island();
                Level.Island island = level.Islands[i];

                if (i >= level.Islands.Length - 1)
                {
                    island.transportables = new TransportableSO[0];
                    continue;
                }

                island.transportables = new TransportableSO[Random.Range(0, 6)];
                for (int j = 0; j < island.transportables.Length; j++)
                {
                    TransportableSO t = _transportables[Random.Range(0, _transportables.Length)];

                    if (count.TryGetValue(t, out int value))
                    {
                        count[t] = value + 1;
                    }
                    else
                    {
                        count.Add(t, 1);
                    }

                    island.transportables[j] = t;
                    if (island.transportables[j].name == "Man")
                        level.OnlyHumansCanDrive = true;
                }

            }
            level.Map = maps[level.Islands.Length - 2];

            List<Rule> rules = new List<Rule>();

            foreach (var rule in _rules)
            {
                if(count[rule.A] > 0 && count[rule.B] > 0)
                {
                    rules.Add(rule);
                }
            }

            level.Rules = rules.ToArray();

            iter++;

        } while (iter < maxIter && Solver.Solver.SolveWidth(new GameLogic(level)) < _desiredSteps);


        AssetDatabase.CreateAsset(level, "Assets/ScriptableObjects/Levels/Random.asset");
        AssetDatabase.SaveAssets();
    }
}
