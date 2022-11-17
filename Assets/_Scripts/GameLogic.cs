using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameLogic
{
    //Entities
    Player _player;
    Boat _boat;
    public List<Island> _islands = new List<Island>();

    //Command
    int _currentCommand = 0;
    List<Command> _commands = new List<Command>();

    //Control
    bool _win = false;
    bool _fail = false;
    bool _undone = false;
    private bool _waitForEnd = true;
    private bool _showingSolveAnimation = false;
    float _nextTime = 0;

    public bool Win { get { return _win; } }
    public bool Fail { get { return _fail; } }

    public Boat Boat { get { return _boat; } }
    public Island FirstIsland { get { return _islands[0]; } }

    public GameLogic(Level level)
    {
        //Desearialize transportables
        foreach (var island in level.Islands)
        {
            Island islandData = new Island();

            foreach (var transportableSO in island.transportables)
            {
                islandData.Add(new Transportable(transportableSO), out float animationDuration, true);
            }

            _islands.Add(islandData);
        }

        //Boat
        _boat = new Boat(level.BoatCapacity);
    }

    public GameLogic(GameLogic original)
    {
        _boat = new Boat(original._boat.Capacity);
        for (int i = 0; i < original._boat.Transportables.Count; i++)
        {
            if (original._boat.Transportables[i] != null)
                _boat.ForceLoad(new Transportable(original._boat.Transportables[i]), i);
        }

        foreach (var island in original._islands)
        {
            Island islandData = new Island();

            foreach (var transportable in island.Transportables)
            {
                islandData.Add(new Transportable(transportable), out float animationDuration);
            }

            if (island == original._boat.GetCurrentIsland())
            {
                _boat.GoTo(islandData, out float animationDuration);
            }

            _islands.Add(islandData);
        }
    }

    internal void Reset()
    {
        for (int i = 0; i <= _commands.Count; i++)
        {
            Undo(true);
        }
        //_commands.Clear();
        _currentCommand = 0;
    }

    public void GenerateGameObjects()
    {
        //Build map
        MapGenerator.instace.GenerateMap(_islands.ToArray()); //Should generate island behaviours
        MapGenerator.instace.GenerateBoat(_boat);
        TransportableManager.instace.GenerateSprites(_islands.ToArray());
    }

    public bool Execute(bool instant = false)
    {
        if (!instant && (_showingSolveAnimation || (_waitForEnd && Time.time < _nextTime)))
        {
            //instant = true;//Should make current animation skipeable
            return false;
        }

        if (!_fail && _currentCommand < _commands.Count)
        {
            bool success = _commands[_currentCommand].Execute(out float animationDuration, instant);


            //If command fails
            if (!success)
            {
                _commands.RemoveAt(_currentCommand);
                return false;
            }

            if (!instant)
                _nextTime = Time.time + (0.5f * animationDuration);

            //Debug.Log(_commands[_currentCommand]);

            _fail = CheckFail();

            if (!_fail)
            {
                CheckWin();
            }

            return _commands[_currentCommand++].Success;
        }

        return false;
    }

    public void Undo(bool instant = false)
    {
        if (!instant && (_showingSolveAnimation || (_waitForEnd && Time.time < _nextTime)))
        {
            //instant = true;
            return;
        }

        if (_currentCommand > 0 && _commands.Count > 0)
        {
            _undone = true;
            _currentCommand--;
            _commands[_currentCommand].Undo(out float animationDuration, instant);
            if (!instant)
                _nextTime = Time.time + (0.5f * animationDuration);
            _fail = false;
            //Debug.Log("Undone: " + _commands[_currentCommand]);
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

        /*if (Solver.Solver.CheckFail(_boat.Transportables))
            Debug.Log("Fail in boat");*/

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

        _win = _win && _boat.IsEmpty;

        if (_win)
            Debug.Log("WIN!");
    }



    public bool LoadOnBoat(Transportable load, bool instant = false)
    {
        return AddCommand(new BoatLoadCommand(_boat, load), instant);
    }

    public bool UnloadFromBoat(Transportable load, bool instant = false)
    {
        return AddCommand(new BoatUnloadCommand(_boat, load), instant);
    }

    public bool MoveBoatToIsland(Island island, bool instant = false)
    {
        return AddCommand(new BoatTravelCommand(_boat, island), instant);
    }

    public bool AddCommand(Command command, bool instant = false)
    {
        if (_undone && _commands.Count > _currentCommand)
            _commands.RemoveRange(_currentCommand, _commands.Count - _currentCommand);
        _undone = false;

        _commands.Add(command);

        return Execute(instant);
    }

    public Solver.State GetCurrentState()
    {
        Solver.State currentState = new Solver.State();
        foreach (var island in _islands)
        {
            currentState.AddIsland(island);
        }
        currentState.currentIsland = _boat.GetCurrentIsland();

        currentState.boatTransportables = _boat.Transportables.FindAll(t => t != null).OrderBy(t => t.ToString()).ToArray();
        currentState.boatCapacity = _boat.Capacity;
        currentState.boatOccupiedSeats = _boat.Occupied;

        if (_commands.Count > 0 && _currentCommand > 0 && _currentCommand - 1 < _commands.Count)
            currentState.command = _commands[_currentCommand - 1];

        return currentState;
    }

    public IEnumerator ShowAllMovesCoroutine()
    {
        _showingSolveAnimation = true;

        while (_currentCommand < _commands.Count)
        {
            _commands[_currentCommand].Execute(out float wait);
            _currentCommand++;
            yield return new WaitForSeconds(1.1f * wait);
        }

        _showingSolveAnimation = false;

        yield return null;
    }

    public override string ToString()
    {
        string result = "";

        string progress = "";
        for (int i = 0; i < _commands.Count; i++)
        {
            if (i == _currentCommand)
            {
                progress += "|";
            }
            else
            {
                progress += _commands[i].Success ? "-" : "x";
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
}

