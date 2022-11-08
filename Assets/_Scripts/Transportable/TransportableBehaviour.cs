using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportableBehaviour : MonoBehaviour
{
    Transportable data;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(Transportable t, Sprite sprite)
    {
        data = t;
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    private void OnMouseDown()
    {
        GameManager.instance.TransportableInteraction(data);
    }

    internal void GoTo(Transform transform)
    {
        this.transform.position = transform.position;
        this.transform.parent = transform;
    }
}
