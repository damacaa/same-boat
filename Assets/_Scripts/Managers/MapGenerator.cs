using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator instace;
    void Awake()
    {
        if (instace)
        {
            Destroy(this);
            return;
        }
        instace = this;
    }

    [SerializeField]
    Vector2Int size = Vector2Int.one;
    [SerializeField]
    int resolution = 1;

    [SerializeField]
    Material worldMaterial;
    [SerializeField]
    Color[] colors;
    [SerializeField]
    Color _darkColor;

    [SerializeField]
    Texture2D _map;
    [SerializeField]
    float _threshold = .5f;

    [SerializeField]
    GameObject[] _boatPrefabs;

    [SerializeField]
    GameObject[] _floorDecorations;

    public void GenerateMap(Island[] islands, Level level)
    {
        //Random.InitState(42);

        _map = level.Map;

        // Read data from file
        int[,] tiles = new int[size.x * resolution, size.y * resolution];

        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                var color = _map.GetPixelBilinear(((float)i) / tiles.GetLength(0), ((float)j) / tiles.GetLength(1));
                float average = (color.r + color.b + color.b) / 3f;
                tiles[i, j] = average > _threshold ? 1 : 0;
            }
        }

        // Size of a voxel
        float blockSize = 1f / resolution;
        // Decorations
        AddDecorations(new Map((int[,])tiles.Clone()));

        // Split data in multiple maps
        List<Map> maps = new List<Map>();
        while (maps.Count < islands.Length)
        {
            var islandMap = ExtractIslandMap(ref tiles);
            if (islandMap == null)
                break;
            maps.Add(islandMap);
        }

        // Order maps based on size
        maps = maps.OrderBy(m => m.MaxY).ToList();


        for (int i = 0; i < islands.Length; i++)
        {
            // Mesh generation
            Material mat = new Material(worldMaterial);
            mat.color = colors[i];
            var g = MeshGenerator.GenerateMesh(maps[i].Tiles, blockSize, mat);

            g.name = "Island " + i;
            g.transform.parent = transform;
            g.layer = gameObject.layer;
            IslandBehaviour behaviour = g.AddComponent<IslandBehaviour>();
            Outline outline = g.AddComponent<Outline>();
            outline.enabled = false;
            outline.OutlineWidth = 2f;
            outline.OutlineColor = colors[i];
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.UpdateMaterialProperties();
            maps[i].GameObject = g;

            // Port
            var portTransform = GeneratePort(maps[i]);
            portTransform.parent = g.transform;
            behaviour._port = portTransform;

            Transform spotsTransform = new GameObject("Spots").transform;
            spotsTransform.parent = g.transform;

            // Spots
            Transform[] spots = FindSpots(maps[i]);
            foreach (var s in spots)
            {
                if (!s)
                    continue;

                s.transform.parent = spotsTransform;
                behaviour.AddSpot(s);
            }



            // Link data with behaviour
            islands[i].AssignGameObject(g);
        }

        // Show unused islands ?
        var go = MeshGenerator.GenerateMesh(tiles, blockSize, worldMaterial);
        go.name = "Other";
        go.transform.parent = transform;
        var darkMaterial = new Material(worldMaterial);
        darkMaterial.color = _darkColor;
        go.GetComponent<MeshRenderer>().material = darkMaterial;

        // Functions /////////////////////////////////////////////////////////////////////////////////////

        Transform GeneratePort(Map map)
        {
            List<Vector2Int> openList = new List<Vector2Int>();
            openList.Add(new Vector2Int(map.Width / 2, map.Height / 2));

            Vector2Int port = new Vector2Int();
            int iter = 0;
            while (openList.Count > 0 && iter < 1000)
            {
                iter++;
                port = openList[0];
                openList.Remove(port);

                if (map[port.x, port.y] == 1)
                    break;

                bool done = false;
                for (int i = -5; i <= 5; i++)
                {
                    for (int j = -5; j <= 5; j++)
                    {
                        int px = port.x + i;
                        int py = port.y + j;

                        Vector2Int nv = new Vector2Int(px, py);

                        if (px < 0 || px >= map.Width || py < 0 || py >= map.Height || openList.Contains(nv))
                            continue;

                        if (map[px, py] == 1)
                        {
                            map.Port = nv;
                            done = true;
                            break;
                        }

                        openList.Add(nv);
                    }
                    if (done)
                        break;
                }
                if (done)
                    break;
            }

            float offsetX = (-blockSize * (map.Width) / 2f);
            float offsetY = (-blockSize * (map.Height) / 2f);

            var t = new GameObject("Port").transform;
            t.position = new Vector3(offsetX + ((map.Port.x + 1) * blockSize), offsetY + ((map.Port.y + 1) * blockSize), 0f);
            t.position -= t.position.normalized * 3f;
            t.position = new Vector3(t.position.x, t.position.y, 0f);
            return t;
        }

        Transform[] FindSpots(Map map)
        {
            List<Transform> spots = new List<Transform>();

            float offsetX = (-blockSize * map.Width / 2f);
            float offsetY = (-blockSize * map.Height / 2f);

            List<Vector2Int> openList = new List<Vector2Int>();
            List<Vector2Int> closedList = new List<Vector2Int>();

            openList.Add(new Vector2Int(map.Width / 2, map.Height / 2));

            Vector2Int curr = new Vector2Int();

            int iter = 0;
            int spacing = 1;
            while (openList.Count > 0 && iter < 1000)
            {
                iter++;
                curr = openList[0];
                openList.Remove(curr);
                closedList.Add(curr);

                if (map[curr.x, curr.y] == 1)
                {
                    bool tooCloseToWater = false;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            /*if (Mathf.Abs(i + j) != 1)
                                continue;*/

                            int px = curr.x + i;
                            int py = curr.y + j;

                            Vector2Int nv = new Vector2Int(px, py);

                            if (px < 0 || px >= map.Width || py < 0 || py >= map.Height || openList.Contains(nv) || closedList.Contains(nv))
                                continue;

                            if (map[px, py] == 0)
                            {
                                tooCloseToWater = true;
                                break;
                            }
                        }
                    }

                    if (!tooCloseToWater)
                    {
                        Transform t = new GameObject(spots.Count.ToString()).transform;

                        t.position = new Vector3(offsetX + ((curr.x + .5f) * blockSize), offsetY + ((curr.y + .5f) * blockSize), 0);

                        for (int i = -spacing; i <= spacing; i++)
                        {
                            for (int j = -spacing; j <= spacing; j++)
                            {
                                int px = curr.x + i;
                                int py = curr.y + j;

                                Vector2Int nv = new Vector2Int(px, py);

                                if (px < 0 || px >= map.Width || py < 0 || py >= map.Height || openList.Contains(nv) || closedList.Contains(nv))
                                    continue;

                                openList.Remove(nv);
                                closedList.Add(nv);
                            }
                        }

                        spots.Add(t);
                    }
                }

                bool done = false;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        int px = curr.x + i;
                        int py = curr.y + j;

                        Vector2Int nv = new Vector2Int(px, py);

                        if (px < 0 || px >= map.Width || py < 0 || py >= map.Height || openList.Contains(nv) || closedList.Contains(nv))
                            continue;


                        if (map[nv.x, nv.y] == 1)
                        {
                            openList.Insert(0, nv);
                            done = true;
                            break;
                        }
                        openList.Add(nv);
                    }
                    if (done)
                        break;
                }
            }

            return spots.ToArray();
        }

        void AddDecorations(Map map)
        {
            GameObject root = new GameObject("Decorations");
            root.transform.parent = transform;

            float offsetX = (-blockSize * (map.Width) / 2f);
            float offsetY = (-blockSize * (map.Height) / 2f);

            /*for (int i = 0; i < 50; i++)
            {

                int x = 0, y = 0;

                int iter = 0;
                int maxIter = 1000;
                while (map[x, y] == 0 && iter < maxIter)
                {
                    x = Random.Range(0, map.Width);
                    y = Random.Range(0, map.Height);
                    iter++;
                }

                map[x, y] = 0;


                var g = GameObject.Instantiate(_floorDecorations[0]);
                g.transform.position = new Vector3(offsetX + ((x + 1) * blockSize), offsetY + ((y + 1) * blockSize), 0);
                g.transform.parent = root.transform;

            }*/

            float scale = 3 * Mathf.PI;

            for (int x = 1; x < map.Width - 1; x++)
            {
                for (int y = 1; y < map.Height - 1; y++)
                {
                    if (map[x, y] == 0)
                        continue;

                    if (Mathf.PerlinNoise(scale * (x / (float)map.Width), scale * (y / (float)map.Height)) < 0.5f)
                        continue;

                    var g = GameObject.Instantiate(_floorDecorations[0]);
                    Vector2 variation = .25f * Random.insideUnitCircle;
                    g.transform.position = new Vector3(variation.x + offsetX + ((x + 1) * blockSize), variation.y + offsetY + ((y + 1) * blockSize), 0);
                    g.transform.parent = root.transform;
                }
            }
        }
    }


    Map ExtractIslandMap(ref int[,] world)
    {

        int width = world.GetLength(0);
        int height = world.GetLength(1);
        int[,] tiles = new int[width, height];

        int x = 0, y = 0;

        int iter = 0;
        int maxIter = 1000;
        while (world[x, y] == 0 && iter < maxIter)
        {
            x = Random.Range(0, width);
            y = Random.Range(0, height);
            iter++;
        }

        if (iter == maxIter)
            return null;

        List<Vector2Int> openList = new List<Vector2Int>();
        openList.Add(new Vector2Int(x, y));

        int maxNorth = 0;
        int maxSouth = height;
        int maxEast = 0;
        int maxWest = width;

        int counter = 0;
        while (openList.Count > 0)
        {
            counter++;

            Vector2Int v = openList[0];
            openList.Remove(v);

            world[v.x, v.y] = 0;
            tiles[v.x, v.y] = 1;

            maxNorth = System.Math.Max(maxNorth, v.y);
            maxSouth = System.Math.Min(maxSouth, v.y);
            maxEast = System.Math.Max(maxEast, v.x);
            maxWest = System.Math.Min(maxWest, v.y);


            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int px = v.x + i;
                    int py = v.y + j;

                    if (px < 0 || px >= width || py < 0 || py >= height)
                        continue;

                    if (world[px, py] == 1)
                    {
                        openList.Add(new Vector2Int(px, py));
                        world[px, py] = 0;
                    }
                }
            }
        }

        Map map = new Map(tiles, counter);

        map.MaxY = maxNorth;
        map.MinY = maxSouth;
        map.MaxX = maxEast;
        map.MinX = maxWest;

        return map;
    }

    public void GenerateBoat(Boat boat)
    {
        var g = GameObject.Instantiate(_boatPrefabs[Mathf.Min(_boatPrefabs.Length - 1, boat.Capacity - 1)], Vector3.zero, Quaternion.identity);
        boat.SetUp(g);
    }

    class Map
    {
        public int[,] Tiles { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Occupied { get; private set; }
        public int MaxY { get; internal set; }
        public int MinY { get; internal set; }
        public int MaxX { get; internal set; }
        public int MinX { get; internal set; }
        public GameObject GameObject { get; internal set; }

        public Vector2 Port;

        public Map(int[,] tiles)
        {
            Width = tiles.GetLength(0);
            Height = tiles.GetLength(1);
            Tiles = tiles;

            Occupied = 0;
            foreach (var tile in Tiles)
            {
                if (tile == 1)
                    Occupied++;
            }
        }

        public Map(int[,] tiles, int occupied)
        {
            Width = tiles.GetLength(0);
            Height = tiles.GetLength(1);
            Tiles = tiles;
            Occupied = occupied;
        }

        public int this[int i, int j]
        {
            get { return Tiles[i, j]; }
            set { Tiles[i, j] = value; }
        }
    }
}
