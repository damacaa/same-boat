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

    public override void Execute()
    {
        success = _boat.UnloadBoat(_trasportable);
    }

    public override void Undo()
    {
        if (!success) return;
        _boat.LoadBoat(_trasportable);
    }

    public override string ToString()
    {
        return _trasportable + " unloaded, " + (success ? "suceeded" : "failed");
    }
}
