using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Terrain))]

public class MassPlaceTree : MonoBehaviour
{

    //Terrain data
    private Terrain terrain;
    private TerrainData td;
    private int x = 0;
    private int y = 0;
    private float [,] heightmap;

    // Parameter for decide how many tree will be placed
    public GameObject[] trees;
    public int numberOfTrees;
    public bool keepExistingTree = true;

    // Parameter for keeping track of tree id
    private int lastTreeID = 0;

    public void BeforePlaceTrees(){

        //Getting terrain information
        terrain = GetComponent<Terrain> ();
        td = terrain.terrainData;
        x = td.heightmapResolution;
        y = td.heightmapResolution;

        //Initialize heighmap
        heightmap = td.GetHeights(0, 0, x, y);

        placeTrees();
    }

    private void placeTrees(){

        if(numberOfTrees == 0 && !keepExistingTree){
            DestroyAllTrees();
            lastTreeID = 0;
        }

        #if UNITY_EDITOR

        for (int i = 1; i <= numberOfTrees; i++){

            Selection.activeObject = PrefabUtility.InstantiatePrefab(chooseRandomTree(), transform);

            GameObject tree = Selection.activeGameObject;

            tree.transform.position = getlocation();

            tree.GetComponent<TreeID>().treeID = i + lastTreeID;

            if(i == numberOfTrees){
                lastTreeID += i;
            }
        }

        Selection.activeGameObject = transform.gameObject;

        #endif

        // Make sure to sync with physics engine in order to try to spawn Tree with map
        // Physics.SyncTransforms();
    }

    private GameObject chooseRandomTree(){ 

        return trees[Random.Range(0, trees.Length)];
    }

    private Vector3 getlocation(){
        int x = Random.Range(0, this.x);
        int y = Random.Range(0, this.y);

        Vector3 position = new Vector3(x, (heightmap[y, x] * td.size.y), y);

        // Check if location is free
        Collider[] colliders = Physics.OverlapBox(position + Vector3.up * 6, new Vector3(10, 30, 10) * .5f, Quaternion.identity, LayerMask.GetMask("Tree"));

        while(colliders.Length > 0){
            x = Random.Range(0, this.x);
            y = Random.Range(0, this.y);

            position = new Vector3(x, (heightmap[y, x] * td.size.y), y);

            colliders = Physics.OverlapBox(position + Vector3.up * 6, new Vector3(10, 30, 10) * .5f, Quaternion.identity, LayerMask.GetMask("Tree"));
        }

        return position;
    }

    private void DestroyAllTrees(){
        GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");

        foreach (GameObject tree in trees){
            DestroyImmediate(tree);
        }
    }
}
