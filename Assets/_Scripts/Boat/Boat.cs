using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat
{
    [SerializeField]
    int _capacity = 1;
    int _occupied = 0;

    BoatBehaviour _behaviour;

    Island _currentIsland;
    List<Transportable> _seats = new List<Transportable>();

    public BoatBehaviour Behaviour { get { return _behaviour; } }
    public List<Transportable> Transportables { get { return _seats; } }
    public int Capacity { get { return _capacity; } }
    public int Occupied { get { return _occupied; } }
    public bool IsEmpty { get { return _occupied == 0; } }


    public Boat(int capacity)
    {
        _capacity = capacity;
    }

    public bool LoadBoat(Transportable newTransportable, bool instant = false)
    {
        if (_occupied >= _capacity || _currentIsland == null || !_currentIsland.CheckIfExists(newTransportable))
            return false;

        _currentIsland.Remove(newTransportable);

        int pos = -1;
        if (_seats.Count < _capacity)
        {
            _seats.Add(newTransportable);
            pos = _seats.Count - 1;
        }
        else
        {
            //Find an empty seat
            for (int i = 0; i < _capacity; i++)
            {
                if (_seats[i] == null)
                {
                    _seats[i] = newTransportable;
                    pos = i;
                    break;
                }
            }
        }

        _occupied++;

        if (_behaviour)
        {
            newTransportable.GoTo(_behaviour.GetSeat(pos), instant);
        }

        return true;
    }

    public void ForceLoad(Transportable newTransportable, int pos)
    {

        // 0  <  1
        // 1  ==  1
        while (pos < _seats.Count)
        {
            _seats.Add(null);
        }
        _seats[pos] = newTransportable;

        _occupied++;

        if (_behaviour)
        {
            newTransportable.GoTo(_behaviour.GetSeat(pos), true);
        }
    }

    internal void SetUp(GameObject g)
    {
        _behaviour = g.GetComponent<BoatBehaviour>();
        _behaviour.SetUp(this);
    }

    public void GoTo(Island newIsland, bool instant = false)
    {
        if (_behaviour)
            _behaviour.GoTo(newIsland, instant);

        _currentIsland = newIsland;
    }



    internal bool Contains(Transportable transportable)
    {
        return _seats.Contains(transportable);
    }

    public void UnloadBoat()
    {
        for (int i = 0; i < _capacity; i++)
        {
            if (_seats[i] != null)
                UnloadBoat(_seats[i]);
        }
    }

    public bool UnloadBoat(Transportable selectedTransportable, bool instant = false)
    {
        if (_currentIsland == null)
            return false;

        int i = _seats.IndexOf(selectedTransportable);
        if (i == -1)
        {
            return false;
        }
        _seats[i] = null;
        _currentIsland.Add(selectedTransportable, instant);
        _occupied--;
        return true;
    }

    public override string ToString()
    {
        string result = "< ";

        for (int i = 0; i < _capacity; i++)
        {
            result += "[ ";
            if (i < _seats.Count && _seats[i] != null)
                result += _seats[i];
            result += " ]";
        }

        result += " > (In island ";

        result += _currentIsland.Name + ")";

        return result;
    }

    public Island GetCurrentIsland()
    {
        return _currentIsland;
    }

    internal void Empty()
    {
        _occupied = 0;
    }



}
