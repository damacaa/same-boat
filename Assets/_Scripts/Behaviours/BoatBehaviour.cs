using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoatBehaviour : MonoBehaviour
{
    [SerializeField]
    float _speed = 5f;
    [SerializeField]
    float _rotationTime = 0.2f;
    [SerializeField]
    private bool _rotateBeforeMoving;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float _flagMinHeight = 0.3f;

    [SerializeField]
    Transform[] _seats;

    [SerializeField]
    Transform _flagPole;
    [SerializeField]
    Transform _flag;


    [SerializeField]
    private LayerMask _hitLayers;

    Outline _outline;
    AudioSource _audioSource;
    BoxCollider _boxCollider;

    public Boat Data { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _outline = GetComponent<Outline>();
        _audioSource = GetComponent<AudioSource>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        _outline.enabled = false;

        if (_loadRequested)
        {
            _loadRequested = false;

            if (!_loadRequest.skipAnimation)
                _loadRequest.transportable.GoTo(GetSeat(_loadRequest.pos), out float animationDuration, _loadRequest.skipAnimation, _loadRequest.backwards);
        }
    }

    internal void SetUp(Boat boat)
    {
        Data = boat;
    }


    internal Transform GetSeat(int pos)
    {
        while (_seats[pos].transform.childCount != 0 && (pos < _seats.Length - 1))
        {
            pos = (pos + 1);
        }

        return _seats[pos];
    }

    internal void GoTo(Island newIsland, out float animationDuration, bool skipAnimation, bool backwards)
    {
        animationDuration = 0;
        StopAllCoroutines();
        Vector3 destination = newIsland.Behaviour.GetPortPosition();
        if (skipAnimation)
        {
            transform.position = destination;
        }
        else
        {
            float animationSpeed = backwards ? 2f * _speed : _speed;
            animationDuration = _rotationTime + (Vector2.Distance(transform.position, destination) / animationSpeed);
            StartCoroutine(MovementCoroutine(destination, animationDuration, backwards));
        }
    }



    IEnumerator MovementCoroutine(Vector3 destination, float duration, bool backwards)
    {
        yield return new WaitForEndOfFrame();

        Quaternion flagStartingRotation = _flagPole.rotation;
        Quaternion flagTargetRot = Quaternion.LookRotation(-Vector3.forward, !backwards ? transform.position - destination : destination - transform.position);

        Quaternion startingRotation = transform.rotation;
        Vector3 direction = destination - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, Vector3.Dot(transform.up, direction) < 0 ? transform.position - destination : destination - transform.position);

        Vector3 flagScale = _flag.transform.localScale;

        float flagCorrectionFactor = 1.0f - _flagMinHeight;

        Vector3 pos = transform.position;
        float t = 0;
        while (t < 1)
        {
            flagScale.y = (flagCorrectionFactor * t) + _flagMinHeight;
            _flag.transform.localScale = flagScale;

            float tBezier = t * t * (3.0f - 2.0f * t);

            if (_rotateBeforeMoving)
                transform.rotation = Quaternion.Lerp(startingRotation, targetRot, Mathf.Min(t, 1));

            _flagPole.rotation = Quaternion.Lerp(flagStartingRotation, flagTargetRot, tBezier);

            t += Time.deltaTime / (backwards ? 0.5f * _rotationTime : _rotationTime);
            yield return null;
        }

        _audioSource.Play();

        t = 0;
        // Move boat to new location
        while (t < 1)
        {
            //flagScale.y = flagCorrectionFactor * (1.0f - Mathf.Pow(t, 10.0f)) + _flagMinHeight;
            flagScale.z = 1.0f * (1.0f - Mathf.Pow(2.0f * t - 1.0f, 4.0f));
            _flag.transform.localScale = flagScale;

            // Rotate boat while it starts moving
            if (!_rotateBeforeMoving)
                transform.rotation = Quaternion.Lerp(startingRotation, targetRot, 1.0f * Mathf.Min(1.5f * t, 1));

            // Flag always looking in the same direction
            _flagPole.rotation = flagTargetRot;

            // Move boat
            transform.position = Vector2.Lerp(pos, destination, t * t * (3.0f - 2.0f * t));

            // Increase t
            t += Time.deltaTime / (duration - _rotationTime);

            yield return null;
        }

        _audioSource.Stop();

        t = 0;
        while (t < 1)
        {
            flagScale.y = (flagCorrectionFactor * (1.0f - t)) + _flagMinHeight;
            _flag.transform.localScale = flagScale;

            t += Time.deltaTime / (backwards ? 0.5f * _rotationTime : _rotationTime);
            yield return null;
        }


        yield return null;
    }

    internal Transform FindSeat()
    {
        for (int i = 0; i < _seats.Length; i++)
        {
            if (_seats[i].childCount == 0)
                return _seats[i];
        }

        return transform;
    }

    bool _loadRequested = false;
    LoadRequest _loadRequest;


    internal void ScheduleLoad(Transportable newTransportable, int pos, bool skipAnimation, bool backwards)
    {
        _loadRequested = true;
        _loadRequest = new LoadRequest { transportable = newTransportable, pos = pos, skipAnimation = skipAnimation, backwards = backwards };
    }

    private class LoadRequest
    {
        public Transportable transportable;
        public int pos;
        public bool skipAnimation;
        public bool backwards;
    }
}
