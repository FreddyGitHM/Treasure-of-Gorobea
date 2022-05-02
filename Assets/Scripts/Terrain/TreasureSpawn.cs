using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Terrain))]

public class TreasureSpawn : MonoBehaviour
{
    // Instance of this class in order to access at list of spawn position
    public static TreasureSpawn Instance;

    //Terrain data
    private Terrain terrain;
    private TerrainData td;
    private int x = 0;
    private int y = 0;
    private float[,] heightmap;

    // OnDrawGizmos
    private bool start = false;
    private Vector2 treasurePosition;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //Getting terrain information
        terrain = GetComponent<Terrain>();
        td = terrain.terrainData;
        x = td.heightmapResolution;
        y = td.heightmapResolution;

        //Initialize heighmap
        heightmap = td.GetHeights(0, 0, x, y);
    }

    private void calculatePosition()
    {
        // Random point inside terrain
        treasurePosition = new Vector2(Random.Range(0, x), Random.Range(0, y));

        // TreeMap position 
        Vector2 treeMapPos = new Vector2(RandomTreeMapGenerator.TreeWithMapPosition.x, RandomTreeMapGenerator.TreeWithMapPosition.z);

        // 
        while (Vector2.Distance(treasurePosition, treeMapPos) < 50)
        {
            treasurePosition = new Vector2(Random.Range(0, x), Random.Range(0, y));
        }

        Debug.Log("Treasure position: " + treasurePosition);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (start)
        {
            Vector3 position = new Vector3(treasurePosition.x, heightmap[(int)treasurePosition.y, (int)treasurePosition.x] * td.size.y, treasurePosition.y);
            Gizmos.DrawWireCube(position, new Vector3(50, 50, 50));
        }
    }
}
