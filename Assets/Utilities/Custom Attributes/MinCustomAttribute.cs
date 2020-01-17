using System;
using UnityEngine;

[AttributeUsage( AttributeTargets.Field )]
public class MinCustomAttribute : PropertyAttribute {
    public int intValue;
    public float floatValue;

    public MinCustomAttribute ( int value ) {
        intValue = value;
    }

    public MinCustomAttribute ( float value ) {
        floatValue = value;
    }
}