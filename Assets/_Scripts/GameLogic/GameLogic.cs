using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;


public class GameLogic
{
    private Level _level;

    //Entities
    private Boat _boat;
    private List<Island> _islands = new List<Island>();

    //Command
    private int _currentCommand = 0;
    private List<Command> _commands = new List<Command>();

    //Control
    private bool _win = false;
    private bool _fail = false;
    private bool _undone = false;

    private bool _showingSolveAnimation = false;
    private float _nextTime = 0;

    private bool _strictMode = false;

    public bool Win { get { return _win; } }
    public bool Fail { get { return _fail; } }

    public Boat Boat { get { return _boat; } }

    // Events
    public event Action OnFail;
    public event Action OnWin;


    public GameLogic(Level level)
    {
        _level = level;

        _strictMode = level.StrictMode;

        //Desearialize transportables
        foreach (var island in level.Islands)
        {
            Island islandData = new Island();

            foreach (var transportableSO in island.transportables)
            {
                var t = new Transportable(transportableSO);

                if (level.StrictMode)
                {
                    foreach (var rule in level.Rules)
                    {

                        switch (rule.comparison)
                        {
                            case Rule.RuleType.CantCoexist:
                                if (t.ScripatableObject == rule.A)
                                    t.IsHungry = true;
                                break;
                            case Rule.RuleType.CountMustBeGreaterThan:
                                if (t.ScripatableObject == rule.B)
                                    t.IsHungry = true;
                                break;
                            case Rule.RuleType.CountMustBeGreaterEqualThan:
                                if (t.ScripatableObject == rule.B)
                                    t.IsHungry = true;
                                break;
                            case Rule.RuleType.Requires:
                                break;
                            default:
                                break;
                        }


                    }
                }

                islandData.Add(t, out float animationDuration, true);
            }

            _islands.Add(islandData);
        }

        //Boat
        _boat = new Boat(_islands[0], level.BoatCapacity, level.BoatMaxWeightAllowed, level.BoatMaxTravelCost, level.OnlyHumansCanDrive);
    }



    public void GenerateGameObjects(Level level)
    {
        //Build map
        MapGenerator.instace.GenerateMap(_islands.ToArray(), level); //Should generate island behaviours
        MapGenerator.instace.GenerateBoat(_boat);
        TransportableManager.instace.GenerateSprites(_islands.ToArray(), level);
    }

    #region CommandExecution
    public bool Execute(bool skipAnimation = false)
    {
        /*if (!skipAnimation && (_showingSolveAnimation || (_waitForEnd && Time.time < _nextTime)))
        {
            //skipAnimation = true;//Should make current animation skipeable
            return false;
        }*/

        if (!_fail && _currentCommand < _commands.Count)
        {
            bool success = _commands[_currentCommand].Execute(out float animationDuration, skipAnimation);


            //If command fails
            if (!success)
            {
                _commands.RemoveAt(_currentCommand);
                return false;
            }

            if (!skipAnimation)
                _nextTime = Time.time + (0.5f * animationDuration);

            //Debug.Log(_commands[_currentCommand]);

            _fail = CheckFail(!skipAnimation);
            if (!skipAnimation && _fail && OnFail != null)
                OnFail();

            if (!_fail)
            {
                CheckWin();
                if (!skipAnimation && Win && OnWin != null)
                    OnWin();
            }

            return _commands[_currentCommand++].Success;
        }

        return false;
    }

    public void Undo(bool skipAnimation = false)
    {
        /*if (!skipAnimation && (_showingSolveAnimation || (_waitForEnd && Time.time < _nextTime)))
        {
            //skipAnimation = true;
            return;
        }*/

        if (_currentCommand > 0 && _commands.Count > 0)
        {
            _undone = true;
            _currentCommand--;
            _commands[_currentCommand].Undo(out float animationDuration, skipAnimation);
            if (!skipAnimation)
                _nextTime = Time.time + (0.5f * animationDuration);
            _fail = false;
            _win = false;
            //Debug.Log("Undone: " + _commands[_currentCommand]);
        }
    }

    public void Undo(out float animationDuration)
    {
        animationDuration = 0;

        if (_currentCommand > 0 && _commands.Count > 0)
        {
            _undone = true;
            _currentCommand--;
            _commands[_currentCommand].Undo(out animationDuration, false);

            _nextTime = Time.time + (0.5f * animationDuration);
            _fail = false;
            _win = false;
        }
    }

    internal void Reset(bool skipAnimation = false)
    {
        for (int i = 0; i <= _commands.Count; i++) //Have to figure out how many undos are needed
        {
            Undo(skipAnimation);
        }
        _commands.Clear();
        _currentCommand = 0;
    }

