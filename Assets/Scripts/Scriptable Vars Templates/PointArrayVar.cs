using UnityEngine;

[CreateAssetMenu()]
public class PointArrayVar : ScriptableObject {
    public Point[] value;

    public void Set ( Point[] value ) {
        this.value = value;
    }
}