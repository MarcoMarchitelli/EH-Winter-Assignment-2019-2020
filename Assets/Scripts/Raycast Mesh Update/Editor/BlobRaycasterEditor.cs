using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( BlobRaycaster ) )]
public class BlobRaycasterEditor : Editor {
    private BlobRaycaster br;

    private void OnEnable () {
        br = target as BlobRaycaster;
    }

    public override void OnInspectorGUI () {
        base.OnInspectorGUI();

        if ( GUILayout.Button( "Create Points" ) )
            br.CreatePoints();
    }
}