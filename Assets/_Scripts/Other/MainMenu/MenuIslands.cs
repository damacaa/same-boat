using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuIslands : MonoBehaviour
{
    [SerializeField]
    Level _emptyLevel;


    [SerializeField]
    GameObject spritePrefab;
    [SerializeField]
    Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        Island[] emptyIslands = new Island[_emptyLevel.Islands.Length];
        for (int i = 0; i < emptyIslands.Length; i++)
        {
            emptyIslands[i] = new Island();
        }

        MapGenerator.instace.GenerateMap(emptyIslands, _emptyLevel);
    }


    float _nextSpawnTime = 0;
    [SerializeField]
    float _spawnRate = 1f;
    private void Update()
    {
        if (Time.time > _nextSpawnTime)
        {
            _nextSpawnTime = Time.time + 1f / _spawnRate;

            var g = GameObject.Instantiate(spritePrefab);
            g.transform.rotation = Quaternion.Euler(-45, 0, 0);
            g.transform.position = Camera.main.transform.position;
            g.transform.position += Random.Range(27, 33) * Camera.main.transform.forward;
            g.transform.position += 7.5f * Camera.main.transform.up;
            g.transform.position += 7.5f * Camera.main.transform.right * Random.Range(-1f, 1f);

            g.GetComponent<MenuFallingSprite>().Velocity = -Camera.main.transform.up * Random.Range(1f, 3f);
            g.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        }
    }
}
