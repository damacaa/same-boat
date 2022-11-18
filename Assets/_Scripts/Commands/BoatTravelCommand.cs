using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatTravelCommand : BoatCommand
{
    Island _previousIsland;

    public BoatTravelCommand(Boat boat, Island island)
    {
        _boat = boat;
        _island = island;
    }

    public override bool Execute(out float animationDuration, bool instant = false)
    {
        _previousIsland = _boat.GetCurrentIsland();
        _boat.GoTo(_island, out animationDuration, instant);
        return true;
    }

    public override void Undo(out float animationDuration, bool instant = false)
    {
        _boat.GoTo(_previousIsland, out animationDuration, instant);
    }

    public override string ToString()
    {
        return "boat travelled to " + _island.Name;
    }
}
