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
    bool _fail = false;

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

        //Start game
        //Example();
    }

    void Example()
    {
        Transportable fox = _islands[0].Transportables[0];
        Transportable chicken = _islands[0].Transportables[1];
        Transportable corn = _islands[0].Transportables[2];

        Island a = _islands[0];
        Island b = _islands[1];

        LoadOnBoat(chicken);
        MoveBoatToIsland(b);
        UnloadFromBoat(chicken);
        MoveBoatToIsland(a);
        LoadOnBoat(fox);
        MoveBoatToIsland(b);
        UnloadFromBoat(fox);
        LoadOnBoat(chicken);
        MoveBoatToIsland(a);
        UnloadFromBoat(chicken);
        LoadOnBoat(corn);
        MoveBoatToIsland(b);
        UnloadFromBoat(corn);
        MoveBoatToIsland(a);
        LoadOnBoat(chicken);
        MoveBoatToIsland(b);
        UnloadFromBoat(chicken);
    }

    void Execute(int n)
    {
        for (int i = 0; i < 2; i++)
        {
            Execute();
        }
    }

    public void GenerateGameObjects()
    {
        //Build map
        MapGenerator.instace.GenerateMap(_islands.ToArray()); //Should generate island behaviours
        MapGenerator.instace.GenerateBoat(_boat);
        TransportableManager.instace.GenerateSprites(_islands.ToArray());
    }

    public void Execute()
    {
        _fail = CheckFail();

        if (!_fail && _currentCommand < _commands.Count)
        {
            _commands[_currentCommand].Execute();
            Debug.Log(_commands[_currentCommand]);

            _currentCommand++;

            CheckWin();
        }
    }

    bool _undone = false;

    public void Undo()
    {
        if (_currentCommand > 0)
        {
            _undone = true;
            _currentCommand--;
            _commands[_currentCommand].Undo();
            _fail = false;
        }
    }

    bool CheckFail()
    {
        bool fail = false;
        foreach (var island in _islands)
        {
            fail = fail || Solver.Solver.CheckFail(island.Transportables);
            if (fail)
                Debug.Log("Fail in island " + island.Name);
        }

        if (Solver.Solver.CheckFail(_boat.Transportables))
            Debug.Log("Fail in boat");

        return fail;
    }

    void CheckWin()
    {
        bool win = true;
        foreach (var island in _islands)
        {
            if (island != _islands[_islands.Count - 1]) //Last island
            {
                win = win && island.IsEmpty();
            }
        }

        if (win)
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

    public void LoadOnBoat(Transportable load)
    {
        AddCommand(new BoatLoadCommand(_boat, load));
    }

    public void UnloadFromBoat(Transportable load)
    {
        AddCommand(new BoatUnloadCommand(_boat, load));
    }

    public void MoveBoatToIsland(Island island)
    {
        AddCommand(new BoatTravelCommand(_boat, island));
    }

    void AddCommand(Command command)
    {
        if (_undone && _commands.Count > _currentCommand)
            _commands.RemoveRange(_currentCommand, _commands.Count - _currentCommand);
        _undone = false;

        _commands.Add(command);

        Execute();
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