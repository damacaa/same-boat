using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportableBehaviour : MonoBehaviour
{
    [SerializeField]
    float _speed = 2f;

    Transportable data;
    bool _walking = false;

    Animator _animator;
    SpriteRenderer _renderer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_walking)
        {
            float r = Mathf.Sin(_speed * Time.time * 10) * 10f;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, r));
        }

        _renderer.sortingOrder = 1 + (int)Mathf.Abs(100 - Mathf.Min(transform.position.y * 10, 100));
    }

    public void SetUp(Transportable t, TransportableSO scriptableObject)
    {
        data = t;

        if (!TryGetComponent<Animator>(out _animator))
        {
            _animator = gameObject.AddComponent<Animator>();
        }
        _renderer = gameObject.GetComponent<SpriteRenderer>();

        if (scriptableObject.animatorController != null)
            _animator.runtimeAnimatorController = scriptableObject.animatorController;
    }

    private void OnMouseDown()
    {
        GameManager.instance.TransportableInteraction(data);
    }

    Coroutine _movement;
    internal void GoTo(Transform target, bool instant)
    {
        if (instant)
        {
            StopAllCoroutines();
            transform.position = target.position;
        }
        else
        {
            if (_walking)
            {
                StopCoroutine(_movement);
                transform.position = transform.parent.position;
            }

            float duration = Vector2.Distance(transform.position, target.position) / _speed;
            _movement = StartCoroutine(MovementCoroutine(target, duration));
        }
        this.transform.parent = target;
    }

    IEnumerator MovementCoroutine(Transform target, float duration)
    {
        Vector2 pos = transform.position;
        _walking = true;

        yield return null;

        float t = 0;
        while (t < 1)
        {
            transform.position = Vector2.Lerp(pos, target.position, t);
            t += Time.deltaTime / duration;
            yield return null;
        }

        _walking = false;

        yield return null;
    }
}
