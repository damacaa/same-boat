using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Island
{
    public static int _islandsCreated = 0;

    string _name = "";
    public string Name
    {
        get { return _name; }
    }

    List<Transportable> _transportables = new List<Transportable>();
    public List<Transportable> Transportables
    {
        get { return _transportables; }
    }

    public Vector2 Position
    {
        get
        {
            if (_behaviour)
                return _behaviour.transform.position;
            else
                return Vector2.zero;
        }
        internal set
        {
            if (_behaviour)
                _behaviour.transform.position = value;
        }
    }

    IslandBehaviour _behaviour;
    public IslandBehaviour Behaviour { get { return _behaviour; } }


    public Island()
    {
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        _name = "" + letters[_islandsCreated];
        _islandsCreated++;
    }

    public void AssignGameObject(GameObject g)
    {
        _behaviour = g.GetComponent<IslandBehaviour>();
        _behaviour.Assign(this);
    }

    internal Transform FindSpot(out int index)
    {
        index = -1;
        if (_behaviour)
            return _behaviour.FindSpot(out index);

        return null;
    }

    internal bool CheckIfExists(Transportable newTransportable)
    {
        return _transportables.Contains(newTransportable);
    }

    public void Add(Transportable data, out float animationDuration, bool instant = false)
    {
        Add(data, -1, out animationDuration, instant);
    }

    public void Add(Transportable data, int position, out float animationDuration, bool instant = false, bool backwards = false)
    {
        animationDuration = 0;

        data.Island = this;

        if (position != -1)
        {
            while (_transportables.Count < position - 1)
            {
                _transportables.Add(null);
            }
            _transportables[position] = data;
            data.PositionIndexInIsland = position;
        }
        else
        {
            int pos = _transportables.FindIndex(a => a == null);

            if (pos == -1)
            {
                _transportables.Add(data);
                data.PositionIndexInIsland = _transportables.Count - 1;
            }
            else
            {
                _transportables[pos] = data;
                data.PositionIndexInIsland = pos;
            }
        }

        if (_behaviour)
            data.GoTo(_behaviour.GetSpot(data.PositionIndexInIsland), out animationDuration, instant, backwards);
    }

    public void Remove(Transportable data)
    {
        _transportables[data.PositionIndexInIsland] = null;
    }

    public bool CheckFail()
    {
        throw new NotImplementedException();
    }

    internal bool IsEmpty()
    {
        return _transportables.Count(a => a != null) == 0;
    }
    internal void Enable()
    {
        if (_behaviour)
            _behaviour.GetComponent<Collider2D>().enabled = true;
    }

    internal void Disable()
    {
        if (_behaviour)
            _behaviour.GetComponent<Collider2D>().enabled = false;
    }


    public override string ToString()
    {
        string result = _name + ": [ ";
        for (int i = 0; i < _transportables.Count; i++)
        {
            Transportable t = _transportables[i];
            result += t == null ? "null " : t + " ";
        }
        result += "]";
        return result;
    }
}
