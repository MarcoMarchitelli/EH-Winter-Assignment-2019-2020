using UnityEngine;

[CreateAssetMenu()]
public class MeshVar : ScriptableObject {
    public Mesh value;

    public void Set ( Mesh value ) {
        this.value = value;
    }
}