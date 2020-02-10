using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( BlobGraphics ) )]
public class BlobGraphicsEditor : Editor {
    BlobGraphics bg;

    private void OnEnable () {
        bg = target as BlobGraphics;
    }

    public override void OnInspectorGUI () {
        base.OnInspectorGUI();
        if ( GUILayout.Button( "Generate Mesh" ) ) {
            bg.GenerateMesh();
        }
    }
}