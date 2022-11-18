using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameObject _sprite;
    GameObject _shadow;

    SpriteRenderer _renderer;

    void Start()
    {
        _sprite = transform.GetChild(0).gameObject;
        _shadow = transform.GetChild(1).gameObject;

        _renderer = _sprite.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _renderer.sortingOrder = 1 + (int)Mathf.Abs(100 - Mathf.Min(transform.position.y * 10, 100));

        transform.rotation = Quaternion.identity;
        _sprite.transform.localRotation = Quaternion.Euler(-45, 0, 0);
    }
}
