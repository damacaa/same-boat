using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 2)]
public class Level : ScriptableObject
{
    public new string name;
    public int BoatCapacity = 1;
    public Level.Island[] Islands;

    [System.Serializable]
    public class Island
    {
        public TransportableSO[] transportables;
    }
}


