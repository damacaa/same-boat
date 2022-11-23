using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    [SerializeField]
    Level[] levels;
    [SerializeField]
    int _currentLevel;

    public GameLogic Game { get; private set; }

    string _rules = "";

    private void Start()
    {
        if (!SceneLoader.Instance)
            LoadLevel(levels[_currentLevel]);
    }

    public void LoadLevel(Level level)
    {
        Game = new GameLogic(level);
        Game.GenerateGameObjects(level);

        foreach (var rule in level.rules)
        {
            _rules += rule + "\n";
        }
    }

    private void Update()
    {
        if (Game == null)
            return;

        if (Input.GetKeyDown("right"))
        {
            Game.Execute();
            print(Game);
        }

        if (Input.GetKeyDown("left"))
        {
            Game.Undo();
            print(Game);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            int steps = Solver.Solver.Solve(Game);
            print(steps);

            StartCoroutine(Game.ShowAllMovesCoroutine());
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            int steps = Solver.Solver.SolveWidth(Game);
            print(steps + " steps");

            StartCoroutine(Game.ShowAllMovesCoroutine());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Game.Reset();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Game.Test();
        }

        if (Game.Fail)
        {

        }

        if (Game.Win)
        {

        }
    }

    internal bool MoveBoatTo(BoatBehaviour boat, IslandBehaviour island)
    {
        return Game.MoveBoatToIsland(island.Data);
    }

    internal bool MoveTransportableTo(TransportableBehaviour transportable, IslandBehaviour island)
    {
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
        return Game.LoadOnBoat(transportable.Data);
    }

    private void OnGUI()
    {
        if (Game == null)
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
            Solver.Solver.SolveWidth(Game);
            StartCoroutine(Game.ShowAllMovesCoroutine());
        }

        GUI.TextArea(new Rect(Screen.width - (3 * width) - space, space, 3 * width, 5 * height), _rules);
    }
}
