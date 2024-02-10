using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transportable
{
    TransportableBehaviour _behaviour;
    public TransportableBehaviour Behaviour { get { return _behaviour; } }
    public TransportableSO ScripatableObject { get; private set; }

    Island _currentIsland;
    public Island Island { get { return _currentIsland; } set { _currentIsland = value; } }

    public int Weight { get { return ScripatableObject.Weight; } }
    public int PositionIndexInIsland { get; internal set; }

    public Transportable(TransportableSO scripatableObject)
    {
        ScripatableObject = scripatableObject;
    }

    public Transportable(Transportable original)
    {
        ScripatableObject = original.ScripatableObject;
    }

    /*public bool CheckCompatibility(Transportable other)
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
    }*/

    public override string ToString()
    {
        return ScripatableObject.name;
    }

    internal void AssignGameObject(GameObject g)
    {
        _behaviour = g.GetComponent<TransportableBehaviour>();
        _behaviour.SetUp(this, ScripatableObject);
    }

    internal void GoTo(Transform transform, out float animationDuration, bool skipAnimation, bool backwards = false)
    {
        animationDuration = 0;
        if (_behaviour)
            _behaviour.GoTo(transform, skipAnimation, out animationDuration, backwards);
    }
}
