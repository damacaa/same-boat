
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameLogic Game { get; private set; }

    [SerializeField]
    Level[] levels;
    [SerializeField]
    int _currentLevel;

    string _levelDescription = "";
    public string LevelDescription
    {
        get { return _levelDescription; }
        private set { _levelDescription = value; }
    }

    public delegate void Delegate();
    public event Delegate OnLevelLoaded;
    public event Delegate OnSolverStarted;
    public event Delegate OnSolverEnded;

    [SerializeField]
    bool _useHeuristicForSolver = false;
    [SerializeField]
    bool _showDebugUI = false;

    private void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }

    [HideInInspector]
    public bool Win { get; private set; }
    [HideInInspector]
    public bool Fail { get; private set; }


    private void Start()
    {
#if !UNITY_EDITOR
        _showDebugUI = false;
#endif

        SoundController.Instace.PlaySong(1);

        if (!ProgressManager.Instance)
            LoadLevel(levels[_currentLevel]);
    }

    public void LoadLevel(Level level)
    {
        if (Game != null)
        {
            var transportables = FindObjectsOfType<TransportableBehaviour>();
            foreach (var t in transportables)
            {
                Destroy(t.gameObject);
            }

            var boat = FindObjectOfType<BoatBehaviour>();
            Destroy(boat.gameObject);

            var map = FindObjectOfType<MapGenerator>();
            for (int i = 0; i < map.transform.childCount; i++)
            {
                Destroy(map.transform.GetChild(i).gameObject);
            }

            Game = null;
        }

        Game = new GameLogic(level);
        Game.GenerateGameObjects(level);

        Game.OnWin += delegate { SoundController.Instace.PlaySound(SoundController.Sound.Win); };
        Game.OnFail += delegate { SoundController.Instace.PlaySound(SoundController.Sound.Fail); };

        LevelDescription = level.ToString();

        if (OnLevelLoaded != null) OnLevelLoaded();
    }

    private void Update()
    {
        if (Game == null)
            return;

        if (Fail != Game.Fail)
        {
            Fail = Game.Fail;
            if (Fail)
            {
                SoundController.Instace.PlaySound(SoundController.Sound.Fail);
                //if (OnGameOver != null) OnGameOver();
            }
        }

        if (Win != Game.Win)
        {
            Win = Game.Win;
            if (Win)
                SoundController.Instace.PlaySound(SoundController.Sound.Win);
            //if (OnVictory != null) OnVictory();
        }

#if UNITY_EDITOR

        if (Input.GetKeyDown("right"))
        {
            Game.Execute();
            print(Game);
        }

        if (Input.GetKeyDown("left"))
        {
            Undo();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            ScreenCapture.CaptureScreenshot("screen.png");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            /*int steps = Solver.Solver.SolveWidth(Game);
            print(steps + " steps");

            if (steps != -1)
                StartCoroutine(Game.ShowAllMovesCoroutine());*/
            StartCoroutine(SolveCoroutine());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
#endif
    }

    IEnumerator SolveCoroutine()
    {
        if (OnSolverStarted != null)
            OnSolverStarted();

        float startTime = Time.realtimeSinceStartup;
        yield return Solver.Solver.SolveWidthCoroutine(Game, _useHeuristicForSolver);
        float elapsedTime = Time.realtimeSinceStartup - startTime;

        if (OnSolverEnded != null)
            OnSolverEnded();
        yield return Game.ShowAllMovesCoroutine();

        //print($"Used heuristic: {_useHeuristicForSolver}\n" +
        //    $"Time elapsed: {elapsedTime}s\n" +
        //    $"Crossings: {Game.Boat.Crossings}\n" +
        //    $"Travel cost: {Game.Boat.CurrentTravelCost}");

        yield return null;
    }

    IEnumerator ClueCoroutine()
    {
        if (OnSolverStarted != null)
            OnSolverStarted();

        yield return Solver.Solver.SolveWidthCoroutine(Game, _useHeuristicForSolver);

        if (OnSolverEnded != null)
            OnSolverEnded();

        yield return null;

        Game.Execute();

        yield return null;
    }

    public void Undo()
    {
        if (Win)
            return;

        if (Fail)
        {
            print("Undo fail");
            Fail = false;
            SoundController.Instace.PlaySong(1);
        }

        Game.Undo();
    }

    public void Reset()
    {
        StopAllCoroutines();

        Game.Reset();

        Win = false;
        Fail = false;
        SoundController.Instace.PlaySong(1);
    }


    internal bool MoveBoatTo(BoatBehaviour boat, IslandBehaviour island)
    {
        if (Win || Fail)
            return false;

        return Game.MoveBoatToIsland(island.Data);
    }

    internal bool MoveTransportableTo(TransportableBehaviour transportable, IslandBehaviour island)
    {
        if (Win || Fail)
            return false;

        if (island.Data != Game.Boat.Island)
        {
            if (!Game.MoveBoatToIsland(island.Data, true))
                return false;

            if (!Game.UnloadFromBoat(transportable.Data, true))
            {
                Game.Undo(true);
                return false;
            }

            Game.Undo(true);
            Game.Undo(true);

            StartCoroutine(Game.ShowAllMovesCoroutine());

            return true;
        }
        //return false; // Game.MoveBoatToIsland(island.Data)

        return Game.UnloadFromBoat(transportable.Data);
    }

    internal bool MoveTransportableTo(TransportableBehaviour transportable, BoatBehaviour boat)
    {
        if (Win || Fail)
            return false;

        return Game.LoadOnBoat(transportable.Data);
    }

    private void OnGUI()
    {
        if (Game == null || !_showDebugUI)
            return;

        int width = Screen.width / 8;
        int height = Screen.height / 20;
        int space = Screen.height / 50;

        if (GUI.Button(new Rect(space, space, width, height), "Undo"))
            Game.Undo();

        if (GUI.Button(new Rect(space, space + height + space, width, height), "Reset"))
            Game.Reset();

        if (GUI.Button(new Rect(space, 2 * (space + height) + space, width, height), "Solve"))
        {
            //StartCoroutine(SolveCoroutine());
            Solver.Solver.SolveWidth(Game, false);
            StartCoroutine(Game.ShowAllMovesCoroutine());
        }

        if (GUI.Button(new Rect(space, 3 * (space + height) + space, width, height), "Clue"))
        {
            StartCoroutine(ClueCoroutine());
        }

        // GUI.TextArea(new Rect(Screen.width - (3 * width) - space, space * 5, 3 * width, 5 * height), LevelDescription);

        width = height;
        for (int i = 0; i < levels.Length; i++)
        {
            if (GUI.Button(new Rect(space + i * (space + width), Screen.height - space - height, width, height), (i + 1).ToString()))
            {
                LoadLevel(levels[i]);
            }
        }
    }
}