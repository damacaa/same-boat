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

    public override bool Execute(out float animationDuration, bool skipAnimation = false)
    {
        _previousIsland = _boat.Island;
        _success = _boat.GoTo(_island, out animationDuration, skipAnimation, false);
        return _success;
    }

    public override void Undo(out float animationDuration, bool skipAnimation = false)
    {
        if (!_success)
        {
            animationDuration = 0;
            return;
        }
        _boat.GoTo(_previousIsland, out animationDuration, skipAnimation, true);
    }

    public override string ToString()
    {
        return "boat travelled to " + _island.Name;
    }
}
