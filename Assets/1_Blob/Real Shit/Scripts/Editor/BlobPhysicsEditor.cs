using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( BlobPhysics ) )]
public class BlobPhysicsEditor : Editor {
    BlobPhysics bp;

    private void OnEnable () {
        bp = target as BlobPhysics;
    }

    public override void OnInspectorGUI () {
        base.OnInspectorGUI();
        if ( GUILayout.Button( "Generate" ) ) {
            bp.Generate();
        }
    }
}