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

    internal void GoTo(Transform transform, bool instant)
    {
        this.transform.parent = transform;
        if (instant)
        {
            this.transform.position = transform.position;
        }
        else
        {
            StartCoroutine(MovementCoroutine());
        }
    }

    IEnumerator MovementCoroutine(float duration = 0.5f)
    {
        Vector2 pos = transform.position;


        float t = 0;
        while (t < 1)
        {
            transform.position = Vector2.Lerp(pos, transform.parent.position, t);
            t += Time.deltaTime / duration;
            yield return null;
        }


        yield return null;
    }
}
