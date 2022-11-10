using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameLogic
{
    //Entities
    Player _player;
    Boat _boat;
    List<Island> _islands = new List<Island>();

    //Command
    int _currentCommand = 0;
    List<Command> _commands = new List<Command>();

    //Control
    bool _win = false;
    bool _fail = false;
    bool _undone = false;

    public bool Win { get { return _win; } }
    public bool Fail { get { return _fail; } }

    public Boat Boat { get { return _boat; } }
    public Island FirstIsland { get { return _islands[0]; } }

    public GameLogic()
    {
        //Read scene from file
        SerializedInfo.Level serializedLevel = new SerializedInfo.Level();
        serializedLevel.islands = new SerializedInfo.Island[2];
        serializedLevel.islands[0] = new SerializedInfo.Island();
        serializedLevel.islands[1] = new SerializedInfo.Island();
        serializedLevel.islands[0].transportables = new string[] { "fox", "chicken", "corn" };
        serializedLevel.islands[1].transportables = new string[] { };

        //Desearialize transportables
        foreach (var serializedIsland in serializedLevel.islands)
        {
            Island islandData = new Island();

            foreach (var transportableKey in serializedIsland.transportables)
            {
                islandData.Add(new Transportable(transportableKey));
            }

            _islands.Add(islandData);
        }

        //Boat
        _boat = new Boat(serializedLevel.boatCapacity);
    }

    public void GenerateGameObjects()
    {
        //Build map
        MapGenerator.instace.GenerateMap(_islands.ToArray()); //Should generate island behaviours
        MapGenerator.instace.GenerateBoat(_boat);
        TransportableManager.instace.GenerateSprites(_islands.ToArray());
    }

    public bool Execute()
    {
        if (!_fail && _currentCommand < _commands.Count)
        {
            _commands[_currentCommand].Execute();
            Debug.Log(_commands[_currentCommand]);


            _fail = CheckFail();


            if (!_fail)
                CheckWin();

            return _commands[_currentCommand++].Success;
        }

        return false;
    }


    public void Undo()
    {
        if (_currentCommand > 0)
        {
            _undone = true;
            _currentCommand--;
            _commands[_currentCommand].Undo();
            _fail = false;
            Debug.Log("Undone: " + _commands[_currentCommand]);
        }
    }

    bool CheckFail()
    {
        bool fail = false;
        foreach (var island in _islands)
        {
            if (island == _boat.GetCurrentIsland())
                continue;

            if (Solver.Solver.CheckFail(island.Transportables))
            {
                fail = true;
                Debug.Log("Fail in island " + island.Name);
            }
        }

        if (Solver.Solver.CheckFail(_boat.Transportables))
            Debug.Log("Fail in boat");

        return fail;
    }

    void CheckWin()
    {
        _win = true;
        foreach (var island in _islands)
        {
            if (island != _islands[_islands.Count - 1]) //Last island
            {
                _win = _win && island.IsEmpty();
            }
        }

        _win = win && _boat.IsEmpty();

        if (_win)
            Debug.Log("WIN!");
    }

    public override string ToString()
    {
        string result = "";

        string progress = "";
        for (int i = 0; i <= _commands.Count; i++)
        {
            if (i == _currentCommand)
            {
                progress += "|";
            }
            else
            {
                progress += "-";
            }
        }

        result += progress + " " + _currentCommand + "/" + _commands.Count + "\n";

        foreach (var island in _islands)
        {
            result += island + "    ";
        }

        result += _boat + "\n";

        return result;
    }

    public bool LoadOnBoat(Transportable load)
    {
        return AddCommand(new BoatLoadCommand(_boat, load));
    }

    public bool UnloadFromBoat(Transportable load)
    {
        return AddCommand(new BoatUnloadCommand(_boat, load));
    }

    public bool MoveBoatToIsland(Island island)
    {
        return AddCommand(new BoatTravelCommand(_boat, island));
    }

    bool AddCommand(Command command)
    {
        if (_undone && _commands.Count > _currentCommand)
            _commands.RemoveRange(_currentCommand, _commands.Count - _currentCommand);
        _undone = false;

        _commands.Add(command);

        return Execute();
    }

    public Solver.State GetCurrentState()
    {
        Solver.State currentState = new Solver.State();
        foreach (var island in _islands)
        {
            currentState.AddIsland(island);
        }
        currentState.currentIsland = _boat.GetCurrentIsland();

        currentState.boat = _boat.Transportables;
        currentState.boatCapacity = _boat.Capacity;
        currentState.boatOccupiedSeats = _boat.Occupied;

        return currentState;
    }

    public void SetState(Solver.State state)
    {

        foreach (var transportable in state.boat)
        {
            transportable.Teleport(_boat);
        }

        List<Transportable> transportablesToMove = new List<Transportable>();
        List<Island> islandToMoveTo = new List<Island>();

        foreach (var island in state.islands)
        {
            foreach (var transportable in island.transportables)
            {
                transportable.Teleport(island.islandRef);
                //transportablesToMove.Add(transportable);
                //islandToMoveTo.Add(island);
            }
        }

        for (int i = 0; i < transportablesToMove.Count; i++)
        {
            transportablesToMove[i].Teleport(islandToMoveTo[i]);
        }

        //_boat.GoTo(state.currentIsland, true);
    }
}

namespace SerializedInfo
{
    [System.Serializable]
    public class Level
    {
        public Island[] islands;
        public int boatCapacity = 1;
    }

    [System.Serializable]
    public class Island
    {
        public string[] transportables;
    }
}