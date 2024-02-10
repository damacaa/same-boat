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

    [SerializeField]
    float idleSpeed = 1;
    [SerializeField]
    AnimationCurve _idleAnimation;
    [SerializeField]
    AudioClip _stepSound;

    bool _mirror = false;
    float _wiggle = 0;
    float _rotY = 0;
    float _unscaledCenterOffset;

    public Transportable Data { get; private set; }
    bool _walking;
    public bool Walking
    {
        get { return _walking; }
        set
        {
            _walking = value;

            if (!_particles)
                return;

            if (_walking)
                _particles.Play();
            else
                _particles.Stop();
        }
    }

    GameObject _sprite;
    GameObject _shadow;

    Animator _animator;
    SpriteRenderer _renderer;
    ParticleSystem _particles;
    AudioSource _audioSource;
    AudioSource _stepAudioSource;


    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponents<AudioSource>()[0];
        _stepAudioSource = GetComponents<AudioSource>()[1];
    }

    // Update is called once per frame
    void Update()
    {
        _renderer.sortingOrder = 1 + (int)Mathf.Abs(100 - Mathf.Min(transform.position.y * 10, 100));

        // Rotation
        transform.rotation = Quaternion.identity;//Always look at camera, even if parent rotates

        _rotY += _flipSpeed * 1000 * Time.deltaTime * (_mirror ? 1 : -1);
        _rotY = Mathf.Clamp(_rotY, 0, 180);
        Quaternion rotY = Quaternion.AngleAxis(_rotY, transform.up);

        _wiggle = Walking ? 1 : Mathf.Max(0, _wiggle - Time.deltaTime * (1f / _smoothTransition));
        float rotZ = _wiggle * (_mirror ? -1 : 1) * Mathf.Sin(_speed * Time.time * _wiggleSpeed) * _wiggleAmpitude;
        Quaternion rotXZ = Quaternion.Euler(-45, 0, rotZ);


        if (!_mirror ? rotZ < -_wiggleAmpitude + Time.deltaTime : rotZ > _wiggleAmpitude - Time.deltaTime)
            _stepAudioSource.PlayOneShot(_stepSound);


        _sprite.transform.localRotation = rotXZ * rotY;

        // Scale
        float scaleY = 0.5f;
        scaleY += _wiggle * 0.15f * (MathF.Sin(_speed * Time.time * _wiggleSpeed) + 1) / 2;//Walking
        scaleY += (1 - _wiggle) * _idleAnimation.Evaluate(Time.time * idleSpeed);//Resting

        _sprite.transform.localScale = new Vector3(0.5f, scaleY, 0.5f);

        // Position
        float h = _unscaledCenterOffset * _sprite.transform.localScale.y;

        Vector3 center = new Vector3();
        center.x = 0;
        center.y = Mathf.Cos(-45 * Mathf.Deg2Rad) * h;
        center.z = Mathf.Sin(-45 * Mathf.Deg2Rad) * h;

        _sprite.transform.localPosition = center + _wiggle * 0.3f * ((Mathf.Sin(_speed * Time.time * _wiggleSpeed) + 1f) / 2f) * _sprite.transform.up;
    }

    public void SetUp(Transportable t, TransportableSO scriptableObject)
    {
        Data = t;

        _sprite = transform.GetChild(0).gameObject;
        _shadow = transform.GetChild(1).gameObject;

        _animator = _sprite.GetComponent<Animator>();
        _renderer = _sprite.GetComponent<SpriteRenderer>();
        _renderer.sprite = scriptableObject.sprite;
        _particles = GetComponentInChildren<ParticleSystem>();
        _particles.Pause();


        int res = scriptableObject.sprite.texture.width;
        int halfRes = res / 2;
        float ppu = scriptableObject.sprite.pixelsPerUnit;
        _unscaledCenterOffset = halfRes / ppu;
    }



    public void OnMouseDown()
    {
        //GameManager.instance.TransportableInteraction(Data);
    }

    Coroutine _movement;
    internal float GoTo(Transform target, bool skipAnimation, out float animationDuration, bool backwards)
    {
        _mirror = target.position.x < transform.position.x;
        if (backwards)
            _mirror = !_mirror;

        if (skipAnimation)
        {
            StopAllCoroutines();
            transform.position = target.position;
            Walking = false;
            animationDuration = 0;
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

            if (Data.ScripatableObject.Sounds.Length > 0)
                _audioSource.PlayOneShot(Data.ScripatableObject.Sounds[UnityEngine.Random.Range(0, Data.ScripatableObject.Sounds.Length)]);
        }

        transform.parent = target;

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

        if (target.parent.parent.gameObject.TryGetComponent<BoatBehaviour>(out BoatBehaviour boat))
            SoundController.Instace.PlaySound(SoundController.Sound.Boat);

        yield return null;
    }
}
