using System;
using EVRTH.Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace EVRTH.Editor
{
    [CustomPropertyDrawer(typeof(Date))]
    public class DateTimePropertyDrawer : PropertyDrawer
    {
        private float indent = 10;
        private const int width = 45;
        private const int height = 15;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty dayProp = property.FindPropertyRelative("day");
            SerializedProperty monthProp = property.FindPropertyRelative("month");
            SerializedProperty yearProp = property.FindPropertyRelative("year");
            //float labelHeight = GetPropertyHeight(property, label);
            Rect region = new Rect(position.x + indent, position.y /*+ labelHeight*/, width, height);
            EditorGUI.BeginChangeCheck();
            EditorGUI.LabelField(region,"Month");
            region = new Rect(position.x + indent, position.y + height, width, height);
            int month = EditorGUI.IntField(region, monthProp.intValue);
            region = new Rect(position.x + indent + 40, position.y , width, height);
            EditorGUI.LabelField(region, "Day");
            region = new Rect(position.x + indent + 40, position.y + height, width, height);
            int day = EditorGUI.IntField(region, dayProp.intValue);
            region = new Rect(position.x + indent + 80, position.y , width, height);
            EditorGUI.LabelField(region, "Year");
            region = new Rect(position.x + indent + 80, position.y + height, width, height);
            int year = EditorGUI.IntField(region, yearProp.intValue);
            if (EditorGUI.EndChangeCheck())
            {
                dayProp.intValue = day;
                monthProp.intValue = month;
                yearProp.intValue = year;
            }
        }
    }
}
