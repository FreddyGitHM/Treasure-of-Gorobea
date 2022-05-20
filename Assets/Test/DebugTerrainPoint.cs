using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTerrainPoint : MonoBehaviour
{
    Vector2Int[,] TerrainPoints;

    //Terrain data
    private Terrain terrain;
    private TerrainData td;
    private int x = 0;
    private int y = 0;
    private float[,] heightmap;

    bool start;

    void Start()
    {
        //Getting terrain information
        terrain = GetComponent<Terrain>();
        td = terrain.terrainData;
        x = td.heightmapResolution;
        y = td.heightmapResolution;

        //Initialize heighmap
        heightmap = td.GetHeights(0, 0, x, y);

        TerrainPoints = new Vector2Int[40, y];

        fillTerrainPoints();

        start = true;
    }

    private void fillTerrainPoints()
    {
        for (int i = 0; i < 40; i++)
        {
            for (int j = 0; j < y; j++)
            {
                TerrainPoints[i, j] = new Vector2Int(i, j);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (start)
        {
            for (int i = 0; i < TerrainPoints.GetLength(0); i++)
            {
                for (int j = 0; j < TerrainPoints.GetLength(1); j++)
                {
                    Vector2Int terrainPoint = TerrainPoints[i, j];
                    Vector3 worldPoint = new Vector3(terrainPoint.x, heightmap[terrainPoint.y, terrainPoint.x] * td.size.y, terrainPoint.y);
                    Gizmos.DrawSphere(worldPoint, .1f);
                }
            }
        }
    }
}
