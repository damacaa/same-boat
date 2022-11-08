using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    protected Transportable _actor;
    protected bool success = true;

    public abstract void Execute();
    public abstract void Undo();
}

public abstract class BoatCommand : Command
{
    protected Boat _boat;
    protected Island _island;

    protected BoatCommand() { }
    public BoatCommand(Transportable actor, Boat boat, Island island)
    {
        _actor = actor;
        _boat = boat;
        _island = island;
    }
}