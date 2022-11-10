using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    protected bool _success = true;
    public bool Success { get { return _success; } }
    public abstract void Execute(bool instant = false);
    public abstract void Undo(bool instant = false);

    public IEnumerator ExecuteCoroutine(bool instant = false)
    {
        Execute(instant);
        yield return null;
    }

    public IEnumerator UndoCoroutine(bool instant = false)
    {
        Undo(instant);
        yield return null;
    }
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