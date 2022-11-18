using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatUnloadCommand : BoatCommand
{
    int _positionInIsland = -1;

    public BoatUnloadCommand(Boat boat, Transportable actor)
    {
        _boat = boat;
        _trasportable = actor;
    }

    public override bool Execute(out float animationDuration, bool instant = false)
    {
        _success = _boat.UnloadBoat(_trasportable, _positionInIsland, out animationDuration, instant);
        return _success;
    }

    public override void Undo(out float animationDuration, bool instant = false)
    {
        _boat.LoadBoat(_trasportable, out _positionInIsland, out  animationDuration, instant, true);
    }

    public override string ToString()
    {
        return _trasportable + " unloaded, " + (_success ? "suceeded" : "failed");
    }
}
