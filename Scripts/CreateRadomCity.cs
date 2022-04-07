// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Create : MonoBehaviour
// {
//     public int width = 256;
//     public int height = 256;
//     public int depth = 20;
//     public float scale = 20f;
//     public float spread = 25f;
//     public float cityHeight = 0.5f;
//     public float Inhoud = 50f;
//     [HideInInspector] public float offsetX;
//     [HideInInspector] public float offsetY;
//     public TerrainData filter;
//     struct cord{
//         public int y, x;
//     };
//     private cord p1;
//     private cord p2;

//     // void SetLine(cord p1, cord p2);

//     int abs(int input){
//         if (input > 0) return input;
//         else return input * -1;
//     }

//     // void SetLine(cord p1, cord p2){
//     //     int dx = p2.x - p1.x;
//     //     int dy = p2.y - p1.y;
//     //     int max = 0;
//     //     if (abs(dx) < abs(dy)) max = abs(dy);
//     //     else max = abs(dx); 
//     //     for (int i = 0; i <= max; i++){
//     //         input[p1.y + (int)round(((float)dy / (float)max * (float)i))][p1.x + (int)round(((float)dx / (float)max * (float)i))] = 1;
//     //     }
//     // }

//     void Start(){
//         offsetX = Random.Range(0f, 9999f);
//         offsetY = Random.Range(0f, 9999f);
//         Terrain terrain = GetComponent<Terrain>();
//         terrain.terrainData = GenerateTerrain(terrain.terrainData);
//     }

//     TerrainData GenerateTerrain (TerrainData terrainData){
//         terrainData.heightmapResolution = width + 1;
//         terrainData.size = new Vector3(width, depth, height);
//         float[,] mainTerrain = new float[width, height];
//         mainTerrain = GenerateHeights(2, false);

//         float[] lengths = new float[60];
//         float BaseLength = Mathf.Sqrt(Inhoud / Mathf.PI) - 12;
//         for (int i = 0; i < 60; i++){
//             float height = BaseLength + CalculateHeight(0, i, false, 20f) * 24;
//             if (i > 50) height = (lengths[0] - height) / 10 * (i - 50) + height;
//             lengths[i] = height;
//             p2.x = p1.x;
//             p2.y = p1.y;
//             p1.x = 100 + (int)(Mathf.Cos(2f * Mathf.PI / 60f * (float)i) * lengths[i]);
//             p1.y = 100 + (int)(Mathf.Sin(2f * Mathf.PI / 60f * (float)i) * lengths[i]);

//             if (i > 0){
//                 int dx = p2.x - p1.x;
//                 int dy = p2.y - p1.y;
//                 int max = 0;
//                 if (abs(dx) < abs(dy)) max = abs(dy);
//                 else max = abs(dx); 
//                 for (int n = 0; n <= max; n++){
//                     mainTerrain[p1.y + (int)Mathf.Round(((float)dy / (float)max * (float)n)), p1.x + (int)Mathf.Round(((float)dx / (float)max * (float)n))] = 1;
//                 }
//             }
//         }
//         // p2.x = p1.x;
//         // p2.y = p1.y;
//         // p1.x = 100 + (int)(Mathf.Cos(2f * Mathf.PI / 60f * 0) * lengths[0]);
//         // p1.y = 100 + (int)(Mathf.Sin(2f * Mathf.PI / 60f * 0) * lengths[0]);
//         // int dx = p2.x - p1.x;
//         // int dy = p2.y - p1.y;
//         // int max = 0;
//         // if (abs(dx) < abs(dy)) max = abs(dy);
//         // else max = abs(dx); 
//         // for (int n = 0; n <= max; n++){
//         //     mainTerrain[p1.y + (int)Mathf.Round(((float)dy / (float)max * (float)n)), p1.x + (int)Mathf.Round(((float)dx / (float)max * (float)n))] = 1;
//         // }

//         terrainData.SetHeights(0, 0, mainTerrain);

//         return terrainData;
//     }

//     float[,] GenerateHeights(float factor, bool city){
//         float[,] heights = new float[width, height];
//         for (int x = 0; x < width; x++){
//             for (int y = 0; y < height; y++){
//                 heights[x, y] = CalculateHeight(x, y, city, scale) / factor;
//             }
//         }

//         return heights;
//     }
    
//     float CalculateHeight(int x, int y, bool city, float scale){
//         float xCoord, yCoord;

//         if (!city){
//             xCoord = (float)x / width * scale + offsetX;
//             yCoord = (float)y / height * scale + offsetY;
//         }else{
//             xCoord = (float)x / width * 2 + offsetX;
//             yCoord = (float)y / height * 2 + offsetY;
//         }
        
//         return Mathf.PerlinNoise(xCoord, yCoord); 
//     }
// }