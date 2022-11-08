using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatLoadCommand : BoatCommand
{
    public BoatLoadCommand(Boat boat, Transportable actor)
    {
        _boat = boat;
        _actor = actor;
    }

    public override void Execute()
    {
        success = _boat.LoadBoat(_actor);
    }

    public override void Undo()
    {
        if (!success) return;
        _boat.UnloadBoat(_actor);
    }

    public override string ToString()
    {
        return _actor + " loaded, " + (success ? "suceeded" : "failed");
    }
}
