using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeMapLocation
{
    public int x;
    public int z;

    public PipeMapLocation(int _x, int _z)
    {
        x = _x;
        z = _z;
    }
}

public class PipeMaze : MonoBehaviour
{
    public List<PipeMapLocation> directions = new List<PipeMapLocation>() {
                                            new PipeMapLocation(1,0),
                                            new PipeMapLocation(0,1),
                                            new PipeMapLocation(-1,0),
                                            new PipeMapLocation(0,-1) };
    public int width = 30; //x length
    public int depth = 30; //z length
    public byte[,] map;
    public int scale = 6;

    public GameObject straight;
    public GameObject crossroad;
    public GameObject corner;
    public GameObject tIntersection;
    public GameObject endpiece;
    public GameObject up;
    private bool isUp = false;
    public int initialX = 0;
    public int initialY = 0;
    public int initialZ = 0;
    //private ZombieSpawner zombieSpawner;

    public GameObject FPC;

    // Start is called before the first frame update
    void Start()
    {
        InitialiseMap();
        Generate();

        
        bool temp = true;
        while(map[3, 4] == 1 || !Search2D(3, 4, new int[] { 5, 1, 5, 0, 0, 0, 5, 1, 5 })) {
            //Debug.Log(map[3, 4]);
            //Debug.Log(Search2D(3, 4, new int[] { 5, 1, 5, 0, 0, 0, 5, 1, 5 }));

               //Debug.Log("bi da");
                InitialiseMap();
                Generate();
        }

        //zombieSpawner = GetComponent<ZombieSpawner>();
        

        DrawMap();
        //PlaceZombies();
        PlaceFPS();
    }

    public byte[,] SendMap()
    {
        return map;
    }

    void InitialiseMap()
    {
        map = new byte[width, depth];
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                map[x, z] = 1;     //1 = wall  0 = corridor
            }
    }

    public virtual void Generate()
    {
        for (int z = 0; z < depth; z++) { 
            for (int x = 0; x < width; x++)
            {
                if (Random.Range(0, 100) < 50)
                    map[x, z] = 0;     //1 = wall  0 = corridor
            }
        }
    }

    public virtual void PlaceFPS()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 0)
                {
                    FPC.transform.position = new Vector3(initialX + 1+ (x * scale), initialY , initialZ + 1 +(z * scale));
                    return;
                }
            }
        }
    }

    public void PlaceZombies()
    {
        Debug.Log("PlaceZombies deyim");
   //     zombieSpawner.GetComponent<ZombieSpawner>().girdi();
       // zombieSpawner.SpawnZombieInMaze();
        
    }

    void DrawMap()
    {
        Debug.Log("in maze");
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 1)
                {
                    //Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    //GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //wall.transform.localScale = new Vector3(scale, scale, scale);
                    // wall.transform.position = pos;
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 1, 5, 1, 5 })) //horizontal end piece -|
                {
                    GameObject block = Instantiate(endpiece);
                    block.transform.SetParent(gameObject.transform.Find("Maze"));
                    block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 1, 5 })) //horizontal end piece |-
                {
                    GameObject block = Instantiate(endpiece);
                    block.transform.SetParent(gameObject.transform.Find("Maze"));
                    block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    block.transform.Rotate(0, 180, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 1, 5, 0, 5 })) //vertical end piece T
                {
                    GameObject block = Instantiate(endpiece);
                    block.transform.SetParent(gameObject.transform.Find("Maze"));
                    block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    block.transform.Rotate(0, -90, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 5, 1, 0, 1, 5, 1, 5 })) //vertical end piece upside downT
                {
                    GameObject block = Instantiate(endpiece);
                    block.transform.SetParent(gameObject.transform.Find("Maze"));
                    block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    block.transform.Rotate(0, 90, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 5, 1, 0, 1, 5, 0, 5 })) //vertical straight
                {
                    Vector3 pos = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    Instantiate(straight, pos, Quaternion.identity);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 5, 1, 5 })) //horizontal straight
                {
                    if (isUp == false && x == 3 && z == 4)
                    {
                        Vector3 pos = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        GameObject go = Instantiate(up, pos, Quaternion.identity);
                        go.transform.Rotate(0, 90, 0);
                        Debug.Log("x  " + x + "  z  " + z);
                        isUp = true;
                    }
                    else
                    {
                        Vector3 pos = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        GameObject go = Instantiate(straight, pos, Quaternion.identity);
                        go.transform.Rotate(0, 90, 0);
                    }
                        
                }
                else if (Search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 1, 0, 1 })) //crossroad
                {
                    GameObject go = Instantiate(crossroad);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 1, 1, 0, 5 })) //upper left corner
                {
                    GameObject go = Instantiate(corner);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    go.transform.Rotate(0, 180, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 0, 1 })) //upper right corner
                {
                    GameObject go = Instantiate(corner);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    go.transform.Rotate(0, 90, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 1, 1, 0, 0, 5, 1, 5 })) //lower right corner
                {
                    GameObject go = Instantiate(corner);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 5, 5, 0, 1, 5, 1, 5 })) //lower left corner
                {
                    GameObject go = Instantiate(corner);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    go.transform.Rotate(0, -90, 0);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 5, 1, 5 })) //tjunc  upsidedown T
                {
                    GameObject go = Instantiate(tIntersection);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    go.transform.Rotate(0, -90, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 1, 0, 1 })) //tjunc  T
                {
                    GameObject go = Instantiate(tIntersection);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    go.transform.Rotate(0, 90, 0);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 5, 0, 0, 1, 1, 0, 5 })) //tjunc  -|
                {
                    GameObject go = Instantiate(tIntersection);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    go.transform.Rotate(0, 180, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 1, 1, 0, 0, 5, 0, 1 })) //tjunc  |-
                {
                    GameObject go = Instantiate(tIntersection);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                }


            }
    }

    public bool Search2D(int c, int r, int[] pattern)
    {
        int count = 0;
        int pos = 0;
        for (int z = 1; z > -2; z--)
        {
            for (int x = -1; x < 2; x++)
            {
                if (pattern[pos] == map[c + x, r + z] || pattern[pos] == 5)
                    count++;
                pos++;
            }
        }
        return (count == 9);
    }

    public int CountSquareNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z] == 0) count++;
        if (map[x + 1, z] == 0) count++;
        if (map[x, z + 1] == 0) count++;
        if (map[x, z - 1] == 0) count++;
        return count;
    }

    public int CountDiagonalNeighbours(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z - 1] == 0) count++;
        if (map[x + 1, z + 1] == 0) count++;
        if (map[x - 1, z + 1] == 0) count++;
        if (map[x + 1, z - 1] == 0) count++;
        return count;
    }

    public int CountAllNeighbours(int x, int z)
    {
        return CountSquareNeighbours(x, z) + CountDiagonalNeighbours(x, z);
    }
}
