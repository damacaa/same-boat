using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatBehaviour : MonoBehaviour
{
    [SerializeField]
    float _speed = 5f;
    [SerializeField]
    float _rotationTime = 0.2f;

    [SerializeField]
    Transform[] _seats;

    Outline _outline;
    AudioSource _audioSource;

    public Boat Data { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        _outline = GetComponent<Outline>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        _outline.enabled = false;
    }

    internal void SetUp(Boat boat)
    {
        Data = boat;
    }

    private void OnMouseDown()
    {
        //GameManager.instance.BoatInteraction(Data);
    }

    internal Transform GetSeat(int pos)
    {
        while(_seats[pos].transform.childCount != 0)
        {
            pos = (pos + 1) % _seats.Length;
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


        Quaternion rot = transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, backwards ? transform.position - destination : destination - transform.position);

        Vector2 pos = transform.position;
        float t = 0;
        while (t < 1)
        {
            transform.rotation = Quaternion.Lerp(rot, targetRot, t);
            t += Time.deltaTime / _rotationTime;
            yield return null;
        }


        _audioSource.Play();

        t = 0;
        while (t < 1)
        {
            transform.position = Vector2.Lerp(pos, destination, t);
            t += Time.deltaTime / duration;
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
}