    public IEnumerator ExecuteAllMovesCoroutine()
    {
        var wait = new WaitForSeconds(.5f);
        while (_showingSolveAnimation)
        {
            yield return wait;
        }

        _showingSolveAnimation = true;

        while (_currentCommand < _commands.Count && _showingSolveAnimation)
        {
            _commands[_currentCommand].Execute(out float waitTime);
            _currentCommand++;
            yield return new WaitForSeconds(1.1f * waitTime);
        }

        CheckWin();
        if (Win && OnWin != null)
            OnWin();
        _showingSolveAnimation = false;

        yield return null;
    }

    public void CancelMoveCOrorutine()
    {
        _showingSolveAnimation = false;
    }

    #endregion

    #region Checks
    bool CheckFail(bool showMessage = true)
    {
        bool fail = false;
        foreach (var island in _islands)
        {
            if (!_strictMode && island == _boat.Island)
                continue;

            List<Transportable> group = island.Transportables.Where(item => item != null).Select(item => item).ToList();


            if (_strictMode && island == _boat.Island)
            {
                group.AddRange(_boat.Transportables.Where(item => item != null));
            }

            /*var hungryTransportablesInBoat = _boat.Transportables.Where(item => item != null && item.IsHungry);
            group.AddRange(hungryTransportablesInBoat);*/

            if (!CheckRules(group))
            {
                fail = true;
                if (showMessage)
                    Debug.Log("Fail in island " + island.Name);
            }
        }

        /*if (_checkBoatForMistakes && !CheckRules(_boat.Transportables))
        {
            fail = true;
            if (showMessage)
                Debug.Log("Fail in boat.");
        }*

        /*if (Solver.Solver.CheckFail(_boat.Transportables))
            Debug.Log("Fail in boat");*/

        return fail;
    }

    public bool CheckRules(List<Transportable> transportables)
    {
        // Could be optimized by counting each only once, using a dictionary maybe

        foreach (var r in _level.Rules)
        {
            int aCount = 0, bCount = 0;

            // Count how many transportables of each type in rule
            foreach (var t in transportables)
            {
                if (t == null)
                    continue;

                if (t.ScripatableObject == r.A)
                    aCount++;
                else if (t.ScripatableObject == r.B)
                    bCount++;
            }

            switch (r.comparison)
            {
                case Rule.RuleType.CantCoexist:
                    if (aCount > 0 && bCount > 0)
                        return false;
                    break;
                case Rule.RuleType.CountMustBeGreaterThan:
                    if (aCount > 0 && aCount <= bCount)
                        return false;
                    break;
                case Rule.RuleType.CountMustBeGreaterEqualThan:
                    if (aCount > 0 && aCount < bCount)
                        return false;
                    break;
                case Rule.RuleType.Requires:
                    if (aCount > 0 && aCount > bCount)
                        return false;
                    break;
                default:
                    Debug.Log("Rule not implemented");
                    break;
            }
        }

        return true;
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
    }
    #endregion

    #region Commands
    public bool LoadOnBoat(Transportable load, bool skipAnimation = false)
    {
        return AddCommand(new BoatLoadCommand(_boat, load), skipAnimation);
    }

    public bool UnloadFromBoat(Transportable load, bool skipAnimation = false)
    {
        return AddCommand(new BoatUnloadCommand(_boat, load), skipAnimation);
    }

    public bool MoveBoatToIsland(Island island, bool skipAnimation = false)
    {
        return AddCommand(new BoatTravelCommand(_boat, island), skipAnimation);
    }

    public bool AddCommand(Command command, bool skipAnimation = false)
    {
        if (_undone && _commands.Count > _currentCommand)
            _commands.RemoveRange(_currentCommand, _commands.Count - _currentCommand);
        _undone = false;

        _commands.Add(command);

        return Execute(skipAnimation);
    }
    #endregion

    public Solver.State GetCurrentState()
    {
        Solver.State currentState = new Solver.State();
        foreach (var island in _islands)
        {
            currentState.AddIsland(island);
        }
        currentState.CurrentIsland = _boat.Island;

        currentState.BoatTransportables = _boat.Transportables.FindAll(t => t != null).OrderBy(t => t.ToString()).ToArray();
        currentState.BoatCapacity = _boat.Capacity;
        currentState.BoatOccupiedSeats = _boat.Occupied;
        currentState.BoatMaxWeight = _boat.MaxWeight;
        currentState.BoatCurrentWeight = _boat.CurrentWeight;
        currentState.BoatMaxTravelCost = _boat.MaxTravelCost;
        currentState.BoatTravelCost = _boat.CurrentTravelCost;

        currentState.Crossings = _boat.Crossings;

        if (_commands.Count > 0 && _currentCommand > 0 && _currentCommand - 1 < _commands.Count)
            currentState.Command = _commands[_currentCommand - 1];

        return currentState;
    }



    public override string ToString()
    {
        return GetCurrentState().ToString();
    }
}

