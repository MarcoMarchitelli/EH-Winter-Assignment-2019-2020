using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof( Text ) )]
public class Localizer : MonoBehaviour {
    public string key;

    private void OnEnable () {
        Text text = GetComponent<Text>();
        if ( text != null ) {
            text.text = key.Localize();
        }
    }
}