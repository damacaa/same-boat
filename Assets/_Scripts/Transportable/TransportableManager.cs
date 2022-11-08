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
    List<TransportableSO> transportables;



    [SerializeField]
    GameObject transportablePrefab;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public TransportableSO GetTransportable(string name)
    {
        return transportables.Find(i => i.name.ToLower() == name.ToLower());
    }

    internal void GenerateSprites(Island[] islands)
    {
        foreach (var island in islands)
        {
            foreach (var t in island.Transportables)
            {
                var g = GameObject.Instantiate(transportablePrefab, island.FindSpot());
                t.AssignGameObject(g);
            }
        }
    }
}


