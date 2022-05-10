using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelMapLocation
{
    public int x;
    public int z;
    public TunnelMapLocation(int _x, int _z)
    {
        x = _x;
        z = _z;
    }
}

public class TunnelMaze : MonoBehaviour
{
    public List<TunnelMapLocation> directions = new List<TunnelMapLocation>() {
                                            new TunnelMapLocation(1,0),
                                            new TunnelMapLocation(0,1),
                                            new TunnelMapLocation(-1,0),
                                            new TunnelMapLocation(0,-1) };
    public int width = 30; //x length
    public int depth = 30; //z length
    public byte[,] map;
    public int scale = 6;

    public GameObject straight;
    public GameObject crossroad;
    public GameObject corner;
    public GameObject tIntersection;
    public GameObject endpiece;
    public GameObject endpieceWithRoom;
    public GameObject bridge;

    public bool isBridge = false;

    public int initialX = 0;
    public int initialY = 0;
    public int initialZ = 0;
    
    public Transform parent;
    private List<GameObject> navMeshElements = new List<GameObject>();


    //public GameObject FPC;

    // Start is called before the first frame update
    void Start()
    {
        InitialiseMap();
        Generate();

        /*for (int z = 0; z < depth - 1; z++)
        {
            for (int x = 0; x < width - 1; x++)
            { 
                if (Search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 1, 0, 1 })) 
                {
                    InitialiseMap();
                    Generate();
                }
            }
        }*/
        bool temp = true;

        while(map[8,6] == 1 || !Search2D(8, 6,  new int[] { 1, 0, 5, 5, 0, 1, 5, 1, 5 })) {  //vertical straight
            //Debug.Log(map[3, 4]);
            //Debug.Log(Search2D(3, 4, new int[] { 5, 1, 5, 0, 0, 0, 5, 1, 5 }));

            Debug.Log("bi da");
            InitialiseMap();
            Generate();
        }
        
        DrawMap();
        //PlaceFPS();
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
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (Random.Range(0, 100) < 50)
                    map[x, z] = 0;     //1 = wall  0 = corridor
            }
        }
    }

     public byte[,] SendMap()
    {
        return map;
    }
    
    public virtual void PlaceFPS()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 0)
                {
                    //FPC.transform.position = new Vector3(initialX + x * scale, initialY + 6, initialZ + z * scale);
                    return;
                }
            }
        }

        
        /*bool temp = true;
        while (temp)
        {
            int x = Random.Range(0, width);
            int z = Random.Range(0, depth);
            Debug.Log("x " + x + " z " +z + "  "  + map[x, z]);
            if (map[x, z] == 0)
            {
                FPC.transform.position = new Vector3( ( -1 * x * scale), initialY,  (z * scale ));
                Debug.Log("x  " +  (-1 * x * scale) + " z " +  (z * scale ));
                temp = false;
            }
        }*/

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
                    if (x + 1 <= width && z + 1 <= depth && z - 1 <= depth ) 
                    {
                        Debug.Log("horizontal end piece -|  " + "x " + x + " z "  + z);
                        if (map[x + 1, z - 1] == 1 && map[x + 1, z + 1] == 1)  
                        {
                            GameObject _block = Instantiate(endpieceWithRoom);
                            _block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                            _block.transform.SetParent(parent);
                        }
                    }
                    GameObject block = Instantiate(endpiece);
                    block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    block.transform.SetParent(parent);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 1, 5 })) //horizontal end piece |-
                {
                    Debug.Log("horizontal end piece |-  " + "x " + x + " z "  + z);
                    if (x - 1 <= width && z + 1 <= depth && z - 1 <= depth ) 
                    {
                        if (map[x - 1, z - 1] == 1 && map[x - 1, z + 1] == 1)  
                        {
                            GameObject _block = Instantiate(endpieceWithRoom);
                            _block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                            _block.transform.Rotate(0, 180, 0);
                            _block.transform.SetParent(parent);
                        }
                    }
                    GameObject block = Instantiate(endpiece);
                    block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    block.transform.Rotate(0, 180, 0);
                    block.transform.SetParent(parent);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 1, 5, 0, 5 })) //vertical end piece T
                {
                    Debug.Log("vertical end piece T  " + "x " + x + " z "  + z);
                        if (x + 1 <= width && x - 1 <= width && z + 1 <= depth ) 
                        {
                            if (map[x + 1, z + 1] == 1 && map[x - 1, z + 1] == 1)  
                            {
                                GameObject _block = Instantiate(endpieceWithRoom);
                                _block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                                _block.transform.Rotate(0, -90, 0);
                                _block.transform.SetParent(parent);
                            }
                        }
                        GameObject block = Instantiate(endpiece);
                        block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        block.transform.Rotate(0, -90, 0);
                        block.transform.SetParent(parent);
                    

                   
                }
                else if (Search2D(x, z, new int[] { 5, 0, 5, 1, 0, 1, 5, 1, 5 })) //vertical end piece upside downT
                {
                    if (x + 1 <= width && x - 1 <= width && z - 1 <= depth ) 
                    {
                        if (map[x + 1, z - 1] == 1 && map[x - 1, z - 1] == 1)  
                        {
                            GameObject _block = Instantiate(endpieceWithRoom);
                            _block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                            _block.transform.Rotate(0, 90, 0);
                            _block.transform.SetParent(parent);
                        }
                    }
                    Debug.Log("vertical end piece upside downT  " + "x " + x + " z "  + z);
                    GameObject block = Instantiate(endpiece);
                    block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    block.transform.Rotate(0, 90, 0);
                    block.transform.SetParent(parent);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 5, 1, 0, 1, 5, 0, 5 })) //vertical straight
                {
                    //if (isBridge == false && x == 1 && z == 7)
                    //{
                    //    Vector3 pos = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    //    GameObject go = Instantiate(bridge, pos, Quaternion.identity);
                    //    go.transform.Rotate(0, 90, 0);
                    //    Debug.Log("x  " + x + "  z  " + z);
                    //    isBridge = true;
                    //}
                    
                    Debug.Log("vertical straight  " + "x " + x + " z "  + z);
                        GameObject block = Instantiate(straight);
                    block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    block.transform.SetParent(parent);
                    
                    
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 5, 1, 5 })) //horizontal straight
                {
                    Debug.Log("horizontal straight " + "x " + x + " z "  + z);
                    GameObject block = Instantiate(straight);
                    block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    block.transform.Rotate(0, 90, 0);
                    block.transform.SetParent(parent);
                    
                    //Vector3 pos = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    //GameObject go = Instantiate(straight, pos, Quaternion.identity);
                    //go.transform.Rotate(0, 90, 0);
                    //go.transform.SetParent(parent);

                }
                else if (Search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 1, 0, 1 })) //crossroad
                {
                    GameObject go = Instantiate(crossroad);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    go.transform.SetParent(parent);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 1, 1, 0, 5 })) //upper left corner
                {
                    Debug.Log("upper left corner  " + "x " + x + " z "  + z);
                    GameObject go = Instantiate(corner);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    go.transform.Rotate(0, 180, 0);
                    go.transform.SetParent(parent);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 0, 1 })) //upper right corner
                {
                    Debug.Log("upper right corner  " + "x " + x + " z "  + z);
                    GameObject go = Instantiate(corner);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    go.transform.Rotate(0, 90, 0);
                    go.transform.SetParent(parent);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 1, 1, 0, 0, 5, 1, 5 })) //lower right corner
                {
                    Debug.Log("lower right corner  " + "x " + x + " z "  + z);
                    GameObject go = Instantiate(corner);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    go.transform.SetParent(parent);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 5, 5, 0, 1, 5, 1, 5 })) //lower left corner
                {
                    if (isBridge == false && x == 8 && z == 6)
                    {
                        Vector3 pos = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        GameObject go = Instantiate(bridge, pos, Quaternion.identity);
                        Debug.Log("x  " + x + "  z  " + z);
                        isBridge = true;
                    } else 
                    {
                        Debug.Log("lower left corner  " + "x " + x + " z "  + z);
                        GameObject go = Instantiate(corner);
                        go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        go.transform.Rotate(0, -90, 0);
                        go.transform.SetParent(parent);
                    }
                }
                else if (Search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 5, 1, 5 })) //tjunc  upsidedown T
                {
                    Debug.Log("tjunc  upsidedown T  " + "x " + x + " z "  + z);
                    GameObject go = Instantiate(tIntersection);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    go.transform.Rotate(0, -90, 0);
                    go.transform.SetParent(parent);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 1, 0, 1 })) //tjunc  T
                {
                    Debug.Log("tjunc   T  " + "x " + x + " z "  + z);
                    GameObject go = Instantiate(tIntersection);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    go.transform.Rotate(0, 90, 0);
                    go.transform.SetParent(parent);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 5, 0, 0, 1, 1, 0, 5 })) //tjunc  -|
                {
                    Debug.Log("tjunc  -|  " + "x " + x + " z "  + z);
                    GameObject go = Instantiate(tIntersection);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    go.transform.Rotate(0, 180, 0);
                    go.transform.SetParent(parent);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 1, 1, 0, 0, 5, 0, 1 })) //tjunc  |-
                {
                    Debug.Log("tjunc  |-  " + "x " + x + " z "  + z);
                    GameObject go = Instantiate(tIntersection);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    go.transform.SetParent(parent);
                }


            }
        //PlaceFPS();
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
