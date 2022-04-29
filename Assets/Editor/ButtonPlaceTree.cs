using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MassPlaceTree))]
public class ButtonPlaceTree : Editor
{

    public override void OnInspectorGUI(){

        DrawDefaultInspector();

        MassPlaceTree myScript = (MassPlaceTree)target;

        if(GUILayout.Button("Generate Trees")){
            myScript.BeforePlaceTrees();
        }
    }

}
