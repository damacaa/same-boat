
using Localization;
using Solver;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }

    private const float MIN_SOLVER_TIME = 1.5f;

    [SerializeField]
    private UIGame _ui;
    [SerializeField]
    private Renderer _water;

    // Debug stuff
    [SerializeField]
    private LevelCollection levels;
    [SerializeField]
    int _currentLevel;
    [SerializeField]
    bool _showDebugUI;

    // Solver
    [Header("Solver settings")]
    [SerializeField]
    bool _useHeuristicForSolver;
    public enum SolverMethod
    {
        Normal,
        Coroutine,
        Task
    }
    [SerializeField]
    SolverMethod _solverMethod;



    private GameLogic _game;
    private string _levelDescription;

    private Level _levelToLoad;
    private bool _isWin;
    private bool _isFail;
    private Level _loadedLevel;
    private CancellationTokenSource _solverCancellationToken;

    public event Action OnLevelLoaded;
    public event Action OnSolverStarted;
    public event Action OnSolverEnded;

    private void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }

    #region UnityEventFunctions
    private void Start()
    {
        LocalizationManager.Update();

#if !UNITY_EDITOR
        _showDebugUI = false;
#endif
        InputSystem.InputEnabled = true;


#if UNITY_EDITOR
        Application.targetFrameRate = 120;
#endif

        SoundController.Instace.PlaySong(1);

        if (ProgressManager.Instance)
        {
            // TO DO Load selected level from progress manager
            LoadLevel(ProgressManager.Instance.LevelToLoad);
        }
        else
        {
            LoadLevel(levels[_currentLevel]);
        }
    }

    private void Update()
    {
        if (_game == null)
            return;

#if UNITY_EDITOR
        if (Input.GetKeyDown("right"))
        {
            _game.Execute();
            print(_game);
        }

        if (Input.GetKeyDown("left"))
        {
            Undo();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _showDebugUI = !_showDebugUI;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            ScreenCapture.CaptureScreenshot("screen.png");
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            LocalizationManager.SetLanguage(Language.En);
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            LocalizationManager.SetLanguage(Language.Es);
        }


#endif


        if (Time.time > _timeToStopRewinding)
            _timeIncrement = 1.0f;

        SoundController.Instace.SetSpeed(_timeIncrement);

        _time += Time.deltaTime * _timeIncrement;
        _water.material.SetFloat("_T", _time);
    }

    float _time = 0;
    float _timeIncrement = 1;
    float _timeToStopRewinding = 0;
    private Coroutine _c;

    // Show debug UI
    private void OnGUI()
    {
        if (_game == null || !_showDebugUI)
            return;

        int width = Screen.width / 8;
        int height = Screen.height / 20;
        int space = Screen.height / 50;

        if (GUI.Button(new Rect(space, space, width, height), "Undo"))
            _game.Undo();

        if (GUI.Button(new Rect(space, space + height + space, width, height), "Reset"))
        {
            if (_isWin || _isFail)
                SoundController.Instace.PlaySong(1);

            _game.Reset();
        }

        if (GUI.Button(new Rect(space, 2 * (space + height) + space, width, height), "Solve"))
        {
            switch (_solverMethod)
            {
                case SolverMethod.Normal:
                    {
                        float startTime = Time.realtimeSinceStartup;
                        Solver.Solver.SolveWidthAndReset(_game, _useHeuristicForSolver);
                        float elapsedTime = Time.realtimeSinceStartup - startTime;

                        Debug.Log($"Elapsed normal {elapsedTime}");

                        StartCoroutine(_game.ExecuteAllMovesCoroutine());
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
            RequestClue();
        }

        if (GUI.Button(new Rect(space, 4 * (space + height) + space, width, height), "Screenshot"))
        {
            ScreenCapture.CaptureScreenshot("screen.png");
        }

        // GUI.TextArea(new Rect(Screen.width - (3 * width) - space, space * 5, 3 * width, 5 * height), LevelDescription);

        width = height;
        for (int i = 0; i < levels.Count; i++)
        {
            if (GUI.Button(new Rect(space + i * (space + width), Screen.height - space - height, width, height), (i + 1).ToString()))
            {
                LoadLevel(levels[i]);
            }
        }
    }
    #endregion

    public void LoadLevel(Level level)
    {
        // Clear previous game
        if (_game != null)
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

            _game = null;
        }

        _game = new GameLogic(level);
        _game.GenerateGameObjects(level);

        _game.OnWin += HandleWin;
        _game.OnFail += HandleFail;

        _ui.SetLevelDetails(level);
        _ui.SetGameState(_game.GetCurrentState());
        _ui.SetState(UIGame.UIState.Playing);

        Debug.Log(level);

        _loadedLevel = level;

        OnLevelLoaded?.Invoke();
    }

    public void Undo()
    {
        //StopAllCoroutines();
        _game.CancelMoveCOrorutine();


        if (_isWin)
            return;

        if (_isFail)
        {
            _ui.SetState(UIGame.UIState.Playing);
            _isFail = false;
            SoundController.Instace.PlaySong(1);
        }

        _game.Undo(out float animationDuration);
        _timeToStopRewinding = Time.time + animationDuration;
        _ui.SetGameState(_game.GetCurrentState());

        _timeIncrement = -2.0f;
    }


    public void RequestClue()
    {
        StartCoroutine(ClueCoroutine());
    }

    public void ResetGame()
    {
        StopAllCoroutines();

        _game.Reset();

        _isWin = false;
        _isFail = false;
        SoundController.Instace.PlaySong(1);
        _ui.SetGameState(_game.GetCurrentState());
        _ui.SetState(UIGame.UIState.Playing);

    }

    private void HandleWin()
    {
        SoundController.Instace.PlaySound(SoundController.Sound.Win);
        ProgressManager.Instance?.CompleteLevel();

        _ui.SetGameState(_game.GetCurrentState());
        _ui.SetState(UIGame.UIState.Win);
        _isWin = true;
    }

    private void HandleFail()
    {
        SoundController.Instace.PlaySound(SoundController.Sound.Fail);
        _ui.SetState(UIGame.UIState.Fail);
        _isFail = true;
    }

    private IEnumerator SolveCoroutine()
    {
        if (OnSolverStarted != null)
            OnSolverStarted();

        float startTime = Time.realtimeSinceStartup;

        switch (_solverMethod)
        {
            case SolverMethod.Coroutine:
                yield return SolverCoroutine.SolveWidthAndReset(_game, _useHeuristicForSolver);
                break;
            case SolverMethod.Task:
                if (_solverCancellationToken != null)
                    _solverCancellationToken.Cancel();
                _solverCancellationToken = new CancellationTokenSource();
                yield return SolverTask.SolveCoroutine(_game, _solverCancellationToken, _useHeuristicForSolver);
                break;
        }

        float elapsedTime = Time.realtimeSinceStartup - startTime;

        Debug.Log($"Elapsed {_solverMethod}: {elapsedTime}");

        if (OnSolverEnded != null)
            OnSolverEnded();

        yield return _game.ExecuteAllMovesCoroutine();

        if (_game.GetCurrentState().Crossings > 0 &&
            _game.GetCurrentState().Crossings < _loadedLevel.OptimalCrossings)
        {
            _loadedLevel.OptimalCrossings = _game.GetCurrentState().Crossings;
#pragma warning disable CS0618 // Type or member is obsolete
            _loadedLevel.SetDirty();
#pragma warning restore CS0618 // Type or member is obsolete
        }


        yield return null;
    }

    private IEnumerator ClueCoroutine()
    {
        OnSolverStarted?.Invoke();

        _ui.ShowLoadingScreen();

        var t0 = Time.time;

        switch (_solverMethod)
        {
            case SolverMethod.Coroutine:
                yield return SolverCoroutine.SolveWidthAndReset(_game, _useHeuristicForSolver);
                break;
            case SolverMethod.Task:
                if (_solverCancellationToken != null)
                    _solverCancellationToken.Cancel();
                _solverCancellationToken = new CancellationTokenSource();
                yield return SolverTask.SolveCoroutine(_game, _solverCancellationToken, _useHeuristicForSolver);
                break;
        }

        var timeEllapsed = Time.time - t0;

        if (timeEllapsed < MIN_SOLVER_TIME)
        {
            yield return new WaitForSeconds(MIN_SOLVER_TIME - timeEllapsed);
        }


        OnSolverEnded?.Invoke();

        yield return null;

        _game.Execute();


        _ui.HideLoadingScreen();

        _ui.SetGameState(_game.GetCurrentState());

        yield return null;
    }

    internal bool MoveBoatTo(BoatBehaviour boat, IslandBehaviour island)
    {
        if (_isWin || _isFail)
            return false;

        bool success = _game.MoveBoatToIsland(island.Data);
        if (success)
            _ui.SetGameState(_game.GetCurrentState());

        return success;
    }

    internal bool MoveTransportableTo(TransportableBehaviour transportable, IslandBehaviour island)
    {
        if (_isWin || _isFail)
            return false;

        //WTF is this
        if (island.Data != _game.Boat.Island)
        {
            if (!_game.MoveBoatToIsland(island.Data, true))
            {
                Debug.Log("Can't move boat");
                return false;
            }

            if (!_game.UnloadFromBoat(transportable.Data, true))
            {
                Debug.Log("Can't unload boat");
                _game.Undo(true);
                return false;
            }

            _game.Undo(true);
            _game.Undo(true);


            StartCoroutine(_game.ExecuteAllMovesCoroutine());

            return true;
        }
        //return false; // Game.MoveBoatToIsland(island.Data)

        bool success = _game.UnloadFromBoat(transportable.Data);

        if (success)
            _ui.SetGameState(_game.GetCurrentState());

        return success;
    }

    internal bool LoadTransportableOnBoat(TransportableBehaviour transportable, BoatBehaviour boat)
    {
        if (_isWin || _isFail)
            return false;

        bool success = _game.LoadOnBoat(transportable.Data);
        if (success)
            _ui.SetGameState(_game.GetCurrentState());

        return success;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoandNextLevel()
    {
        if (!ProgressManager.Instance)
            return;

        for (int i = 0; i < ProgressManager.Instance.Levels.Count; i++)
        {
            if (_loadedLevel == ProgressManager.Instance.Levels[i])
            {
                if (i + 1 >= ProgressManager.Instance.Levels.Count)
                {
                    // TODO: hide button
                    return;
                }

                ProgressManager.Instance.LevelToLoad = ProgressManager.Instance.Levels[i + 1];
                break;
            }
        }

        SceneManager.LoadScene(1);
    }

    private void OnDestroy()
    {
        if (_solverCancellationToken != null)
            _solverCancellationToken.Cancel();
    }
}