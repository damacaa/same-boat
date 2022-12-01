using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boat
{
    BoatBehaviour _behaviour;
    List<Transportable> _seats = new List<Transportable>();

    public BoatBehaviour Behaviour { get { return _behaviour; } }
    public List<Transportable> Transportables { get { return _seats; } }

    private int _crossings;
    public int Crossings
    {
        get { return _crossings; }
        private set
        {
            _crossings = value;
            if (OnCrossingsChange != null)
                OnCrossingsChange(value);
        }
    }
    public int Capacity { get; private set; }
    public int Occupied { get; private set; }
    public int MaxWeight { get; private set; }
    public int CurrentWeight { get; private set; }
    public int MaxTravelCost { get; private set; }
    public int CurrentTravelCost { get; private set; }
    public bool IsEmpty { get { return Occupied == 0; } }
    public bool OnlyHumansCanDrive { get; private set; }
    public Island Island { get; private set; }

    public delegate void OnCrossingsChangeDelegate(int newValue);
    public event OnCrossingsChangeDelegate OnCrossingsChange;

    public Boat(Island island, int capacity, int maxWeight, int maxTravelCost, bool onlyHumansCanDrive)
    {
        Island = island;

        Capacity = capacity;
        MaxWeight = maxWeight;
        MaxTravelCost = maxTravelCost;
        OnlyHumansCanDrive = onlyHumansCanDrive;

        CurrentTravelCost = 0;
        Crossings = 0;
    }

    public bool LoadBoat(Transportable newTransportable, out int position, out float animationDuration, bool instant = false, bool backwards = false)
    {
        position = -1;
        animationDuration = 0;
        if (Occupied >= Capacity
            || (MaxWeight != 0 && (CurrentWeight + newTransportable.Weight) > MaxWeight)
            || Island == null
            || !Island.Contains(newTransportable))
        {
            //Debug.Log("Fuck");
            return false;
        }

        //Returns position where it was before being loaded on boat
        position = newTransportable.PositionIndexInIsland;

        Island.Remove(newTransportable);

        int pos = -1;
        if (_seats.Count < Capacity)
        {
            _seats.Add(newTransportable);
            pos = _seats.Count - 1;
        }
        else
        {
            //Find an empty seat
            pos = _seats.FindIndex(a => a == null);
            _seats[pos] = newTransportable;
        }

        Occupied++;
        CurrentWeight += newTransportable.Weight;

        if (_behaviour)
        {
            newTransportable.GoTo(_behaviour.GetSeat(pos), out animationDuration, instant, backwards);
        }

        return true;
    }

    internal void SetUp(GameObject g)
    {
        g.transform.position = Island.Behaviour.GetPortPosition();
        _behaviour = g.GetComponent<BoatBehaviour>();
        _behaviour.SetUp(this);
    }

    public bool GoTo(Island newIsland, out float animationDuration, bool instant = false, bool backwards = false)
    {
        animationDuration = 0;

        int travelCost = 0;
        foreach (var t in _seats)
        {
            if (t == null)
                continue;

            //travelCost += t.ScripatableObject.TravelCost;
            travelCost = Math.Max(travelCost, t.ScripatableObject.TravelCost);
        }

        if (_seats.Count(t => t != null) <= 0 || (OnlyHumansCanDrive && _seats.Count(t => t != null && t.ScripatableObject.name == "Man") <= 0) || (!backwards && MaxTravelCost > 0 && (CurrentTravelCost + travelCost) > MaxTravelCost))
        {
            return false;
        }

        CurrentTravelCost += backwards ? -travelCost : travelCost;
        Crossings += backwards ? -1 : 1;

        if (_behaviour)
            _behaviour.GoTo(newIsland, out animationDuration, instant, backwards);

        Island = newIsland;

        return true;
    }

    internal bool Contains(Transportable transportable)
    {
        return _seats.Contains(transportable);
    }

    public bool UnloadBoat(Transportable selectedTransportable, int position, out float animationDuration, bool instant = false, bool backwards = false)
    {
        animationDuration = 0;
        if (Island == null)
            return false;

        int i = _seats.FindIndex(a => a == selectedTransportable);
        if (i == -1)
        {
            return false;
        }
        _seats[i] = null;
        Island.Add(selectedTransportable, position, out animationDuration, instant, backwards);
        Occupied--;
        CurrentWeight -= selectedTransportable.Weight;
        return true;
    }

    public override string ToString()
    {
        string result = "< ";

        for (int i = 0; i < Capacity; i++)
        {
            result += "[ ";
            if (i < _seats.Count && _seats[i] != null)
                result += _seats[i];
            result += " ]";
        }

        result += " > (In island ";

        result += Island.Name + ")";

        return result;
    }
}
