using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    protected bool success = true;
    public bool Success { get { return success; } }
    public abstract void Execute();
    public abstract void Undo();
}

public abstract class BoatCommand : Command
{
    protected Transportable _trasportable;
    protected Boat _boat;
    protected Island _island;

    protected BoatCommand() { }
    public BoatCommand(Transportable actor, Boat boat, Island island)
    {
        _trasportable = actor;
        _boat = boat;
        _island = island;
    }
}