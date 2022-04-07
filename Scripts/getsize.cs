using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class getsize : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("Groot:");
        // Debug.Log("X: " + transform.localScale.x);
        // Debug.Log("Y: " + transform.localScale.y);
        // Debug.Log("Z: " + transform.localScale.z);
        Debug.Log("Hi :)");

        float width = GetComponent<SpriteRenderer>().bounds.size.x;
        string path = "Assets/Resources/test.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(width);
        writer.WriteLine("Hi :)");
        writer.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
