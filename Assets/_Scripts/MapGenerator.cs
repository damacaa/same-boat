using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator instace;
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
    GameObject _islandPrefab;


    [SerializeField]
    GameObject[] islandGOs;

    [SerializeField]
    GameObject[] _boatPrefabs;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateMap(Island[] islands)
    {
        print("Please implement me Alpuerro uwu");

        int i = 0;
        foreach (var island in islands)
        {
            island.AssignGameObject(islandGOs[i]);
            i++;
        }
    }

    public void GenerateBoat(Boat boat)
    {
        var g = GameObject.Instantiate(_boatPrefabs[boat.Capacity - 1], Vector3.zero, Quaternion.identity);
        boat.SetUp(g);
    }
}
