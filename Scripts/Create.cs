using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : MonoBehaviour
{
    cord [] riverpoints = new cord[100];
    cord [] city = new cord [] {
        new cord {x=0, y=0},
        new cord {x=5, y=8}
    };

    public float riverperiod = 25f;
    public float riverOffsetFactor = 10f;
    public int width = 256;
    public int height = 256;
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
    }

    TerrainData GenerateTerrain (TerrainData terrainData){
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        float[,] mainTerrain = new float[width, height];
        mainTerrain = GenerateHeights(2, false);

        // Genereer extra basisfilters voor het terrein (telt voor 1/x mee)
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);

        float[,] filter = new float[width, height];
        filter = GenerateHeights(4, false);

        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);

        float[,] filter2 = new float[width, height];
        filter2 = GenerateHeights(8, false);
        
        //Genereer stad/dorp (0 = terrein; 1 = stad/dorp)
        float[,] cityTerrain = new float[width, height];
        //GetCity(x middelpunt vd stad, y middelpunt vd stad, inhoud, referentie naar terrein van stad)
        GetCity(220, 220, 1000, ref cityTerrain);
        GetCity(100, 100, 1600, ref cityTerrain);
        GetCity(200, 50, 2600, ref cityTerrain);

        // Genereer olievlek met veel te veel if-statements.....
        int[] diffX = new int[4]{0, 1, 0, -1};
        int[] diffY = new int[4]{1, 0, -1, 0};
        int x, y;
        for (y = 0; y < height; y++){
            for (x = 0; x < width; x++){
                if (cityTerrain[x, y] == 1){
                    for (int r = 0; r < 4; r++){ // Alle windrichtingen nagaan om afstanden tot stad te updaten.
                        for (int l = 0; l < spread; l++){ // Update over een bepaalde afstand (grootte van de overgang)
                            for (int i = -l; i <= l; i++){
                                // Genereer positie van aan te passen
                                int dx, dy;
                                if (diffX[r] != 0) dx = x + diffX[r] * i;
                                else dx = x + l * diffY[r];
                                if (diffY[r] != 0) dy = y + diffY[r] * i;
                                else dy = y + l * diffX[r];
                            
                                if (dy >= 0 && dy < height && dx >= 0 && dx < width){ // Controleer of de positie binnen het terrein valt
                                    if (cityTerrain[dx, dy] > l || cityTerrain[dx, dy] == 0) cityTerrain[dx, dy] = l + 1; // Enkel aanpassen als de afstand omlaag moet
                                }
                            }
                        }
                    }
                }
            }
        }

        // Schrijf de gekregen gegevens om naar een factor (0 <= f <= 1)
        // Formule: 0.5 * sin(x - 0.5π) + 0.5
        for (x = 0; x < width; x++){
            for (y = 0; y < height; y++){
                if (cityTerrain[x, y] > 1) cityTerrain[x, y] = 0.5f * Mathf.Sin(cityTerrain[x, y] / spread * Mathf.PI - 0.5f * Mathf.PI) + 0.5f;
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
        // float[,] cityTerrain = new float[width, height];
        float AverageLength = (float)Mathf.Sqrt(Inhoud / Mathf.PI);
        float DiffLength = 0f;
        if (AverageLength > 25f) DiffLength = 16f;
        else if (AverageLength > 18f) DiffLength = 12f;
        else if (AverageLength > 10f) DiffLength = 5f;
        float firstlength = 0f;

        for (int i = 0; i <= 60; i++){ // Bereken 60 verschillende coordinaten
            float length = 0f;
            p2.x = p1.x;
            p2.y = p1.y;
            if (i < 60){
                // Bereken x, y positie randpunten
                length = AverageLength - DiffLength + CalculateHeight(0, i, false, 25f) * DiffLength * 2f;
                if (i > 50) length = (firstlength - length) / 10 * (i - 50) + length;
                p1.x = centerx + (int)(Mathf.Cos(2f * Mathf.PI / 60f * (float)i) * length);
                p1.y = centery + (int)(Mathf.Sin(2f * Mathf.PI / 60f * (float)i) * length);
            }else{
                p1.x = first.x;
                p1.y = first.y;
            }

            if (i > 0){
                // Functie bepaling rand stad/dorp
                dx = p2.x - p1.x;
                dy = p2.y - p1.y;
                max = 0;
                if (abs(dx) < abs(dy)) max = abs(dy);
                else max = abs(dx);
                for (int n = 0; n <= max + 1; n++){
                    cityTerrain[CheckX(p1.x + (int)Mathf.Round(((float)dx / (float)max * (float)n))), CheckY(p1.y + (int)Mathf.Round(((float)dy / (float)max * (float)n)))] = 1;
                }
            }else{
                first.x = p1.x;
                first.y = p1.y;
                firstlength = length;
            }
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
        Rec(centerx, centery); //Recursieve functie voor opvulling stad terrein.
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

    float[,] GenerateHeights(float factor, bool city){
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++){
                heights[x, y] = CalculateHeight(x, y, city, scale) / factor;
            }
        }

        return heights;
    }
    float CalculateHeight(int x, int y, bool city, float scale){
        float xCoord, yCoord;

        if (!city){
            xCoord = (float)x / width * scale + offsetX;
            yCoord = (float)y / height * scale + offsetY;
        }else{
            xCoord = (float)x / width * 2 + offsetX;
            yCoord = (float)y / height * 2 + offsetY;
        }
        
        return Mathf.PerlinNoise(xCoord, yCoord); 
    }

    float[,] GenerateRiver(cord [] city, ref float [,] cityTerrain){

        Debug.Log("You came");
        float[,] output = new float[width, height];
        //sixty boundary points of the city
        // a river will be generated between these two points
        //int riverpoint1number = Random.Range(0,59);
        //int riverpoint2number = (riverpoint1 + Random.Range(20,39))%60; //may not overflow the array of boundary points

        int riverpoint1number = 0;
        int riverpoint2number = 1;

        cord riverpoint1 = city[riverpoint1number];
        cord riverpoint2 = city[riverpoint2number];

        //draw line between city points for referance
        dx = riverpoint2.x - riverpoint1.x;
        dy = riverpoint2.y - riverpoint1.y;

        //rico from the normal, used by riveroffset
        int dx_offset = dy;
        int dy_offset = -dx;
 
        max = 0;
        // calculate the maximum delta
        if (abs(dx) < abs(dy)) max = abs(dy);
        else max = abs(dx);


        for (int n = 0; n <= max; n++){

            int riverpointx = CheckX(riverpoint1.x + (int)Mathf.Round(((float)dx / (float)max * (float)n)));
            int riverpointy = CheckY(riverpoint1.y + (int)Mathf.Round(((float)dy / (float)max * (float)n)));

            float riveroffset = riverOffsetFactor * (CalculateHeight(0, n, false, riverperiod)-0.5f);

            float riverpointOffsetFactor = riveroffset/Mathf.Sqrt(Mathf.Pow((int)dx_offset,2f)+Mathf.Pow((int)dy_offset,2f));

            riverpointx += (int)Mathf.Round((int)dx_offset * (float)riverpointOffsetFactor);
            riverpointy += (int)Mathf.Round((int)dy_offset * (float)riverpointOffsetFactor);

            riverpoints [n] = new cord{
                x=riverpointx,
                y=riverpointy
            };
            Debug.Log("x: " + riverpoints[n].x);
            Debug.Log("y: " + riverpoints[n].y);
            
            

            
            
            



            //cityTerrain[CheckX(riverpoint1.x + (int)Mathf.Round(((float)dx / (float)max * (float)n))), CheckY(riverpoint1.y + (int)Mathf.Round(((float)dy / (float)max * (float)n)))] = 1;
        }

        

        



        return output;
    }

}