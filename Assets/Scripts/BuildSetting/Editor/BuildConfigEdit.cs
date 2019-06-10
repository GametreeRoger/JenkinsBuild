using UnityEngine;
using UnityEditor;
using System;

namespace SG.Build {
    [CustomEditor(typeof(BuildConfig))]
    public class BuildConfigEdit : Editor {
        BuildConfig mData;

        public void OnEnable() {
            mData = (BuildConfig)target;
        }

        public override void OnInspectorGUI() {
            mData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("BuildTarget", mData.BuildTarget);
            mData.BuildTargetGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup("BuildTargetGroup", mData.BuildTargetGroup);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("BundleId"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Version"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BuildNumber"));
            SerializedProperty definesProperty = serializedObject.FindProperty("Defines");
            if(EditorGUILayout.PropertyField(definesProperty)) {
                EditorGUI.indentLevel++;
                definesProperty.arraySize = EditorGUILayout.DelayedIntField("Size", definesProperty.arraySize);
                for(int i = 0, size = definesProperty.arraySize; i < size; i++) {
                    SerializedProperty element = definesProperty.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(element);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ICON"));
            SerializedProperty scenesProperty = serializedObject.FindProperty("Scenes");
            if(EditorGUILayout.PropertyField(scenesProperty)) {
                EditorGUI.indentLevel++;
                scenesProperty.arraySize = EditorGUILayout.DelayedIntField("Size", scenesProperty.arraySize);
                for(int i = 0, size = scenesProperty.arraySize; i < size; i++) {
                    SerializedProperty element = scenesProperty.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(element);
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            if(mData.BuildTarget == BuildTarget.iOS) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AppleDeveloperTeamID"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TargetOSVersionString"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraUsageDescription"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("LocationUsageDescription"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("MicrophoneUsageDescription"));
            } else if(mData.BuildTarget == BuildTarget.Android) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("WritePermissionInternal"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("KeyStorePass"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("KeyAliasPass"));
            }

            serializedObject.ApplyModifiedProperties();
            if(GUILayout.Button("Setup")) {
                string filename = serializedObject.FindProperty("m_Name").stringValue;
                BuildConfig cfg = BuildDriver.GetConfigFile(filename);
                if(cfg != null) {
                    if(BuildDriver.Configure(cfg)) {
                        Debug.Log("Build Configure Complete!");
                    }
                }
            }
        }
    }
}