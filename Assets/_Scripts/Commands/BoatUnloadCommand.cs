using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatUnloadCommand : BoatCommand
{
    public BoatUnloadCommand(Boat boat, Transportable actor)
    {
        _boat = boat;
        _actor = actor;
    }

    public override void Execute()
    {
        success = _boat.UnloadBoat(_actor);
        if (success) Debug.Log("Fail to unload!");
    }

    public override void Undo()
    {
        if (!success) return;
        _boat.LoadBoat(_actor);
    }

    public override string ToString()
    {
        return _actor + " unloaded, " + (success ? "suceeded" : "failed");
    }
}
