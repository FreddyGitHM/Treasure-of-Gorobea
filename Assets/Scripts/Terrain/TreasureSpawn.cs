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

    // Spawn distance from tree map
    public int distance;

    // Chest prefab
    public GameObject treasure;

    // Rotation in order to rotate the chest according with point normal
    private Quaternion fitTerrain;

    // OnDrawGizmos
    private bool start = false;
    private Vector2 treasurePosition;
    private Vector3 TreasurePosition;

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

        start = true;
    }

    public Vector3 getTreasurePosition()
    {
        // TreeMap position 
        Vector2 treeMapPos = new Vector2(RandomTreeMapGenerator.TreeWithMapPosition.x, RandomTreeMapGenerator.TreeWithMapPosition.z);

        // Random point on terrain (Tested on large map)
        treasurePosition = new Vector2(Random.Range(60, x - 60), Random.Range(60, y - 60));

        // Treasure position in world coordinates
        TreasurePosition = new Vector3(treasurePosition.x, heightmap[(int)treasurePosition.y, (int)treasurePosition.x] * td.size.y, treasurePosition.y) + Vector3.up * .65f;

        // Check for the colliders except for terrain
        Collider[] colliders = Physics.OverlapBox(TreasurePosition, new Vector3(1, 1, 1), Quaternion.identity, ~LayerMask.GetMask("Terrain"));

        // 
        while (Vector2.Distance(treasurePosition, treeMapPos) < distance || colliders.Length > 0)
        {
            treasurePosition = new Vector2(Random.Range(10, x - 10), Random.Range(10, y - 10));
            TreasurePosition = new Vector3(treasurePosition.x, heightmap[(int)treasurePosition.y, (int)treasurePosition.x] * td.size.y, treasurePosition.y) + Vector3.up * .65f;
            colliders = Physics.OverlapBox(TreasurePosition, new Vector3(1, 1, 1), Quaternion.identity, ~LayerMask.GetMask("Terrain"));
        }

        // Finding best rotation to fit the treasure chest to the terrain steepness according with the normal of the point where the chest will be spawned
        RaycastHit hit;

        Physics.Raycast(new Vector3(treasurePosition.x, 300, treasurePosition.y), Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain"));

        fitTerrain = Quaternion.FromToRotation(Vector3.up, hit.normal);

        return hit.point + Vector3.up * .65f;

    }

    public Quaternion getTreasureRotation()
    {

        Vector3 treasurePos = new Vector3(treasurePosition.x, heightmap[(int)treasurePosition.y, (int)treasurePosition.x] * td.size.y, treasurePosition.y);

        Quaternion treasureRot = Quaternion.LookRotation((RandomTreeMapGenerator.TreeWithMapPosition - treasurePos).normalized, Vector3.up);

        // This combine two rotation, the one needed for fitting chest to terrain steepness and the other one in order to make chest look at treasure map
        // fitTerrain rotation is applied first than treasureRot
        return fitTerrain * treasureRot;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (start)
        {
            Vector3 position = new Vector3(treasurePosition.x, heightmap[(int)treasurePosition.y, (int)treasurePosition.x] * td.size.y, treasurePosition.y);
            Gizmos.DrawWireCube(position, new Vector3(50, 50, 50));

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(TreasurePosition, new Vector3(2, 2, 2));
        }
    }
}
