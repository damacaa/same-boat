using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IslandBehaviour : MonoBehaviour
{
    [SerializeField]
    public Transform _port;

    [SerializeField]
    List<Transform> _transportablePositions = new List<Transform>();

    public Island Data { get; private set; }

    // Start is called before the first frame update


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Outline outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    internal void Assign(Island island)
    {
        Data = island;
    }

    private void OnMouseDown()
    {
        //GameManager.instance.IslandInteraction(Data);
    }

    internal Transform GetSpot(int index)
    {
        return _transportablePositions[Math.Min(index, _transportablePositions.Count - 1)];
    }

    internal Transform FindSpot(out int index)
    {
        for (index = 0; index < _transportablePositions.Count; index++)
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

    internal void AddSpot(Transform t)
    {
        _transportablePositions.Add(t);
    }
}
