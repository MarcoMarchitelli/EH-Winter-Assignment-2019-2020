using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( MiniCirclesGenerator ) )]
public class MiniCirclesGeneratorEditor : Editor {
    MiniCirclesGenerator mcg;

    private void OnEnable () {
        mcg = target as MiniCirclesGenerator;
    }

    public override void OnInspectorGUI () {
        base.OnInspectorGUI();

        if ( mcg.autoGen ) {
            mcg.Generate();
        }
        else if ( GUILayout.Button( "Generate" ) ) {
            mcg.Generate();
        }
    }
}