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

    internal Transform FindSpot()
    {
        Transform t = new GameObject("Spot").transform;
        t.position = (Vector2)_center.position + 2f * UnityEngine.Random.insideUnitCircle;
        t.position = t.position - 0.05f * Vector3.forward;
        t.parent = transform;
        return t;
    }

    internal Vector3 GetPortPosition()
    {
        return _port.position;
    }
}
