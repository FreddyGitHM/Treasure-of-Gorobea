using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[RequireComponent(typeof(Terrain))]

public class MassPlaceTree : MonoBehaviour
{

    //Terrain data
    private Terrain terrain;
    private TerrainData td;
    private int x = 0;
    private int y = 0;
    private float[,] heightmap;

    // Parameter for decide how many tree will be placed
    public GameObject[] trees;
    public int numberOfTrees;
    public bool keepExistingTree = true;

    // Parameter for keeping track of tree id
    private int lastTreeID = 0;

    // On Draw Gizmos
    bool start;
    List<Vector3> positions;

    public void BeforePlaceTrees()
    {

        //Getting terrain information
        terrain = GetComponent<Terrain>();
        td = terrain.terrainData;
        x = td.heightmapResolution;
        y = td.heightmapResolution;

        //Initialize heighmap
        heightmap = td.GetHeights(0, 0, x, y);

        positions = new List<Vector3>();
        start = true;

        placeTrees();
    }

    private void placeTrees()
    {

        if ((numberOfTrees == 0 && !keepExistingTree) || !keepExistingTree)
        {
            DestroyAllTrees();
            lastTreeID = 0;
        }

#if UNITY_EDITOR

        for (int i = 1; i <= numberOfTrees; i++)
        {

            Selection.activeObject = PrefabUtility.InstantiatePrefab(chooseRandomTree(), transform);

            GameObject tree = Selection.activeGameObject;

            tree.transform.position = getlocation();
            Physics.SyncTransforms();

            tree.GetComponent<TreeID>().treeID = i + lastTreeID;

            if (i == numberOfTrees)
            {
                lastTreeID += i;
            }
        }

        Selection.activeGameObject = transform.gameObject;

#endif
    }

    private GameObject chooseRandomTree()
    {
        return trees[Random.Range(0, trees.Length)];
    }

    private Vector3 getlocation()
    {
        int x = Random.Range(0, this.x);
        int y = Random.Range(0, this.y);

        Vector3 position = new Vector3(x, (heightmap[y, x] * td.size.y), y);

        // Check if location is free
        Collider[] colliders = Physics.OverlapBox(position + Vector3.up * 6, new Vector3(20, 30, 20) * .5f, Quaternion.identity, ~LayerMask.GetMask("Terrain"));

        while (colliders.Length > 0)
        {
            x = Random.Range(0, this.x);
            y = Random.Range(0, this.y);

            position = new Vector3(x, (heightmap[y, x] * td.size.y), y);

            colliders = Physics.OverlapBox(position + Vector3.up * 6, new Vector3(20, 30, 20) * .5f, Quaternion.identity, ~LayerMask.GetMask("Terrain"));
        }

        positions.Add(position);

        return position;
    }

    private void DestroyAllTrees()
    {
        GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");

        foreach (GameObject tree in trees)
        {
            DestroyImmediate(tree);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (start)
        {
            foreach (var position in positions)
            {
                Gizmos.DrawWireCube(position + Vector3.up * 6, new Vector3(20, 20, 20));
            }
        }
    }
}
