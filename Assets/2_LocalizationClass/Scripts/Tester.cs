using UnityEngine;

public class Tester : MonoBehaviour {
    public string key;

    void Start () {
        Debug.Log( key.Localize( Random.Range( 0, 100000 ) ) );
    }
}