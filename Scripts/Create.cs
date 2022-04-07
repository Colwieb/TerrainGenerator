using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : MonoBehaviour
{
    public int width = 256; // Links
    public int height = 256; // Omhoog
    public int depth = 20;
    public float scale = 20f;
    public float spread = 25f;
    public float cityHeight = 0.5f;
    public float Inhoud = 50f;
    [HideInInspector] public float offsetX;
    [HideInInspector] public float offsetY;
    public TerrainData filter;
    struct cord{
        public int x, y;
    };
    private cord p1;
    private cord p2;
    private cord first;
    private int dx;
    private int dy;
    private int max;
    int abs(int input){
        if (input > 0) return input;
        else return input * -1;
    }

    void Start(){
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
        Debug.Log("Runtime: " + Time.realtimeSinceStartup);
    }

    TerrainData GenerateTerrain (TerrainData terrainData){
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        float[,] mainTerrain = new float[width, height];
        mainTerrain = GenerateHeights(2);

        // Genereer extra basisfilters voor het terrein (telt voor 1/x mee)
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);

        float[,] filter = new float[width, height];
        filter = GenerateHeights(4);

        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);

        float[,] filter2 = new float[width, height];
        filter2 = GenerateHeights(8);
        
        //Genereer stad/dorp in boolean map (0 = terrein; 1 = stad/dorp)
        float[,] cityTerrain = new float[width, height];
        GetCity(220, 220, 1000, ref cityTerrain);
        GetCity(180, 100, 1600, ref cityTerrain);
        GetCity(50, 50, 2600, ref cityTerrain);
        GetCity(100, 200, 2600, ref cityTerrain);

        // Genereer olievlek (overvloeien platte vlak stad met landschap)
        int x, y;
        for (x = 0; x < width; x++){
            float PreviousValue = 0;
            for (y = 0; y < height; y++){
                if (PreviousValue > 0){
                    if (cityTerrain[x, y] == 0 && PreviousValue + 1 < spread || cityTerrain[x, y] > PreviousValue + 1){
                        cityTerrain[x, y] = PreviousValue + 1;
                    }
                }
                PreviousValue = cityTerrain[0 + x, 0 + y];
            }

            PreviousValue = 0;
            for (y = y - 1; y >= 0; y--){
                if (PreviousValue > 0){
                    if (cityTerrain[x, y] == 0 && PreviousValue + 1 < spread || cityTerrain[x, y] > PreviousValue + 1){
                        cityTerrain[x, y] = PreviousValue + 1;
                    }
                }
                PreviousValue = cityTerrain[0 + x, 0 + y];
            }
        }

        for (y = 0; y < height; y++){
            float PreviousValue = 0;
            for (x = 0; x < width; x++){
                if (PreviousValue > 0){
                    if (cityTerrain[x, y] == 0 && PreviousValue + 1 < spread || cityTerrain[x, y] > PreviousValue + 1){
                        cityTerrain[x, y] = PreviousValue + 1;
                    }
                }
                PreviousValue = cityTerrain[0 + x, 0 + y];
            }

            PreviousValue = 0;
            for (x = x - 1; x >= 0; x--){
                if (PreviousValue > 0){
                    if (cityTerrain[x, y] == 0 && PreviousValue + 1 < spread || cityTerrain[x, y] > PreviousValue + 1){
                        cityTerrain[x, y] = PreviousValue + 1;
                    }
                }
                PreviousValue = cityTerrain[0 + x, 0 + y];
            }
        }

        // Schrijf de gekregen gegevens om naar een factor (0 <= f <= 1)
        // Formule: 0.5 * sin(x - 0.5π) + 0.5
        float[] factorValues = new float[(int)spread + 1];
        for (int i = 0; i <= (int)spread; i++){
            factorValues[i] = 0.5f * Mathf.Sin(i / spread * Mathf.PI - 0.5f * Mathf.PI) + 0.5f;
        }

    	// Schrijf het cityterrain over naar een filter
        for (x = 0; x < width; x++){
            for (y = 0; y < height; y++){
                if (cityTerrain[x, y] > 1) cityTerrain[x, y] = factorValues[(int)cityTerrain[x, y]];
                else if (cityTerrain[x, y] == 1) cityTerrain[x, y] = 0;
                else cityTerrain[x, y] = 1;
            }
        }

        // Combine filters
        for (x = 0; x < width; x++){
            for (y = 0; y < height; y++){
                mainTerrain[x, y] += filter[x, y] + filter2[x, y];
                mainTerrain[x, y] *= cityTerrain[x, y];
                if (cityTerrain[x, y] != 1){
                    mainTerrain[x, y] += (1f - cityTerrain[x, y]) * cityHeight;
                }
            }
        }

        terrainData.SetHeights(0, 0, mainTerrain);

        return terrainData;
    }
    
    void GetCity(int centerx, int centery, int Inhoud, ref float[,] cityTerrain){
        float AverageLength = (float)Mathf.Sqrt(Inhoud / Mathf.PI);
        float DiffLength = 0f;
        if (AverageLength > 25f) DiffLength = 16f;
        else if (AverageLength > 18f) DiffLength = 12f;
        else if (AverageLength > 10f) DiffLength = 5f;
        cord[] riveroutput = new cord[60];
        float firstlength = 0f;

        for (int i = 0; i <= 60; i++){ // Bereken 60 verschillende coordinaten
            float length = 0f;
            int index = 0;
            if (i < 60){
                // Bereken x, y positie randpunten
                index = i;
                length = AverageLength - DiffLength + CalculateHeight(0, i, 25f) * DiffLength * 2f;
                if (i > 50) length = (firstlength - length) / 10 * (i - 50) + length;
                riveroutput[i] = new cord {x = centerx + (int)(Mathf.Cos(2f * Mathf.PI / 60f * (float)i) * length), y = centery + (int)(Mathf.Sin(2f * Mathf.PI / 60f * (float)i) * length)};
            }else index = 0;

            if (i > 0){
                // Functie bepaling rand stad/dorp
                max = 0;
                dx = riveroutput[i - 1].x - riveroutput[index].x;
                dy = riveroutput[i - 1].y - riveroutput[index].y;
                if (abs(dx) < abs(dy)) max = abs(dy);
                else max = abs(dx);
                for (int n = 0; n <= max + 1; n++) cityTerrain[CheckX(riveroutput[index].x + (int)Mathf.Round(((float)dx / (float)max * (float)n))), CheckY(riveroutput[index].y + (int)Mathf.Round(((float)dy / (float)max * (float)n)))] = 1;
            }else firstlength = length;
        }

        // Genereer boolean map voor plaatsbepaling stad
        int[] diffX = new int[4]{0, 1, 0, -1};
        int[] diffY = new int[4]{1, 0, -1, 0};
        float[,] test = new float[width, height];
        test = cityTerrain;
        void Rec(int x, int y){
            if (x <= 0 || y <= 0 || x >= width || y >= height) return;
            for (int i = 0; i < 4; i++){
                if (test[x + diffX[i], y + diffY[i]] == 0){
                    test[x + diffX[i], y + diffY[i]] = 1;
                    Rec(x + diffX[i], y + diffY[i]);
                }
            }
        }
        Rec(centerx, centery); // Recursieve functie voor opvulling stad terrein.

        GenerateRiver(riveroutput, ref cityTerrain);
        cityTerrain = test;
    }

    int CheckX (int input){
        if (input < 0) return 0;
        if (input >= width) return width - 1;
        return input;
    }

    int CheckY (int input){
        if (input < 0) return 0;
        if (input >= height) return height - 1;
        return input;
    }

    float[,] GenerateHeights(float factor){
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++){
                heights[x, y] = CalculateHeight(x, y, scale) / factor;
            }
        }

        return heights;
    }
    float CalculateHeight(int x, int y, float scale){
        float xCoord, yCoord;
        xCoord = (float)x / width * scale + offsetX;
        yCoord = (float)y / height * scale + offsetY;
        
        return Mathf.PerlinNoise(xCoord, yCoord); 
    }

    float[,] GenerateRiver(cord[] city1, ref float[,] city){
        float[,] output = new float[width, height];
        //sixty boundary points of the city
        // a river will be generated between these two points
        int riverpoint1 = Random.Range(0,59);
        int riverpoint2 = (riverpoint1 + Random.Range(20,39))%60; //may not overflow the array of boundary points

        



        return output;
    }

}