#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Terrain))]

public class RandomRockSpawn : MonoBehaviour
{
    //Terrain data
    private Terrain terrain;
    private TerrainData td;
    private int x = 0;
    private int y = 0;
    private float[,] heightmap;

    // Rocks
    public GameObject[] Rocks;
    public int numberOfRocks;
    public bool keepExistingTree = true;

    // OnDrawGizmos
    bool start;
    Vector3 position;
    GameObject rock;

    public void BeforePlaceRocks()
    {
        //Getting terrain information
        terrain = GetComponent<Terrain>();
        td = terrain.terrainData;
        x = td.heightmapResolution;
        y = td.heightmapResolution;

        //Initialize heighmap
        heightmap = td.GetHeights(0, 0, x, y);

        start = true;

        placeRocks();
    }

    private void placeRocks()
    {
        if ((numberOfRocks == 0 && !keepExistingTree) || !keepExistingTree)
        {
            DestroyAllRocks();
        }

        for (int i = 1; i <= numberOfRocks; i++)
        {

            Selection.activeObject = PrefabUtility.InstantiatePrefab(RandomRock(), transform);

            rock = Selection.activeGameObject;

            rock.transform.position = getlocation();
            rock.transform.rotation = getRotation();
            Physics.SyncTransforms();
        }

        Selection.activeGameObject = transform.gameObject;
    }

    private GameObject RandomRock()
    {
        return Rocks[Random.Range(0, Rocks.Length)];
    }

    private void DestroyAllRocks()
    {
        GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");

        foreach (GameObject rock in rocks)
        {
            DestroyImmediate(rock);
        }
    }

    private Vector3 getlocation()
    {
        int x = Random.Range(0, this.x);
        int y = Random.Range(0, this.y);

        position = new Vector3(x, (heightmap[y, x] * td.size.y), y);

        // Check if location is free
        Collider[] colliders = Physics.OverlapBox(position + rock.GetComponent<BoxCollider>().center, (rock.GetComponent<BoxCollider>().size + Vector3.one * .3f) * .5f, getRotation(), ~LayerMask.GetMask("Terrain"));

        while (colliders.Length > 0)
        {
            x = Random.Range(0, this.x);
            y = Random.Range(0, this.y);

            position = new Vector3(x, (heightmap[y, x] * td.size.y), y);

            colliders = Physics.OverlapBox(position + rock.GetComponent<BoxCollider>().center, (rock.GetComponent<BoxCollider>().size + Vector3.one * .3f) * .5f, getRotation(), ~LayerMask.GetMask("Terrain"));
        }

        return position;
    }

    private Quaternion getRotation()
    {
        RaycastHit hit;

        Physics.Raycast(new Vector3(position.x, 200, position.z), Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain"));
        Debug.DrawRay(position, hit.normal * 1000, Color.yellow, 10);
        Debug.DrawRay(position, Vector3.up * 1000, Color.blue, 10);

        return Quaternion.FromToRotation(Vector3.up, hit.normal);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (start)
        {
            if (rock != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(position + rock.GetComponent<BoxCollider>().center, rock.GetComponent<BoxCollider>().size + Vector3.one * .3f);
            }
        }
    }
}

#endif
