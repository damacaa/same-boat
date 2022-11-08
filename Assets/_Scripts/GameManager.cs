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
    Island _selectedIsland;
    Boat _boat;

    private void Start()
    {
        game = new GameLogic();
        game.GenerateGameObjects();

        _selectedIsland = game.FirstIsland;
        _boat = game.Boat;

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
        }
    }

    public void IslandInteraction(Island island)
    {
        print(island.Name);
        if (_selectedIsland != island)
        {
            _selectedTransportable = null;
            _selectedIsland = island;
            game.MoveBoatToIsland(_selectedIsland);
            print("Go to " + _selectedIsland.Name);
        }
        else
        {
            if (_selectedTransportable != null)
            {
                print("Unload " + _selectedTransportable);
                game.UnloadFromBoat(_selectedTransportable);
                _selectedTransportable = null;
            }
        }
    }

    void SelectIsland(Island island)
    {
        //Doesn't work properly
        if (_selectedIsland != null)
            DeselectIsland(_selectedIsland);
        _selectedIsland = island;
        _selectedIsland.Disable();
    }

    void DeselectIsland(Island island)
    {
        _selectedIsland = null;
        island.Enable();
    }

    public void TransportableInteraction(Transportable transportable)
    {
        print(transportable);
        if (_selectedIsland != null && _selectedIsland.CheckIfExists(transportable))
        {
            _selectedTransportable = transportable;
            print("Select " + _selectedTransportable);
        }
        else if (_boat.Contains(transportable))
        {
            _selectedTransportable = transportable;
            print("Select " + _selectedTransportable);
        }
    }

    public void BoatInteraction(Boat boat)
    {
        print("Boat");
        _boat = boat;
        if (_selectedTransportable != null)
        {
            print("Load " + _selectedTransportable);
            game.LoadOnBoat(_selectedTransportable);
            _selectedTransportable = null;
        }
    }
}
