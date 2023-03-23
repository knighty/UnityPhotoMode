using UnityEditor;
using UnityEngine;

namespace PhotoMode
{
	[CustomPropertyDrawer(typeof(PhotoModeSetting<>))]
	public class PhotoModeSettingEditor : PropertyDrawer
	{
		// Draw the property inside the given rect
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.FindPropertyRelative("currentValue") == null)
				return;

			//PhotoModeSetting setting = property.serializedObject.targetObject as PhotoModeSetting;

			EditorGUI.BeginProperty(position, label, property);

			//position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			var amountRect = new Rect(position.x, position.y, position.width, position.height);
			Rect toggleRect = amountRect;
			toggleRect.width = 20;
			Rect valueRect = amountRect;
			valueRect.x += toggleRect.width;
			valueRect.width -= toggleRect.width;

			//EditorGUIUtility.labelWidth = 60;
			property.FindPropertyRelative("overriding").boolValue = EditorGUI.Toggle(toggleRect, property.FindPropertyRelative("overriding").boolValue);
			//EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("currentValue"), label);
			/*if (EditorGUI.EndChangeCheck())
			{
				property.serializedObject.ApplyModifiedProperties();
				setting.Update();
			}*/

			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}
}