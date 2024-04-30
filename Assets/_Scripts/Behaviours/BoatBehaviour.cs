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
    private float _stoppingDistance = 1.0f;

    [SerializeField]
    Transform[] _seats;

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
            animationDuration = Vector2.Distance(transform.position, destination) / _speed;
            StartCoroutine(MovementCoroutine(destination, animationDuration, backwards));
        }
    }



    IEnumerator MovementCoroutine(Vector3 destination, float duration, bool backwards)
    {
        yield return new WaitForEndOfFrame();

        Quaternion flagStartingRotation = _flag.rotation;
        Quaternion flagTargetRot = Quaternion.LookRotation(Vector3.forward, !backwards ? transform.position - destination : destination - transform.position);

        Quaternion startingRotation = transform.rotation;
        Vector3 direction = destination - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, Vector3.Dot(transform.up, direction) < 0 ? transform.position - destination : destination - transform.position);

        Vector3 halfSize = 0.5f * _boxCollider.size;
        halfSize.y += _stoppingDistance;

        Vector3 pos = transform.position;
        float t = 0;
        while (t < 1)
        {
            float tBezier = t * t * (3.0f - 2.0f * t);

            if (_rotateBeforeMoving)
                transform.rotation = Quaternion.Lerp(startingRotation, targetRot, Mathf.Min(t, 1));
            _flag.rotation = Quaternion.Lerp(flagStartingRotation, flagTargetRot, tBezier);

            t += Time.deltaTime / _rotationTime;
            yield return null;
        }


        _audioSource.Play();

        float lastTime = 0;

        t = 0;
        // Move boat to new location
        while (t < 1)
        {
            // Rotate boat while it starts moving
            if (!_rotateBeforeMoving)
                transform.rotation = Quaternion.Lerp(startingRotation, targetRot, Mathf.Min(2 * t, 1));

            // Flag always looking in the same direction
            _flag.rotation = flagTargetRot;

            // Move boat
            transform.position = Vector2.Lerp(pos, destination, t * t * (3.0f - 2.0f * t));

            // Increase t
            t += Time.deltaTime / duration;

            // Every few ms, check if the boat collides with any of the islands
            if (t > 0.5f && Time.time - lastTime > 0.05f)
            {
                // Store time to avoid checking every frame
                lastTime = Time.time;

                // Raycast with the size of the collider to check collisions with islands
                Vector3 colliderPos = transform.position + _boxCollider.center;
                var hits = Physics.OverlapBox(colliderPos, halfSize, transform.rotation, _hitLayers);
                // Loop all hits
                foreach (var hit in hits)
                {
                    // Start a different animation
                    t = 0;

                    destination = transform.position + (destination - pos).normalized;
                    pos = transform.position;

                    // Calculate animation duration based on distance and uniform speed
                    float animationDuration = Vector2.Distance(pos, destination) / _speed;

                    // Move boat to shore
                    while (t < 1)
                    {
                        float easeOutTime = 1f - (1f - t) * (1f - t);
                        transform.position = Vector2.Lerp(pos, destination, easeOutTime);
                        t += Time.deltaTime / animationDuration;
                        yield return null;
                    }

                    // Stop loop
                    break;
                }
            }

            yield return null;
        }

        _audioSource.Stop();

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
    private bool _rotateBeforeMoving;

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode

        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireCube(transform.position, transform.localScale);

        Vector3 pos = transform.position + _boxCollider.center;// + _stoppingDistance * transform.up;
        Vector3 halfSize = 0.5f * _boxCollider.size;
        halfSize.y += _stoppingDistance;
        Vector2[] vs = { new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1) };

        for (int i = 0; i < vs.Length; i++)
        {
            Vector3 p = pos +
                Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward) * new Vector3(vs[i].x * halfSize.x, vs[i].y * halfSize.y, 0);

            Gizmos.DrawSphere(p, 0.3f);

        }



    }
}
