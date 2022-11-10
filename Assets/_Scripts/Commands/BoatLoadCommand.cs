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

    public override void Execute()
    {
        success = _boat.LoadBoat(_trasportable);
    }

    public override void Undo()
    {
        if (!success) return;
        _boat.UnloadBoat(_trasportable);
    }

    public override string ToString()
    {
        return _trasportable + " loaded, " + (success ? "suceeded" : "failed");
    }
}
