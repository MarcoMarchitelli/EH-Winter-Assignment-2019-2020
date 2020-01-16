using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobProcAnims : MonoBehaviour {
    [Header("Bones Refs")]
    public Transform[] vertsBones;

    [Header("Params")]
    public float gravityMult = 1;
    [Range(0,360)] public float angleMin;
    [Range(0,360)] public float angleMax;
    
    private void LateUpdate () {
        
    }
}