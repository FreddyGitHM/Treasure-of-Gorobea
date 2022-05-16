using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomRockSpawn))]
public class ButtonPlaceRocks : Editor
{

    public override void OnInspectorGUI(){

        DrawDefaultInspector();

        RandomRockSpawn myScript = (RandomRockSpawn)target;

        if(GUILayout.Button("Generate Rocks")){
            myScript.BeforePlaceRocks();
        }
    }

}
