using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatLoadCommand : BoatCommand
{
    public BoatLoadCommand(Boat boat, Transportable actor)
    {
        _boat = boat;
        _trasportable = actor;
    }

    public override void Execute(bool instant = false)
    {
        _success = _boat.LoadBoat(_trasportable, instant);
    }

    public override void Undo(bool instant = false)
    {
        if (!_success) return;
        _boat.UnloadBoat(_trasportable, instant);
    }

    public override string ToString()
    {
        return _trasportable + " loaded, " + (_success ? "suceeded" : "failed");
    }
}
