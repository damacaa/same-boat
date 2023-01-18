using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    protected bool _success = true;
    public bool Success { get { return _success; } }

    public abstract bool Execute(out float animationDuration, bool instant = false);
    public abstract void Undo(out float animationDuration, bool instant = false);

    public bool Execute() { return Execute(out float animationDuration, true); }
    public void Undo() { Undo(out float animationDuration, true); }
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