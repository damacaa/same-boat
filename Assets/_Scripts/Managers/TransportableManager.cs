using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportableManager : MonoBehaviour
{
    public static TransportableManager instace;
    void Awake()
    {
        if (instace)
        {
            Destroy(this);
            return;
        }
        instace = this;
    }

    [SerializeField]
    GameObject transportablePrefab;

    internal void GenerateSprites(Island[] islands)
    {
        foreach (var island in islands)
        {
            foreach (var t in island.Transportables)
            {
                var g = GameObject.Instantiate(transportablePrefab, island.FindSpot(out int index));
                t.AssignGameObject(g);
            }
        }
    }
}


