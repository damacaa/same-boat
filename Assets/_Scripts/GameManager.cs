
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
            Destroy(this);
        else
            instance = this;
    }

    [SerializeField]
    Level[] levels;
    [SerializeField]
    int _currentLevel;


    public GameLogic Game { get; private set; }

    string _levelDescription = "";


    private void Start()
    {
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

        _levelDescription = level.ToString();
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

            if (steps != -1)
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
            SoundController.Instace.PlaySound(SoundController.Sound.Fail);
        }

        if (Game.Win)
        {
            SoundController.Instace.PlaySound(SoundController.Sound.Win);
            print(Game.Boat.Crossings);
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
        if (Game == null || false)
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

        if (GUI.Button(new Rect(space, 3 * (space + height) + space, width, height), "Clue"))
        {
            Solver.Solver.SolveWidth(Game);
            Game.Execute();
        }

        GUI.TextArea(new Rect(Screen.width - (3 * width) - space, space * 3, 3 * width, 10 * height), _levelDescription);

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


