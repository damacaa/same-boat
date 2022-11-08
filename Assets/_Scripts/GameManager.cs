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


    GameLogic game;

    Transportable _selectedTransportable;
    Boat _boat;

    private void Start()
    {
        game = new GameLogic();
        game.GenerateGameObjects();

        _boat = game.Boat;
        _boat.GoTo(game.FirstIsland, true);

        print(game);
    }

    private void Update()
    {
        if (Input.GetKeyDown("right"))
        {
            game.Execute();
            print(game);
        }

        if (Input.GetKeyDown("left"))
        {
            game.Undo();
            print(game);
            _selectedTransportable = null;
        }
    }

    public void IslandInteraction(Island island)
    {
        //print(island.Name);
        if (_boat.GetCurrentIsland() != island)
        {
            _selectedTransportable = null;
            game.MoveBoatToIsland(island);
        }
        else
        {
            if (_selectedTransportable != null)
            {
                game.UnloadFromBoat(_selectedTransportable);
                _selectedTransportable = null;
            }
        }
    }

    public void TransportableInteraction(Transportable transportable)
    {
        //print(transportable);
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
            game.MoveBoatToIsland(transportable.Island);

            //_boat.GetCurrentIsland() = transportable.Island;
            //game.LoadOnBoat(transportable);
        }
    }

    public void BoatInteraction(Boat boat)
    {
        //print("Boat");
        _boat = boat;
        if (_selectedTransportable != null)
        {
            print("Load " + _selectedTransportable);
            game.LoadOnBoat(_selectedTransportable);
            _selectedTransportable = null;
        }
    }
}
