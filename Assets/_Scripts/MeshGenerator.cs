using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{


    public static GameObject GenerateMesh(int[,] tiles, float blockSize, Material worldMaterial)
    {
        Vector2[] _floorUVs;
        Vector2[] _wallUVs;

        float _w;
        float _h;

        GenerateUVs();

        GameObject g = new GameObject();
        MeshRenderer meshRenderer = g.AddComponent<MeshRenderer>();
        meshRenderer.material = worldMaterial;
        MeshFilter meshFilter = g.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.name = "Custom island";

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int columns = tiles.GetLength(0);
        int rows = tiles.GetLength(1);

        float offsetX = (-blockSize * columns / 2f);
        float offsetY = (-blockSize * rows / 2f);

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (tiles[i, j] >= 1)
                {
                    AddFloor(i, j);
                    AddWalls(i, j);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();

        meshFilter.mesh = mesh;

        MeshCollider meshCollider = g.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        return g;

        void AddFloor(int x, int y)
        {

            int vertCount = vertices.Count;

            Vector3 a = new Vector3(offsetX + (x * blockSize), offsetY + (y * blockSize), 0);
            Vector3 b = new Vector3(offsetX + (x * blockSize) + blockSize, offsetY + (y * blockSize), 0);
            Vector3 c = new Vector3(offsetX + (x * blockSize) + blockSize, offsetY + (y * blockSize) + blockSize, 0);
            Vector3 d = new Vector3(offsetX + (x * blockSize), offsetY + (y * blockSize) + blockSize, 0);

            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);
            vertices.Add(d);

            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);

            int idA = vertCount;
            int idB = vertCount + 1;
            int idC = vertCount + 2;
            int idD = vertCount + 3;

            triangles.Add(idA);
            triangles.Add(idC);
            triangles.Add(idB);

            triangles.Add(idA);
            triangles.Add(idD);
            triangles.Add(idC);

            Vector2 uv00 = _floorUVs[Mathf.FloorToInt(Mathf.Pow(Random.value, 3) * 0.99f * _floorUVs.Length)];
            Vector2 uv10 = new Vector2(uv00.x + _w, uv00.y);
            Vector2 uv11 = new Vector2(uv00.x + _w, uv00.y + _h);
            Vector2 uv01 = new Vector2(uv00.x, uv00.y + _h);

            uvs.Add(uv00);
            uvs.Add(uv10);
            uvs.Add(uv11);
            uvs.Add(uv01);

        }

        void AddWalls(int x, int y)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (Mathf.Abs(i + j) != 1)// Avoid diagonals
                        continue;

                    int xi = x + i;
                    int yj = y + j;

                    if (xi < 0 || xi >= columns || yj < 0 || yj >= rows || tiles[xi, yj] == 0)
                    {
                        AddWall(x, y, new Vector3Int(i, j, 0));
                    }

                }
            }
        }

        void AddWall(int x, int y, Vector3Int normal)
        {
            // var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //go.name = x + ", " + y;
            //go.transform.position = new Vector3(offsetX + (x * scale), offsetY + (y * scale), 0);

            Vector3 right = new Vector3(-normal.y, normal.x, 0);

            float posX = offsetX + (x * blockSize) + Mathf.Clamp01(normal.x) * blockSize + Mathf.Clamp01(right.x) * blockSize - right.x * blockSize;
            float posY = offsetY + (y * blockSize) + Mathf.Clamp01(normal.y) * blockSize + Mathf.Clamp01(right.y) * blockSize - right.y * blockSize;

            int vertCount = vertices.Count;

            Vector3 a = new Vector3(posX, posY, blockSize);
            Vector3 b = new Vector3(posX + (blockSize * right.x), posY + (blockSize * right.y), blockSize);
            Vector3 c = new Vector3(b.x, b.y, 0);
            Vector3 d = new Vector3(posX, posY, 0);

            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);
            vertices.Add(d);

            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);

            Vector2 uv00 = _wallUVs[Mathf.FloorToInt(Mathf.Pow(Random.value, .4f) * 0.99f * _wallUVs.Length)];
            Vector2 uv10 = new Vector2(uv00.x + _w, uv00.y);
            Vector2 uv11 = new Vector2(uv00.x + _w, uv00.y + _h);
            Vector2 uv01 = new Vector2(uv00.x, uv00.y + _h);

            uvs.Add(uv00);
            uvs.Add(uv10);
            uvs.Add(uv11);
            uvs.Add(uv01);

            int idA = vertCount;
            int idB = vertCount + 1;
            int idC = vertCount + 2;
            int idD = vertCount + 3;

            triangles.Add(idA);
            triangles.Add(idC);
            triangles.Add(idB);

            triangles.Add(idA);
            triangles.Add(idD);
            triangles.Add(idC);
        }

        void GenerateUVs()
        {
            _floorUVs = new Vector2[9];
            int columns = 4;
            int rows = 4;

            _w = 1f / columns;
            _h = 1f / rows;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    _floorUVs[i + (3 * (j - 1))] = new Vector2(_w * i, _h * j);
                }
            }

            _wallUVs = new Vector2[2];
            for (int i = 0; i < 2; i++)
            {
                _wallUVs[i] = new Vector2(_w * i, 0);
            }
        }
    }
}
