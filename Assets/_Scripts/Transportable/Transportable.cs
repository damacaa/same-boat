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

    public int PositionIndexInIsland { get; internal set; }

    [Obsolete("")]
    public Transportable(string key)
    {
        _scripatableObject = TransportableManager.instace.GetTransportable(key);
    }

    public Transportable(TransportableSO scripatableObject)
    {
        _scripatableObject = scripatableObject;
    }

    public Transportable(Transportable original)
    {
        _scripatableObject = original._scripatableObject;
    }

    public bool CheckCompatibility(Transportable other)
    {
        return CheckCompatibility(this, other);
    }

    public static bool CheckCompatibility(Transportable a, Transportable b)
    {
        if (a._scripatableObject.name.ToLower() == "fox" && b._scripatableObject.name.ToLower() == "chicken")
            return false;

        if (a._scripatableObject.name.ToLower() == "chicken" && b._scripatableObject.name.ToLower() == "fox")
            return false;

        if (a._scripatableObject.name.ToLower() == "chicken" && b._scripatableObject.name.ToLower() == "corn")
            return false;

        if (a._scripatableObject.name.ToLower() == "corn" && b._scripatableObject.name.ToLower() == "chicken")
            return false;

        return true;
    }

    public override string ToString()
    {
        return _scripatableObject.name;
    }

    internal void AssignGameObject(GameObject g)
    {
        _behaviour = g.GetComponent<TransportableBehaviour>();
        _behaviour.SetUp(this, _scripatableObject);
    }

    internal void GoTo(Transform transform, out float animationDuration, bool instant, bool backwards = false)
    {
        animationDuration = 0;
        if (_behaviour)
            _behaviour.GoTo(transform, instant, out animationDuration, backwards);
    }

    internal void Teleport(Boat boat, int pos)
    {
        _currentIsland.Remove(this);
        _currentIsland = null;
        boat.ForceLoad(this, pos);
    }

    internal void Teleport(Island island)
    {
        if (_currentIsland != null)
            _currentIsland.Remove(this);
        island.Add(this, out float animationDuration, true);
        _currentIsland = island;
    }
}
