using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    [SerializeField]
    float distance = 50f;

    [SerializeField]
    Line _line;

    TransportableBehaviour _transportable;
    IslandBehaviour _island;
    BoatBehaviour _boat;

    GameObject _cursor;


    // Start is called before the first frame update
    void Start()
    {
        _cursor = new GameObject("Cursor");
        _cursor.transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
        else if (Input.GetMouseButton(0))
        {
            OnDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnRelease();
        }
    }


    void OnClick()
    {
        //create a ray cast and set it to the mouses cursor position in game
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            var g = hit.collider.gameObject;
            if (g.TryGetComponent<TransportableBehaviour>(out TransportableBehaviour transportable))
            {
                _transportable = transportable;
                _boat = null;
                _island = null;
                _line.Begin(g.transform);
            }
            else if (g.TryGetComponent<BoatBehaviour>(out BoatBehaviour boat))
            {
                _boat = boat;
                _transportable = null;
                _island = null;
                _line.Begin(g.transform);
            }
            else
            {
                _cursor.transform.position = hit.point;
            }
        }
        else { }

    }

    private void OnDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance) )
        {
            var g = hit.collider.gameObject;

            if (_boat && g.TryGetComponent<IslandBehaviour>(out IslandBehaviour island))
            {
                _line.End(g.transform.GetChild(0));
            }
            else if (_transportable && g.TryGetComponent<IslandBehaviour>(out island))
            {
                _line.End(island.FindSpot(out int index));
            }
            else if (_transportable && g.TryGetComponent<BoatBehaviour>(out BoatBehaviour boat))
            {
                _line.End(boat.FindSeat());
            }
            else
            {
                _cursor.transform.position = hit.point;
                _line.End(_cursor.transform);
            }

            //print(g.name);
        }
    }

    void OnRelease()
    {
        bool fail = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            var g = hit.collider.gameObject;

            if (_boat && g.TryGetComponent<IslandBehaviour>(out IslandBehaviour island))
            {
                _line.End(g.transform.GetChild(0));
                fail = !GameManager.instance.MoveBoatTo(_boat, island);
            }
            else if (_transportable && g.TryGetComponent<IslandBehaviour>(out island))
            {
                _line.End(island.FindSpot(out int index));
                fail = !GameManager.instance.MoveTransportableTo(_transportable, island);
            }
            else if (_transportable && g.TryGetComponent<BoatBehaviour>(out BoatBehaviour boat))
            {
                _line.End(boat.FindSeat());
                fail = !GameManager.instance.MoveTransportableTo(_transportable, boat);
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
}
