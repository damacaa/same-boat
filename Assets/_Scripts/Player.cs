using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float idleSpeed = 1;
    [SerializeField]
    AnimationCurve _idleAnimation;

    float _unscaledCenterOffset;

    GameObject _sprite;
    GameObject _shadow;

    SpriteRenderer _renderer;

    void Start()
    {
        _sprite = transform.GetChild(0).gameObject;
        _shadow = transform.GetChild(1).gameObject;

        _renderer = _sprite.GetComponent<SpriteRenderer>();

        int res = _renderer.sprite.texture.width;
        int halfRes = res / 2;
        float ppu = _renderer.sprite.pixelsPerUnit;
        _unscaledCenterOffset = halfRes / ppu;
    }

    // Update is called once per frame
    void Update()
    {
        _renderer.sortingOrder = 1 + (int)Mathf.Abs(100 - Mathf.Min(transform.position.y * 10, 100));

        transform.rotation = Quaternion.identity;
        _sprite.transform.localRotation = Quaternion.Euler(-45, 0, 0);

        // Scale
        float scaleY = 0.5f;
        scaleY += _idleAnimation.Evaluate(Time.time * idleSpeed);//Resting

        _sprite.transform.localScale = new Vector3(0.5f, scaleY, 0.5f);

        // Position
        float h = _unscaledCenterOffset * _sprite.transform.localScale.y;

        Vector3 center = new Vector3();
        center.x = 0;
        center.y = Mathf.Cos(-45 * Mathf.Deg2Rad) * h;
        center.z = Mathf.Sin(-45 * Mathf.Deg2Rad) * h;

        _sprite.transform.localPosition = center;
    }
}
