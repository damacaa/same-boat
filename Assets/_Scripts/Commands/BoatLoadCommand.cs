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

    public override float Execute(bool instant = false)
    {
        _success = _boat.LoadBoat(_trasportable, out _positionInIsland, out float animationDuration, instant);
        return animationDuration;
    }

    public override float Undo(bool instant = false)
    {
        if (!_success) return 0;
        _boat.UnloadBoat(_trasportable, _positionInIsland, out float animationDuration, instant, true);
        return animationDuration;
    }

    public override string ToString()
    {
        return _trasportable + " loaded, " + (_success ? "suceeded" : "failed");
    }
}
