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
    Transportable _selectedTransportable;
    Boat _boat;

    private void Start()
    {
        if (!SceneLoader.Instance)
            LoadLevel(levels[_currentLevel]);
    }

    public void LoadLevel(Level level)
    {
        Game = new GameLogic(level);
        Game.GenerateGameObjects();
        _boat = Game.Boat;
        _boat.GoTo(Game.FirstIsland, out float animationDuration, true);
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
            _selectedTransportable = null;
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
    }

    public void IslandInteraction(Island island)
    {
        //print(island.Name);
        if (_boat.GetCurrentIsland() != island)
        {
            _selectedTransportable = null;
            Game.MoveBoatToIsland(island);
        }
        else
        {
            if (_selectedTransportable != null && _boat.Contains(_selectedTransportable))
            {
                Game.UnloadFromBoat(_selectedTransportable);
                _selectedTransportable = null;
            }
        }
    }

    public void TransportableInteraction(Transportable transportable)
    {
        if (_boat.GetCurrentIsland().Contains(transportable))
        {
            _selectedTransportable = transportable;
            print("Select " + _selectedTransportable);
        }
        else if (_boat.Contains(transportable))
        {
            _selectedTransportable = transportable;
            print("Select " + _selectedTransportable);
        }
        else
        {
            Game.MoveBoatToIsland(transportable.Island);
            //_boat.GetCurrentIsland() = transportable.Island;
            //game.LoadOnBoat(transportable);
        }
    }

    public void BoatInteraction(Boat boat)
    {
        _boat = boat;
        if (_selectedTransportable != null && Game.LoadOnBoat(_selectedTransportable))
        {
            //print("Load " + _selectedTransportable);
            _selectedTransportable = null;
        }
    }

}
