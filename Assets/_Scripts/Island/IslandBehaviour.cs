using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandBehaviour : MonoBehaviour
{
    [SerializeField]
    Transform _port;
    [SerializeField]
    Transform _center;

    [SerializeField]
    Transform[] _transportablePositions;

    Island _island;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void Assign(Island island)
    {
        _island = island;
    }

    private void OnMouseDown()
    {
        GameManager.instance.IslandInteraction(_island);
    }

    internal Transform GetSpot( int index)
    {
        return _transportablePositions[index];
    }

    internal Transform FindSpot(out int index)
    {
        for (index = 0; index < _transportablePositions.Length; index++)
        {
            Transform t = _transportablePositions[index];
            if (t.childCount == 0)
            {
                return t;
            }
        }

        return transform;
    }

    internal Vector3 GetPortPosition()
    {
        return _port.position;
    }
}
