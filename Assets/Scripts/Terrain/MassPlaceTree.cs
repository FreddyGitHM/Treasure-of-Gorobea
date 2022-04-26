using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // OnDrawGizmos
    bool start;
    Vector3 position;

    void Start(){
        //Getting terrain information
        terrain = GetComponent<Terrain> ();
        td = terrain.terrainData;
        x = td.heightmapResolution;
        y = td.heightmapResolution;

        //Initialize heighmap
        heightmap = td.GetHeights(0, 0, x, y);

        // Draw Gizmos
        start = false;

        placeTrees();
    }

    private void placeTrees(){

        for (int i = 0; i < numberOfTrees; i++){
            var trees = Instantiate(chooseRandomTree(), getlocation(), Quaternion.identity);
        }

        // Make sure to sync with physics engine in order to try to spawn Tree with map
        // Physics.SyncTransforms();
    }

    private GameObject chooseRandomTree(){ 

        return trees[Random.Range(0, trees.Length)];
    }

    private Vector3 getlocation(){
        int x = Random.Range(0, this.x);
        int y = Random.Range(0, this.y);

        position = new Vector3(x, (heightmap[y, x] * td.size.y), y);

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

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        if(start){
            Gizmos.DrawWireCube(position + Vector3.up * 6, new Vector3(10, 30, 10));
        }
    }
}
