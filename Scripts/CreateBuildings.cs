using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CreateBuildings : MonoBehaviour
{
    public const int BUILDING = 0;
    public const int ADDON = 9;
    public const int ROOF = 0;
    public const int EMPTY = 1;
    public const int FREE_WALL = 2;
    public const int WALL = 3;
    public const int DOOR = 4;
    public const int GROUND = 5;



    Renderer rend;
    public static GameObject[] myObjects;

    struct size{
        public float x, y, z;

        public size(float x, float y, float z){
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    struct prefabs_info{
        public int id;
        public int type;
        public size size;
        public int[] edges;

        public prefabs_info(int id, int type, size size, int[] edges) {
            this.id = id;
            this.type = type;
            this.size = size;
            this.edges = edges;
        }
    };

    // prefabs_info [] test = new prefabs_info[4];
    // test[0] = new prefabs_info {id = 1, type = BUILDING, size = new size{x = 2, y = 3, z = 4}, edges = new int[6]{1, 2, 3, 4, 5, 6}};


    //{top, front, left, back, right, bottom} || ROOF EMPTY FREE_WALL WALL DOOR
    prefabs_info [] main_content = new prefabs_info [] 
    {
        new prefabs_info {id = 0, type = BUILDING, size = new size{x = 5.45181f, y = 3, z = 4.098126f}, edges = new int[6]{ROOF, DOOR, FREE_WALL, EMPTY, FREE_WALL, GROUND}},
        new prefabs_info {id = 1, type = BUILDING, size = new size{x = 5.45181f, y = 3, z = 4.098126f}, edges = new int[6]{ROOF, DOOR, FREE_WALL, EMPTY, FREE_WALL, GROUND}}
        // new prefabs_info {id = 2, type = ADDON, size = new size{}}
    };

    void Start()
    {
        int id = 0;
        rend = GetComponent<Renderer>();
        myObjects = Resources.LoadAll<GameObject>("Buildings/Prefabs");
        Instantiate(myObjects[main_content[id].id], new Vector3(0,50,0), Quaternion.identity);
        Debug.Log(main_content[id].size.x + " " + main_content[id].size.y + " " + main_content[id].size.z);

        string path = "Assets/Resources/test.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Test");
        writer.Close();
    }
    
    void Update()
    {
        
    }
}
