
using Solver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  

    // Debug stuff
    [SerializeField]
    Level[] levels;
    [SerializeField]
    int _currentLevel;

    // Singleton
    public static GameManager Instance { get; private set; }

    public GameLogic Game { get; private set; }
    public string LevelDescription { get; private set; }

    private bool _isWin;
    private bool _isFail;

    public event Action OnLevelLoaded;
    public event Action OnSolverStarted;
    public event Action OnSolverEnded;

    public enum SolverMethod
    {
        Normal,
        Coroutine,
        Task
    }

    [Header("Solver settings")]
    [SerializeField]
    bool _useHeuristicForSolver = false;
    [SerializeField]
    SolverMethod _solverMethod;
    [SerializeField]
    bool _showDebugUI = false;


    private void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
#if !UNITY_EDITOR
        _showDebugUI = false;
#endif

        Application.targetFrameRate = 60;

        SoundController.Instace.PlaySong(1);

        if (!ProgressManager.Instance)
            LoadLevel(levels[_currentLevel]);

    }

    public void LoadLevel(Level level)
    {
        // Clear previous game
        if (Game != null)
        {
            ClearPreviousGame();
        }

        Game = new GameLogic(level);
        Game.GenerateGameObjects(level);

        Game.OnWin += HandleWin;
        Game.OnFail += HandleFail;

        LevelDescription = level.ToString();

        OnLevelLoaded?.Invoke();


        void ClearPreviousGame()
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
    }

    private void HandleWin()
    {
        SoundController.Instace.PlaySound(SoundController.Sound.Win);
    }

    private void HandleFail()
    {
        SoundController.Instace.PlaySound(SoundController.Sound.Fail);
    }


    private void Update()
    {
        if (Game == null)
            return;

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

#endif
    }

    IEnumerator SolveCoroutine()
    {
        if (OnSolverStarted != null)
            OnSolverStarted();

        float startTime = Time.realtimeSinceStartup;

        switch (_solverMethod)
        {
            case SolverMethod.Coroutine:
                yield return SolverCoroutine.SolveWidthAndReset(Game, _useHeuristicForSolver);
                break;
            case SolverMethod.Task:
                yield return SolverTask.SolveCoroutine(Game, _useHeuristicForSolver);
                break;
        }

        float elapsedTime = Time.realtimeSinceStartup - startTime;

        Debug.Log($"Elapsed {_solverMethod}: {elapsedTime}");

        if (OnSolverEnded != null)
            OnSolverEnded();

        yield return Game.ExecuteAllMovesCoroutine();
    }


    IEnumerator ClueCoroutine()
    {
        if (OnSolverStarted != null)
            OnSolverStarted();

        switch (_solverMethod)
        {
            case SolverMethod.Coroutine:
                yield return SolverCoroutine.SolveWidthAndReset(Game, _useHeuristicForSolver);
                break;
            case SolverMethod.Task:
                yield return SolverTask.SolveCoroutine(Game, _useHeuristicForSolver);
                break;
        }

        if (OnSolverEnded != null)
            OnSolverEnded();

        yield return null;

        Game.Execute();

        yield return null;
    }


    public void Undo()
    {
        if (_isWin)
            return;

        if (_isFail)
        {
            _isFail = false;
            SoundController.Instace.PlaySong(1);
        }

        Game.Undo();
    }

    public void ResetGame()
    {
        StopAllCoroutines();

        Game.Reset();

        _isWin = false;
        _isFail = false;
        SoundController.Instace.PlaySong(1);
    }

    internal bool MoveBoatTo(BoatBehaviour boat, IslandBehaviour island)
    {
        if (_isWin || _isFail)
            return false;

        return Game.MoveBoatToIsland(island.Data);
    }

    internal bool MoveTransportableTo(TransportableBehaviour transportable, IslandBehaviour island)
    {
        if (_isWin || _isFail)
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

            StartCoroutine(Game.ExecuteAllMovesCoroutine());

            return true;
        }
        //return false; // Game.MoveBoatToIsland(island.Data)

        return Game.UnloadFromBoat(transportable.Data);
    }

    internal bool LoadTransportableOnBoat(TransportableBehaviour transportable, BoatBehaviour boat)
    {
        if (_isWin || _isFail)
            return false;

        return Game.LoadOnBoat(transportable.Data);
    }


    // Show debug UI
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
            switch (_solverMethod)
            {
                case SolverMethod.Normal:
                    {
                        float startTime = Time.realtimeSinceStartup;
                        Solver.Solver.SolveWidthAndReset(Game, _useHeuristicForSolver);
                        float elapsedTime = Time.realtimeSinceStartup - startTime;

                        Debug.Log($"Elapsed normal {elapsedTime}");

                        StartCoroutine(Game.ExecuteAllMovesCoroutine());
                        break;
                    }
                case SolverMethod.Coroutine:
                case SolverMethod.Task:
                    StartCoroutine(SolveCoroutine());
                    break;
            }
        }

        if (GUI.Button(new Rect(space, 3 * (space + height) + space, width, height), "Clue"))
        {
            StartCoroutine(ClueCoroutine());
        }

        if (GUI.Button(new Rect(space, 4 * (space + height) + space, width, height), "Screenshot"))
        {
            ScreenCapture.CaptureScreenshot("screen.png");
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