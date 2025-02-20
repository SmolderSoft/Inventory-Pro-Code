﻿using System.Linq;
using UnityEditor;

namespace Devdog.InventoryPro.Editors
{
    public abstract class InventoryEditorBase : Editor
    {
        protected SerializedProperty script;
        protected CustomOverrideProperty[] overrides;

        public virtual void OnEnable()
        {
            if (target == null)
                return;

            script = serializedObject.FindProperty("m_Script");
        }

        protected CustomOverrideProperty FindOverride(string name)
        {
            return overrides.FirstOrDefault(o => o.serializedName == name);
        }

        public override void OnInspectorGUI()
        {
            OnCustomInspectorGUI();
        }

        protected virtual void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            serializedObject.Update();

//            if(script != null)
//                EditorGUILayout.PropertyField(script);

            serializedObject.ApplyModifiedProperties();
        }

    }
}
