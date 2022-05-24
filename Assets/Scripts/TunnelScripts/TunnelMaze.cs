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
    public int tunnelFloor = 2;
    private int bridgeX;
    private int bridgeZ;
    private int[] bridgeArray;

    
    public Transform parent;
    private List<GameObject> navMeshElements = new List<GameObject>();


    //public GameObject FPC;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public byte[,] StartTunnelMaze(int tunnelFloor)
    {
        InitialiseMap();
        Generate();
        
        CreateBridge(tunnelFloor);

        while(map[bridgeX,bridgeZ] == 1 || !Search2D(bridgeX, bridgeZ,  bridgeArray)) {  
            InitialiseMap();
            Generate();
        }
        
        //DrawMap();
        //PlaceFPS();
        return map;
    }

    public void CreateBridge(int tunnelFloor) {
        if (tunnelFloor == 1) 
        {
            bridgeX = 8;
            bridgeZ = 8;
            bridgeArray = new int [] {5, 1, 5, 0, 0, 1, 1, 0, 5  };
        }
        if (tunnelFloor == 2) 
        {
            bridgeX = 8;
            bridgeZ = 6;
            bridgeArray = new int [] {1, 0, 5, 5, 0, 1, 5, 1, 5};
        } 
        if (tunnelFloor == 3) 
        {
            bridgeX = 8;
            bridgeZ = 1;
            bridgeArray = new int [] {1, 0, 5, 5, 0, 1, 5, 1, 5};
        }
        if (tunnelFloor == 4) 
        {
            bridgeX = 1;
            bridgeZ = 6;
            bridgeArray = new int [] {5, 1, 5, 1, 0, 0, 5, 0, 1};
        }
        if (tunnelFloor == 5) 
        {
            bridgeX = 1;
            bridgeZ = 6;
            bridgeArray = new int [] {5, 1, 5, 1, 0, 0, 5, 0, 1};
        }
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

     public byte[,] GetMap()
    {
        return map;
    }
    
    public void DrawMap()
    {   
        isBridge = false;
        Debug.Log("in maze");
                        Debug.Log("mapppp" + map[5,1] + " " + map[5,2] + " " + map[5,3] + " " + map[5,4] + " " + map[5,5] + " " + map[5,6] + " " + map[5,7] + " " + map[5,8]);

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
                    Debug.Log("tunnel horizontal end piece -|");
                    if (x + 1 <= width && z + 1 <= depth && z - 1 <= depth ) 
                    {
                        if (map[x + 1, z - 1] == 1 && map[x + 1, z + 1] == 1)  
                        {
                            GameObject _block = Instantiate(endpieceWithRoom);
                            _block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        }
                    }
                    GameObject block = Instantiate(endpiece);
                    block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 1, 5 })) //horizontal end piece |-
                {
                    if (x - 1 <= width && z + 1 <= depth && z - 1 <= depth ) 
                    {
                        if (map[x - 1, z - 1] == 1 && map[x - 1, z + 1] == 1)  
                        {
                            GameObject _block = Instantiate(endpieceWithRoom);
                            _block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                            _block.transform.Rotate(0, 180, 0);
                        }
                    }
                    GameObject block = Instantiate(endpiece);
                    block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    block.transform.Rotate(0, 180, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 1, 5, 0, 5 })) //vertical end piece T
                {
                        if (x + 1 <= width && x - 1 <= width && z + 1 <= depth ) 
                        {
                            if (map[x + 1, z + 1] == 1 && map[x - 1, z + 1] == 1)  
                            {
                                GameObject _block = Instantiate(endpieceWithRoom);
                                _block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                                _block.transform.Rotate(0, -90, 0);
                            }
                        }
                        GameObject block = Instantiate(endpiece);
                        block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        block.transform.Rotate(0, -90, 0);
                    

                   
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
                        }
                    }
                    GameObject block = Instantiate(endpiece);
                    block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    block.transform.Rotate(0, 90, 0);
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
                    
                    GameObject block = Instantiate(straight);
                    block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    
                    
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 5, 1, 5 })) //horizontal straight
                {
                    GameObject block = Instantiate(straight);
                    block.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    block.transform.Rotate(0, 90, 0);
                    
                    //Vector3 pos = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                    //GameObject go = Instantiate(straight, pos, Quaternion.identity);
                    //go.transform.Rotate(0, 90, 0);

                }
                else if (Search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 1, 0, 1 })) //crossroad
                {
                    GameObject go = Instantiate(crossroad);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 1, 1, 0, 5 })) //upper left corner
                {
                    if (tunnelFloor == 1 && isBridge == false && x == bridgeX && z == bridgeZ)
                    {
                        Vector3 pos = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        GameObject go = Instantiate(bridge, pos, Quaternion.identity);
                        go.transform.Rotate(0, 270, 0);
                        isBridge = true;
                    }
                     else 
                    {
                        GameObject go = Instantiate(corner);
                        go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        go.transform.Rotate(0, 180, 0);
                    }
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 0, 1 })) //upper right corner
                {
                    if (tunnelFloor == 4 && isBridge == false && x == bridgeX && z == bridgeZ)
                    {
                        Vector3 pos = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        GameObject go = Instantiate(bridge, pos, Quaternion.identity);
                        go.transform.Rotate(0, 180, 0);
                        isBridge = true;
                    } 
                    else if (tunnelFloor == 5 && isBridge == false && x == bridgeX && z == bridgeZ)
                    {
                        Vector3 pos = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        GameObject go = Instantiate(bridge, pos, Quaternion.identity);
                        go.transform.Rotate(0, 180, 0);
                        isBridge = true;
                    } 
                    else {
                        GameObject go = Instantiate(corner);
                        go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        go.transform.Rotate(0, 90, 0);
                    }
                }
                else if (Search2D(x, z, new int[] { 5, 0, 1, 1, 0, 0, 5, 1, 5 })) //lower right corner
                {
                    GameObject go = Instantiate(corner);
                    go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 5, 5, 0, 1, 5, 1, 5 })) //lower left corner
                {
                    if (tunnelFloor == 2 && isBridge == false && x == bridgeX && z == bridgeZ)
                    {
                        Vector3 pos = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        GameObject go = Instantiate(bridge, pos, Quaternion.identity);
                        isBridge = true;
                    } 
                    else if ( tunnelFloor == 3 && isBridge == false && x == bridgeX && z == bridgeZ)
                    {
                        Vector3 pos = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        GameObject go = Instantiate(bridge, pos, Quaternion.identity);
                        go.transform.Rotate(0, -90, 0);
                        isBridge = true;
                    }
                     else 
                    {
                        GameObject go = Instantiate(corner);
                        go.transform.position = new Vector3(initialX + x * scale, initialY, initialZ + z * scale);
                        go.transform.Rotate(0, -90, 0);
                    }
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
