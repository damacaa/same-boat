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

    public override void Execute(bool instant = false)
    {
        _previousIsland = _boat.GetCurrentIsland();
        _boat.GoTo(_island, instant);
    }

    public override void Undo(bool instant = false)
    {
        _boat.GoTo(_previousIsland, instant);
    }

    public override string ToString()
    {
        return "boat travelled to " + _island.Name;
    }
}
