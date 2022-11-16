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
        foreach (var island in islandGOs)
        {
            island.SetActive(false);
        }

        for (int i = 0; i < islands.Length; i++)
        {
            int goId = i == islands.Length - 1 ? islandGOs.Length - 1 : i;
            islandGOs[goId].SetActive(true);
            islands[i].AssignGameObject(islandGOs[goId]);
        }
    }

    public void GenerateBoat(Boat boat)
    {
        var g = GameObject.Instantiate(_boatPrefabs[boat.Capacity - 1], Vector3.zero, Quaternion.identity);
        boat.SetUp(g);
    }
}
