using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Terrain))]

public class SpawnPosition : MonoBehaviour
{
    // Instance of this class in order to access at list of spawn position
    public static SpawnPosition Instance;

    //Terrain data
    private Terrain terrain;
    private TerrainData td;
    private int x = 0;
    private int y = 0;
    private float[,] heightmap;

    // Distance of the spawn positions from the tree with map, radius of the circle
    public int spawnDistance = 350;

    // Stack for putting inside spawn positions
    Stack positions;

    // OnDrawGizmos
    bool start;

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
        terrain = Terrain.activeTerrain;
        td = terrain.terrainData;
        x = td.heightmapResolution;
        y = td.heightmapResolution;

        //Initialize heighmap
        heightmap = td.GetHeights(0, 0, x, y);

        // Initialize stack
        positions = new Stack();

        // Activate OnDrawGizmos
        start = true;
    }

    public void calculateSpawnPositions(int numberOfPlayers)
    {

        // Distance angle between the players
        // angolo del cerchio (angolo giro = 360 gradi)/ numero di giocatori
        float distanceAngle = Mathf.PI * 2 / numberOfPlayers;

        Vector2 center = new Vector2(RandomTreeMapGenerator.TreeWithMapPosition.x, RandomTreeMapGenerator.TreeWithMapPosition.z);

        for (int i = 0; i < numberOfPlayers; i++)
        {
            Vector2 result = center + new Vector2(Mathf.Cos(distanceAngle * i) * spawnDistance, Mathf.Sin(distanceAngle * i) * spawnDistance);
            positions.Push(result);
        }
    }

    public Vector3 getSpawnPosition()
    {
        Vector2 result = (Vector2)positions.Pop();

        return new Vector3(result.x, heightmap[(int)result.y, (int)result.x] * td.size.y, result.y);
    }

    public Quaternion getLookDirection(Vector3 position)
    {
        // Direction from spawn position to tree map
        Vector3 centerDir = (RandomTreeMapGenerator.TreeWithMapPosition - position).normalized;
        // Debug.DrawRay(position, centerDir*500, Color.green, 10);

        // Direction rotated 45 to the left of centerDir
        Vector3 leftDir = Quaternion.Euler(0, -45, 0) * centerDir;
        // Debug.DrawRay(position, leftDir*500, Color.yellow, 10);

        // Direction rotated 45 to the right of centerDir
        Vector3 rightDir = Quaternion.Euler(0, 45, 0) * centerDir;
        // Debug.DrawRay(position, rightDir*500, Color.yellow, 10);

        // Random direction between left and right one
        Vector3 randomDir = Vector3.Lerp(leftDir, rightDir, Random.Range(0f, 1f));

        return Quaternion.LookRotation(randomDir, Vector3.up);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (start)
        {
            // Draw Sphere representing circle on which player can spawn
            Gizmos.DrawWireSphere(RandomTreeMapGenerator.TreeWithMapPosition, spawnDistance);

            foreach (Vector2 position in positions)
            {
                // Spawn position
                Vector3 spawnPos = new Vector3(position.x, heightmap[(int)position.y, (int)position.x] * td.size.y, position.y);

                // Draw Cube representing spawn position
                Gizmos.DrawWireCube(spawnPos, new Vector3(50, 50, 50));
            }
        }
    }
}
