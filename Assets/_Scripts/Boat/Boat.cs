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

    public bool LoadBoat(Transportable newTransportable, out int position, out float animationDuration, bool instant = false, bool backwards = false)
    {
        position = -1;
        animationDuration = 0;
        if (_occupied >= _capacity || _currentIsland == null || !_currentIsland.Contains(newTransportable))
            return false;

        //Returns position where it was before being loaded on boat
        position = newTransportable.PositionIndexInIsland;

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
            pos = _seats.FindIndex(a => a == null);
            _seats[pos] = newTransportable;
        }

        _occupied++;

        if (_behaviour)
        {
            newTransportable.GoTo(_behaviour.GetSeat(pos), out animationDuration, instant, backwards);
        }

        return true;
    }

    internal void SetUp(GameObject g)
    {
        _behaviour = g.GetComponent<BoatBehaviour>();
        _behaviour.SetUp(this);
    }

    public void GoTo(Island newIsland, out float animationDuration, bool instant = false)
    {
        animationDuration = 0;

        if (_behaviour)
            _behaviour.GoTo(newIsland, out animationDuration, instant);

        _currentIsland = newIsland;
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
        _occupied--;
        return true;
    }

    public Island GetCurrentIsland()
    {
        return _currentIsland;
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
}
