using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 2)]
public class Level : ScriptableObject
{
    public new string name;
    [Range(1, 3)]
    public int BoatCapacity = 1;
    [Range(0, 10)]
    public int BoatMaxWeightAllowed = 0;
    [Range(0, 100)]
    public int BoatMaxTravelCost = 0;
    public bool OnlyHumansCanDrive = false;
    public Texture2D Map;
    public Level.Island[] Islands;
    public Rule[] rules;


    public override string ToString()
    {
        string s = name + ":\n";

        s += "Boat has " + BoatCapacity + (BoatCapacity == 1 ? " seat.\n" : " seats.\n");

        if (BoatMaxWeightAllowed > 0)
            s += "Max weight is " + BoatMaxWeightAllowed + ".\n";

        if (BoatMaxTravelCost > 0)
            s += "Max travel cost is " + BoatMaxTravelCost + ".\n";


        foreach (var rule in rules)
        {
            s += rule + "\n";
        }

        return s;
    }

    [System.Serializable]
    public class Island
    {
        public TransportableSO[] transportables;
    }
}


