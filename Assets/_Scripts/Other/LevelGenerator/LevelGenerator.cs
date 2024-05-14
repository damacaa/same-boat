using Solver;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelGenerator", menuName = "ScriptableObjects/LevelGenerator", order = 3)]

public class LevelGenerator : ScriptableObject
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


    [SerializeField]
    private CancellationTokenSource _cancellationTokenSource;
    [SerializeField]
    private Task _task;

    [SerializeField]
    public bool IsGenerating { get; internal set; }

    public void Generate()
    {

#if UNITY_EDITOR
        IsGenerating = true;
        _cancellationTokenSource = new CancellationTokenSource();
        _task = Task.Run(() => { GenerateLevel(_cancellationTokenSource); });

#endif

    }

    public void Cancel()
    {
        Debug.Log("Bye");
        _cancellationTokenSource?.Cancel();
    }

    private void GenerateLevel(CancellationTokenSource cancellationToken) {

        int maxIter = 100;
        int iter = 0;

        GameLogic game = null;
        Level level = null;

        while (iter < maxIter && !cancellationToken.IsCancellationRequested)
        {

            Dictionary<TransportableSO, int> count = new Dictionary<TransportableSO, int>();

            level = ScriptableObject.CreateInstance<Level>();
            level.name = "Random";
            //level.Islands = new Level.Island[Random.Range(2, 5)];
            level.Islands = new Level.IslandData[2];

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
                level.Islands[i] = new Level.IslandData();
                Level.IslandData island = level.Islands[i];

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
            //Solver.Solver.SolveWidth(game, true);

            if (!game.Win || game.Boat.Crossings < _desiredCrossings)
                continue;

            var state = game.GetCurrentState();
            level.BoatMaxTravelCost = state.BoatTravelCost;

        };

        IsGenerating = false;

        if (cancellationToken.IsCancellationRequested)
        {
            Debug.Log("Aborted");
            return;
        }

        Level levelWithSmallerWeightLimit = level;
        levelWithSmallerWeightLimit.BoatMaxWeightAllowed--;

        game.Reset();
        while (!game.Win)
        {
            //Solver.Solver.SolveWidth(new GameLogic(levelWithSmallerWeightLimit), true);
            level = levelWithSmallerWeightLimit;
            levelWithSmallerWeightLimit.BoatMaxWeightAllowed--;
        }

        if (iter == maxIter)
            Debug.Log("Need more steps");

        AssetDatabase.CreateAsset(level, "Assets/ScriptableObjects/Levels/Random.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }
}
