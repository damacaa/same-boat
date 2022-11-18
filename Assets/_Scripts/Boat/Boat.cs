using System.Collections.Generic;
using UnityEngine;

public class Boat
{
    BoatBehaviour _behaviour;

    Island _currentIsland;
    List<Transportable> _seats = new List<Transportable>();

    public BoatBehaviour Behaviour { get { return _behaviour; } }
    public List<Transportable> Transportables { get { return _seats; } }
    public int Capacity { get; private set; }
    public int Occupied { get; private set; }
    public int MaxWeight { get; private set; }
    public int CurrentWeight { get; private set; }
    public bool IsEmpty { get { return Occupied == 0; } }
    public bool CanMoveEmpty { get;private set; }


    public Boat(Island island, int capacity, int maxWeight,bool canMoveEmpty)
    {
        _currentIsland = island;

        Capacity = capacity;
        MaxWeight = maxWeight;
        CanMoveEmpty = canMoveEmpty;
    }

    public bool LoadBoat(Transportable newTransportable, out int position, out float animationDuration, bool instant = false, bool backwards = false)
    {
        position = -1;
        animationDuration = 0;
        if (Occupied >= Capacity
            || (CurrentWeight + newTransportable.Weight) > MaxWeight
            || _currentIsland == null
            || !_currentIsland.Contains(newTransportable)) {
            Debug.Log("Fuck");
            return false;
        }

        //Returns position where it was before being loaded on boat
        position = newTransportable.PositionIndexInIsland;

        _currentIsland.Remove(newTransportable);

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
        g.transform.position = _currentIsland.Behaviour.GetPortPosition();
        _behaviour = g.GetComponent<BoatBehaviour>();
        _behaviour.SetUp(this);
    }

    public bool GoTo(Island newIsland, out float animationDuration, bool instant = false)
    {
        animationDuration = 0;

        if (!CanMoveEmpty && Occupied == 0) {
            return false;
        }

        if (_behaviour)
            _behaviour.GoTo(newIsland, out animationDuration, instant);

        _currentIsland = newIsland;

        return true;
    }

    internal bool Contains(Transportable transportable)
    {
        return _seats.Contains(transportable);
    }

    public bool UnloadBoat(Transportable selectedTransportable, int position, out float animationDuration, bool instant = false, bool backwards = false)
    {
        animationDuration = 0;
        if (_currentIsland == null)
            return false;

        int i = _seats.FindIndex(a => a == selectedTransportable);
        if (i == -1)
        {
            return false;
        }
        _seats[i] = null;
        _currentIsland.Add(selectedTransportable, position, out animationDuration, instant, backwards);
        Occupied--;
        CurrentWeight -= selectedTransportable.Weight;
        return true;
    }

    public Island GetCurrentIsland()
    {
        return _currentIsland;
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

        result += _currentIsland.Name + ")";

        return result;
    }
}
