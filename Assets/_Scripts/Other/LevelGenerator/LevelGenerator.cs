using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    int _desiredCrossings = 10;
    [SerializeField]
    int _desiredCapacity = 2;
    [SerializeField]
    int _desiredWeightLimit = 0;
    [SerializeField]
    int _desiredMaxTravelCost = 0;

    [Space]
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

        GameLogic game = null;
        Level level = null;

        while (iter < maxIter)
        {

            Dictionary<TransportableSO, int> count = new Dictionary<TransportableSO, int>();

            level = ScriptableObject.CreateInstance<Level>();
            level.name = "Random";
            //level.Islands = new Level.Island[Random.Range(2, 5)];
            level.Islands = new Level.Island[2];

            if (_desiredCapacity != level.BoatCapacity)
                level.BoatCapacity = _desiredCapacity;
            else
                level.BoatCapacity = Random.Range(2, 4);

            if (_desiredWeightLimit != level.BoatMaxWeightAllowed)
                level.BoatMaxWeightAllowed = _desiredWeightLimit;
            else
                level.BoatMaxWeightAllowed = Random.Range(0, 20);

            if (_desiredMaxTravelCost != level.BoatMaxTravelCost)
                level.BoatMaxTravelCost = _desiredMaxTravelCost;
            else
                level.BoatMaxTravelCost = Random.Range(0, 100);

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
                if (count.ContainsKey(rule.A) && count.ContainsKey(rule.B))
                {
                    rules.Add(rule);
                }
            }

            level.Rules = rules.ToArray();

            iter++;

            game = new GameLogic(level);
            var state = Solver.Solver.SolveWidth(game, true);

            if (state == null || game.Boat.Crossings < _desiredCrossings)
                continue;

            level.BoatMaxTravelCost = state.BoatTravelCost;

        };

        Level levelWithSmallerWeightLimit = level;
        levelWithSmallerWeightLimit.BoatMaxWeightAllowed--;

        while (Solver.Solver.SolveWidth(new GameLogic(levelWithSmallerWeightLimit), true) != null)
        {
            level = levelWithSmallerWeightLimit;
            levelWithSmallerWeightLimit.BoatMaxWeightAllowed--;
        }

        if (iter == maxIter)
            Debug.Log("Need more steps");

        AssetDatabase.CreateAsset(level, "Assets/ScriptableObjects/Levels/Random.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif

    }
}
