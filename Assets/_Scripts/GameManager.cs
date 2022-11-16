using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
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

    GameLogic _game;
    Transportable _selectedTransportable;
    Boat _boat;

    private void Start()
    {
        LoadLevel(levels[_currentLevel]);
    }

    public void LoadLevel(Level level)
    {
        _game = new GameLogic(level);
        _game.GenerateGameObjects();
        _boat = _game.Boat;
        _boat.GoTo(_game.FirstIsland, out float animationDuration, true);
    }

    private void Update()
    {
        if (_game == null)
            return;

        if (Input.GetKeyDown("right"))
        {
            _game.Execute();
            print(_game);
        }

        if (Input.GetKeyDown("left"))
        {
            _game.Undo();
            print(_game);
            _selectedTransportable = null;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            GameLogic g = new GameLogic(_game);
            //g.MoveBoatToIsland(g.FirstIsland);
            //var s = game.GetCurrentState();
            int steps = Solver.Solver.Solve(_game);
            print(steps);

            StartCoroutine(_game.ShowAllMovesCoroutine());
        }
    }

    public void IslandInteraction(Island island)
    {
        //print(island.Name);
        if (_boat.GetCurrentIsland() != island)
        {
            _selectedTransportable = null;
            _game.MoveBoatToIsland(island);
        }
        else
        {
            if (_selectedTransportable != null && _boat.Contains(_selectedTransportable))
            {
                _game.UnloadFromBoat(_selectedTransportable);
                _selectedTransportable = null;
            }
        }
    }

    public void TransportableInteraction(Transportable transportable)
    {
        print(transportable);
        if (_boat.GetCurrentIsland().CheckIfExists(transportable))
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
            _game.MoveBoatToIsland(transportable.Island);
            //_boat.GetCurrentIsland() = transportable.Island;
            //game.LoadOnBoat(transportable);
        }
    }

    public void BoatInteraction(Boat boat)
    {
        _boat = boat;
        if (_selectedTransportable != null)
        {
            print("Load " + _selectedTransportable);
            _game.LoadOnBoat(_selectedTransportable);
            _selectedTransportable = null;
        }
    }

}
