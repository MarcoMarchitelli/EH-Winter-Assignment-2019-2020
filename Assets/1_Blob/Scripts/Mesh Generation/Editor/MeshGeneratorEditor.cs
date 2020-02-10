using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( MeshGenerator ) )]
public class MeshGeneratorEditor : Editor {
    MeshGenerator mg;

    private void OnEnable () {
        mg = target as MeshGenerator;
    }

    public override void OnInspectorGUI () {
        base.OnInspectorGUI();

        if ( mg.autoGen ) {
            mg.GenerateMesh();
        }
        else if ( GUILayout.Button( "Generate" ) ) {
            mg.GenerateMesh();
        }
    }
}