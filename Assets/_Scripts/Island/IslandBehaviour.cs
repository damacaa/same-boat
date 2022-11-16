using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IslandBehaviour : MonoBehaviour
{
    [SerializeField]
    Transform _port;
    [SerializeField]
    Transform _center;

    [SerializeField]
    Transform[] _transportablePositions;

    Island _island;

    // Start is called before the first frame update
    private void Awake()
    {
        return;

        Tilemap t = GetComponent<Tilemap>();
        var t0 = t.GetTile(new Vector3Int(-6, -6, 0));
        if (!t0)
            return;
        t.SetTile(new Vector3Int(-6, -5, 0), t0);


        Tile tempTile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;

        int size = 100;
        Texture2D tex = new Texture2D(size, size);

        Color[] colors = new Color[size * size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int id = i + (size * j);
                colors[id] = Mathf.Sin(i)*Color.red;
            }
        }

        tex.SetPixels(0, 0, size, size, colors);
        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        tempTile.sprite = sprite;

        t.SetTile(new Vector3Int(-6, -6, 0), tempTile);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void Assign(Island island)
    {
        _island = island;
    }

    private void OnMouseDown()
    {
        GameManager.instance.IslandInteraction(_island);
    }

    internal Transform GetSpot(int index)
    {
        return _transportablePositions[index];
    }

    internal Transform FindSpot(out int index)
    {
        for (index = 0; index < _transportablePositions.Length; index++)
        {
            Transform t = _transportablePositions[index];
            if (t.childCount == 0)
            {
                return t;
            }
        }

        return transform;
    }

    internal Vector3 GetPortPosition()
    {
        return _port.position;
    }
}
