using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    [SerializeField]
    float distance = 50f;
    [SerializeField]
    Vector2 _offset;

    [SerializeField]
    LayerMask _layerMask;

    [SerializeField]
    Line _line;

    TransportableBehaviour _transportable;
    IslandBehaviour _island;
    BoatBehaviour _boat;

    GameObject _cursor;

    public static bool InputEnabled = true;



    // Start is called before the first frame update
    void Start()
    {
        _cursor = new GameObject("Cursor");
        _cursor.transform.position = Vector3.zero;

        if (_island != null) { }
    }

    // Update is called once per frame
    void LateUpdate()
    {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
        else if (Input.GetMouseButton(0))
        {
            OnHover();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnRelease();
        }
#endif


    }


    void OnClick()
    {
        if (!InputEnabled)
            return;

        // Ray from mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + (Vector3)_offset);

        // Find collisions with ray
        //var hits = Physics.SphereCastAll(ray, _r, 100f, Physics.AllLayers);
        var hits = Physics.RaycastAll(ray, distance);

        // Handle no hits
        if (hits.Length == 0)
        {
            return;
        }

        // Sort hits by y coordinate
        List<RaycastHit> hitList = hits.ToList().OrderBy(a => a.collider.transform.position.y).ToList();

        // Filter by tag
        var characters = hitList.FindAll(a => a.collider.CompareTag("Draggable"));

        if (characters.Count == 0)
            return;


        var firstObject = characters[0].collider.gameObject;
        if (firstObject.TryGetComponent<TransportableBehaviour>(out TransportableBehaviour transportable))
        {
            HandleTransportableClick(transportable);
        }
        else if (firstObject.TryGetComponent<BoatBehaviour>(out BoatBehaviour boat))
        {
            if (characters.Count > 1 &&
                characters[1].collider.gameObject &&
                characters[1].collider.gameObject.TryGetComponent<TransportableBehaviour>(out TransportableBehaviour hiddenTransportable))
            {
                HandleTransportableClick(hiddenTransportable);

            }
            else
            {
                _boat = boat;
                _transportable = null;
                _island = null;
                _line.Begin(boat.transform);
            }
        }
        else
        {
            _cursor.transform.position = hits[0].point;
        }


    }

    private void HandleTransportableClick(TransportableBehaviour transportable)
    {
        _transportable = transportable;
        _boat = null;
        _island = null;
        _line.Begin(transportable.transform);
    }

    private void OnHover()
    {

        /*Ray rayy = Camera.main.ScreenPointToRay(Input.mousePosition + (Vector3)_offset);
        var hits = Physics.SphereCastAll(rayy, _r, 100f, Physics.AllLayers);
        List<RaycastHit> hitList = hits.ToList().OrderByDescending(a => a.collider.transform.position.y).ToList();

        var item = hitList.Find(a => a.collider.CompareTag("Character"));

        if (item.collider)
        {

           // item.collider.gameObject.SetActive(false);

        if (item.collider.CompareTag("Character"))
            item.collider.GetComponentInChildren<SpriteRenderer>().color = new Color(
                0.5f * (Mathf.Sin(10 * Time.time) + 1f),
                0.5f * (Mathf.Sin(10 * Time.time) + 1f),
                0.5f * (Mathf.Sin(10 * Time.time) + 1f)
                );

       // Debug.Log(item.collider.transform.position.y);
        }*/

        if (!InputEnabled)
        {
            _line.Reset();
            return;
        }


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + (Vector3)_offset);

        if (Physics.Raycast(ray, out RaycastHit hit, distance, _layerMask))
        {
            var g = hit.collider.gameObject;
            _cursor.transform.position = hit.point;


            if (_boat && g.TryGetComponent<IslandBehaviour>(out IslandBehaviour island))
            {
                //_line.End(g.transform.GetChild(0));
                Outline outline = g.GetComponent<Outline>();
                outline.enabled = true;
            }
            else if (_boat)
            {
                _line.End(_cursor.transform);
            }
            else if (_transportable && g.TryGetComponent<IslandBehaviour>(out island))
            {
                //_line.End(island.FindSpot(out int index));
                if (_transportable.Data.Island == null)
                {
                    Outline outline = g.GetComponent<Outline>();
                    outline.enabled = true;
                }
            }
            else if (_transportable && g.TryGetComponent<BoatBehaviour>(out BoatBehaviour boat))
            {
                //_line.End(boat.transform);
                if (boat.Data.Island == _transportable.Data.Island)
                {
                    Outline outline = g.GetComponent<Outline>();
                    outline.enabled = true;
                }
            }
            else if (!_transportable)
            {
                //return;
            }

            _line.End(_cursor.transform);
        }
    }

    void OnRelease()
    {
        if (!InputEnabled)
        {
            _line.Reset();
            return;
        }

        bool fail = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + (Vector3)_offset);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance, _layerMask))
        {
            var g = hit.collider.gameObject;

            if (_boat && g.TryGetComponent<IslandBehaviour>(out IslandBehaviour island))
            {
                _line.End(g.transform.GetChild(0));
                fail = !GameManager.Instance.MoveBoatTo(_boat, island);
            }
            else if (_transportable && g.TryGetComponent<IslandBehaviour>(out island))
            {
                _line.End(island.FindSpot(out int index));
                fail = !GameManager.Instance.MoveTransportableTo(_transportable, island);
            }
            else if (_transportable && g.TryGetComponent<BoatBehaviour>(out BoatBehaviour boat))
            {
                _line.End(boat.FindSeat());
                fail = !GameManager.Instance.LoadTransportableOnBoat(_transportable, boat);
            }
            else
            {
                fail = true;
            }
        }
        else
        {
            fail = true;
        }

        _transportable = null;
        _island = null;
        _boat = null;


        if (fail)
            _line.Reset();
    }


    public void EnableInput()
    {
        InputEnabled = true;
    }

    public void DisableInput()
    {
        InputEnabled = false;
    }
}
