using System;
using System.Collections.Generic;
using UnityEngine;

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

    internal Transform FindSpot()
    {
        return _behaviour.FindSpot();
    }

    internal bool CheckIfExists(Transportable newTransportable)
    {
        return _transportables.Contains(newTransportable);
    }

    public void Add(Transportable data)
    {
        _transportables.Add(data);
        data.Island = this;
        if (_behaviour)
            data.GoTo(_behaviour.FindSpot());
    }

    public void Remove(Transportable data)
    {
        _transportables.Remove(data);
    }

    public bool CheckFail()
    {
        throw new NotImplementedException();
    }

    internal bool IsEmpty()
    {
        return _transportables.Count == 0;
    }
    internal void Enable()
    {
        if (_behaviour)
            _behaviour.GetComponent<Collider2D>().enabled = true;
    }

    internal void Disable()
    {
        _behaviour.GetComponent<Collider2D>().enabled = false;
    }


    public override string ToString()
    {
        string result = _name + ": [ ";
        foreach (var t in _transportables)
        {
            result += t + " ";
        }
        result += "]";
        return result;
    }
}
