using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;

[RequireComponent(typeof(Terrain))]

public class RandomTreeMapGenerator : MonoBehaviourPunCallbacks
{

    // Instance of this class in order to access at list of spawn position
    public static RandomTreeMapGenerator Instance;

    //Terrain data
    private Terrain terrain;
    private TerrainData td;
    private int x = 0;
    private int y = 0;
    private static float[,] heightmap;

    // Tree with Map
    public GameObject TreeWithMap;
    public static Vector3 TreeWithMapPosition;

    // Radius of the circle where will spawn tree with a map inside it
    public float radius = 180;

    // Boolean value for OnDrawGizmos
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
        terrain = GetComponent<Terrain>();
        td = terrain.terrainData;
        x = td.heightmapResolution;
        y = td.heightmapResolution;

        //Initialize heighmap
        heightmap = td.GetHeights(0, 0, x, y);

        // Initialize start value
        start = true;
    }

    public Vector3 spawnTreeMap()
    {

        // Get Random position inside unit circle
        Vector2 randomPosition = new Vector2(x / 2, y / 2) + Random.insideUnitCircle * radius;

        // Initialize the position of the tree with map
        TreeWithMapPosition = new Vector3((int)randomPosition.x, (heightmap[(int)randomPosition.y, (int)randomPosition.x] * td.size.y) - 3, (int)randomPosition.y);

        Collider[] colliders = Physics.OverlapBox(TreeWithMapPosition, new Vector3(100, 100, 100) * .5f, Quaternion.identity, LayerMask.GetMask("Tree"));

        List<int> trees = new List<int>();

        foreach (Collider collider in colliders)
        {
            // Debug.Log("Destroying tree at position: " + collider.gameObject.transform.position);

            trees.Add(collider.gameObject.GetComponent<TreeID>().getTreeID());

            Destroy(collider.gameObject);
        }

        if (trees.Count > 0)
        {
            object[] data = new object[] { trees.ToArray() };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
            raiseEventOptions.Receivers = ReceiverGroup.Others;
            raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

            SendOptions sendOptions = new SendOptions();
            sendOptions.Reliability = true;

            PhotonNetwork.RaiseEvent(Codes.TREE_DESTROY, data, raiseEventOptions, sendOptions);
        }

        // Instantiate TreeMap at that random position
        // var treeMap = Instantiate(TreeWithMap, TreeWithMapPosition, Quaternion.identity);

        return TreeWithMapPosition;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (start)
        {
            // Draw Sphere representing circle where tree with map can spawn
            Gizmos.DrawWireSphere(new Vector3(x / 2, (heightmap[y / 2, x / 2] * td.size.y) + 50, y / 2), radius);

            // Draw cube representing the area where trees are destroyed
            Gizmos.DrawWireCube(TreeWithMapPosition + Vector3.up * 50, new Vector3(100, 100, 100));

            // Changing Gizmos color for drawing spawn circle
            Gizmos.color = Color.blue;
        }
    }
}
