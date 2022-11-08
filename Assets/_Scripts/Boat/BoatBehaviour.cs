using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatBehaviour : MonoBehaviour
{
    Boat _boat;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void SetUp(Boat boat)
    {
        _boat = boat;
    }

    private void OnMouseDown()
    {
        GameManager.instance.BoatInteraction(_boat);
    }

    internal Transform GetSeat(int pos)
    {
        //Needs work
        return transform;
    }
}
