using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat
{
    [SerializeField]
    int _capacity = 1;
    int _occupied = 0;

    List<Transportable> _seats = new List<Transportable>();
    Island _currentIsland;

    BoatBehaviour _behaviour;

    public List<Transportable> Transportables
    {
        get { return _seats; }
    }

    public int Capacity { get { return _capacity; } }


    public Boat(int capacity)
    {
        _capacity = capacity;
    }

    public bool LoadBoat(Transportable newTransportable)
    {
        if (_occupied >= _capacity || !_currentIsland.CheckIfExists(newTransportable))
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
            Debug.Log("Boat loaded");
            newTransportable.GoTo(_behaviour.GetSeat(pos));
        }

        return true;
    }

    internal void SetUp(GameObject g)
    {
        _behaviour = g.GetComponent<BoatBehaviour>();
        _behaviour.SetUp(this);
    }

    public void GoTo(Island newIsland)
    {
        if (_currentIsland == newIsland)
            return;

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

    public bool UnloadBoat(Transportable selectedTransportable)
    {

        int i = _seats.IndexOf(selectedTransportable);
        if (i == -1)
        {
            return false;
        }
        _seats[i] = null;
        _currentIsland.Add(selectedTransportable);
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
}
