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

    public override void Execute(bool instant = false)
    {
        _success = _boat.UnloadBoat(_trasportable, instant);
    }

    public override void Undo(bool instant = false)
    {
        if (!_success) return;
        _boat.LoadBoat(_trasportable, instant);
    }

    public override string ToString()
    {
        return _trasportable + " unloaded, " + (_success ? "suceeded" : "failed");
    }
}
