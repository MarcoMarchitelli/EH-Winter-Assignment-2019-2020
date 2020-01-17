using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer( typeof( MinCustomAttribute ) )]
public class MinCustomAttributeDrawer : PropertyDrawer {
    public override void OnGUI ( Rect position, SerializedProperty property, GUIContent label ) {
        MinCustomAttribute mca = attribute as MinCustomAttribute;

        if ( property.propertyType == SerializedPropertyType.Integer ) {
            if ( property.intValue < mca.intValue )
                property.intValue = mca.intValue;
        }
        else if ( property.propertyType == SerializedPropertyType.Float ) {
            if ( property.floatValue < mca.floatValue )
                property.floatValue = mca.floatValue;
        }

        EditorGUI.PropertyField( position, property, label );
    }
}