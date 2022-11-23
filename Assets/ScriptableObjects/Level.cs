using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 2)]
public class Level : ScriptableObject
{
    public new string name;
    public int BoatCapacity = 1;
    public int BoatMaxWeightAllowed = 2;
    public int BoatMaxTravelCost = 0;
    public bool CanMoveEmpyBoat = true;
    public Level.Island[] Islands;
    public Rule[] rules;

    [System.Serializable]
    public class Island
    {
        public TransportableSO[] transportables;
    }
}


