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
#if UNITY_EDITOR
        int maxIter = 100;
        int iter = 0;

        Level level = null;
        int steps = -1;

        do
        {
            Dictionary<TransportableSO, int> count = new Dictionary<TransportableSO, int>();

            level = ScriptableObject.CreateInstance<Level>();
            level.name = "Random";
            level.Islands = new Level.Island[Random.Range(2, 5)];

            if (Random.value > 0.75f)
                level.BoatCapacity = Random.Range(2, 4);

            if (Random.value > 0.75f)
                level.BoatMaxWeightAllowed = Random.Range(0, 20);

            if (Random.value > 0.75f)
                level.BoatMaxTravelCost = Random.Range(0, 30);

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
                    TransportableSO t = _transportables[Random.Range(level.OnlyHumansCanDrive ? 1 : 0, _transportables.Length)];

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
                if (count.ContainsKey(rule.A) && count.ContainsKey(rule.A))
                {
                    rules.Add(rule);
                }
            }

            level.Rules = rules.ToArray();

            iter++;

            steps = Solver.Solver.SolveWidth(new GameLogic(level));
            print($"{iter}: {steps}");

        } while (iter < maxIter && steps < _desiredSteps);

        if (iter == maxIter)
            Debug.Log("Need more steps");

        AssetDatabase.CreateAsset(level, "Assets/ScriptableObjects/Levels/Random.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
}
