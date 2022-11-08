using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transportable
{
    TransportableBehaviour _behaviour;
    public TransportableBehaviour Behaviour { get { return _behaviour; } }

    TransportableSO _scripatableObject;

    Island _currentIsland;
    public Island Island { get { return _currentIsland; } set { _currentIsland = value; } }

    public Transportable(string key)
    {
        _scripatableObject = TransportableManager.instace.GetTransportable(key);
    }

    public bool CheckCompatibility(Transportable other)
    {
        return CheckCompatibility(this, other);
    }

    public static bool CheckCompatibility(Transportable a, Transportable b)
    {
        //Pending implementation
        Debug.Log("Not implemented");
        return true;
    }

    public override string ToString()
    {
        return _scripatableObject.name;
    }

    internal void AssignGameObject(GameObject g)
    {
        _behaviour = g.GetComponent<TransportableBehaviour>();
        _behaviour.SetUp(this, _scripatableObject.sprite);
    }

    internal void GoTo(Transform transform, bool instant = false)
    {
        if (_behaviour)
            _behaviour.GoTo(transform, instant);
    }
}
