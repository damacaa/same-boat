using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatUnloadCommand : BoatCommand
{
    public BoatUnloadCommand(Boat boat, Transportable actor)
    {
        _boat = boat;
        _trasportable = actor;
    }

    public override float Execute(bool instant = false)
    {
        _success = _boat.UnloadBoat(_trasportable, out float animationDuration, instant);
        return animationDuration;
    }

    public override float Undo(bool instant = false)
    {
        if (!_success) return 0;
        _boat.LoadBoat(_trasportable, out float animationDuration, instant); 
        return animationDuration;
    }

    public override string ToString()
    {
        return _trasportable + " unloaded, " + (_success ? "suceeded" : "failed");
    }
}
