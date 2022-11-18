using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatLoadCommand : BoatCommand
{
    int _positionInIsland = -1;

    public BoatLoadCommand(Boat boat, Transportable actor)
    {
        _boat = boat;
        _trasportable = actor;
    }

    public override bool Execute(out float animationDuration, bool instant = false)
    {
        _success = _boat.LoadBoat(_trasportable, out _positionInIsland, out animationDuration, instant);
        return _success;
    }

    public override void Undo(out float animationDuration, bool instant = false)
    {
        _boat.UnloadBoat(_trasportable, _positionInIsland, out animationDuration, instant, true);
    }

    public override string ToString()
    {
        return _trasportable + " loaded, " + (_success ? "suceeded" : "failed");
    }
}
