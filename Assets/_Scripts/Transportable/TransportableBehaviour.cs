using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportableBehaviour : MonoBehaviour
{
    [SerializeField]
    float _speed = 2f;
    [SerializeField]
    float _wiggleSpeed = 10f;
    [SerializeField]
    float _wiggleAmpitude = 10f;
    [SerializeField]
    float _smoothTransition = .1f;
    [SerializeField]
    float _flipSpeed = 1f;

    bool _mirror = false;
    float _wiggle = 0;
    float _rotY = 0;

    Transportable data;
    bool _walking;
    public bool Walking
    {
        get { return _walking; }
        set
        {
            _walking = value;
            if (_animator)
                _animator.SetBool("walking", _walking);
        }
    }

    GameObject sprite;
    GameObject shadow;

    Animator _animator;
    SpriteRenderer _renderer;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _renderer.sortingOrder = 1 + (int)Mathf.Abs(100 - Mathf.Min(transform.position.y * 10, 100));

        transform.rotation = Quaternion.identity;

        _rotY += _flipSpeed * 1000 * Time.deltaTime * (_mirror ? 1 : -1);
        _rotY = Mathf.Clamp(_rotY, 0, 180);
        Quaternion rotY = Quaternion.AngleAxis(_rotY, transform.up);

        _wiggle = Walking ? 1 : Mathf.Max(0, _wiggle - Time.deltaTime * (1f / _smoothTransition));
        float rotZ = _wiggle * (_mirror ? -1 : 1) * Mathf.Sin(_speed * Time.time * _wiggleSpeed) * _wiggleAmpitude;
        Quaternion rotXZ = Quaternion.Euler(-45, 0, rotZ);

        sprite.transform.localRotation = rotXZ * rotY;
    }

    public void SetUp(Transportable t, TransportableSO scriptableObject)
    {
        data = t;

        sprite = transform.GetChild(0).gameObject;
        shadow = transform.GetChild(1).gameObject;

        _animator = sprite.GetComponent<Animator>();
        _renderer = sprite.GetComponent<SpriteRenderer>();

        if (scriptableObject.AnimatorController != null)
            _animator.runtimeAnimatorController = scriptableObject.AnimatorController;
    }

    public void OnMouseDown()
    {
        GameManager.instance.TransportableInteraction(data);
    }

    Coroutine _movement;
    internal float GoTo(Transform target, bool instant, out float animationDuration, bool backwards)
    {
        _mirror = target.position.x < transform.position.x;
        if (backwards)
            _mirror = !_mirror;

        if (instant)
        {
            StopAllCoroutines();
            transform.position = target.position;
            Walking = false;
            animationDuration = 0;
            transform.parent = target;
        }
        else
        {
            if (Walking)
            {
                StopCoroutine(_movement);
                transform.position = transform.parent.position;
            }

            animationDuration = Vector2.Distance(transform.position, target.position) / _speed;
            _movement = StartCoroutine(MovementCoroutine(target, animationDuration));
        }
        //this.transform.parent = target;
        return animationDuration;
    }

    IEnumerator MovementCoroutine(Transform target, float duration)
    {
        Vector2 pos = transform.position;
        Walking = true;

        yield return null;

        float t = 0;
        while (t < 1)
        {
            transform.position = Vector2.Lerp(pos, target.position, t);
            t += Time.deltaTime / duration;
            yield return null;
        }

        Walking = false;

        _mirror = false;// I'm not sure

        transform.parent = target;

        yield return null;
    }
}
