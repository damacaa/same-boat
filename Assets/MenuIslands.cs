using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuIslands : MonoBehaviour
{
    [SerializeField]
    Level _emptyLevel;


    // Start is called before the first frame update
    void Start()
    {
        Island[] emptyIslands = new Island[_emptyLevel.Islands.Length];
        for (int i = 0; i < emptyIslands.Length; i++)
        {
            emptyIslands[i] = new Island();
        }

        MapGenerator.instace.GenerateMap(emptyIslands, _emptyLevel);
    }
}
