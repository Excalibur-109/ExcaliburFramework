using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Excalibur;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental.TerrainAPI;

namespace Excalibur
{
	public class VirtualSlotEditor : Editor
	{
		SerializedProperty m_Select;

        private void OnEnable()
        {
            m_Select = serializedObject.FindProperty("m_Select");
        }

		public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Select);
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
