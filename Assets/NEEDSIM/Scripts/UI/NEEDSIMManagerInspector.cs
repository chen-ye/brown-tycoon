/**
 * This is part of the NEEDSIM Life simulation plug in for Unity, version 1.0.1
 * Copyright 2014 - 2015 Fantasieleben UG (haftungsbeschraenkt)
 *
 * http;//www.fantasieleben.com for further details.
 *
 * For questions please get in touch with Tilman Geishauser: tilman@fantasieleben.com
 */

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using NEEDSIMEditor;
using Simulation;
using System.Collections;

namespace NEEDSIM
{
    /// <summary>
    /// The inspector exposes the fields of the NEEDSIM Manager in a convenient way to the unity editor.
    /// </summary>
    [CustomEditor(typeof(NEEDSIMManager))]
    [System.Serializable]
    public class NEEDSIMManagerInspector : Editor
    {
        #region Serialized Properties

        private SerializedProperty BuildAffordanceTreeFromScene;
        private SerializedProperty LogSimulation;
        private SerializedProperty DatabaseName;

        public bool generalSettings = true;
        public bool advancedSettings = true;

        public Simulation.DatabaseAsset myDatabase;

        #endregion

        #region Unity Runtime

        void OnEnable()
        {
            BuildAffordanceTreeFromScene = serializedObject.FindProperty("buildAffordanceTreeFromScene");
            LogSimulation = serializedObject.FindProperty("LogSimulation");
            DatabaseName = serializedObject.FindProperty("databaseName");

            serializedObject.Update();
            if (DatabaseName.stringValue == "")
            {
                DatabaseName.stringValue = Strings.DefaultDatabaseName;
            }

            serializedObject.ApplyModifiedProperties();

            myDatabase = Resources.Load(DatabaseName.stringValue) as Simulation.DatabaseAsset;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (generalSettings = EditorGUILayout.Foldout(generalSettings, Simulation.Strings.GeneralSettings))
            {
                EditorExtensionSettings.StandardMargin();
                GeneralSettingsEditing();
            }

            EditorExtensionSettings.LargeMargin();
            if (advancedSettings = EditorGUILayout.Foldout(advancedSettings, Simulation.Strings.AdvancedSettings))
            {
                EditorExtensionSettings.StandardMargin();
                AdvancedSettingsEditing();
            }
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        private void GeneralSettingsEditing()
        {
            EditorGUILayout.BeginVertical();
            {
                LogSimulation.boolValue = EditorGUILayout.ToggleLeft(Simulation.Strings.LogSimulationLabel, LogSimulation.boolValue, EditorExtensionSettings.LongEditorFieldWidth);
                BuildAffordanceTreeFromScene.boolValue = EditorGUILayout.ToggleLeft(Simulation.Strings.BuildAffordanceTreeFromSceneLabel, BuildAffordanceTreeFromScene.boolValue, EditorExtensionSettings.LongEditorFieldWidth);
                EditorExtensionSettings.StandardMargin();
                EditorExtensionSettings.StandardMargin();
                if (GUILayout.Button(Simulation.Strings.PrintSimDebugLogLabel, EditorExtensionSettings.MediumEditorFieldWidth))
                {
                    NEEDSIMManager.PrintSimulationDebugLogToConsole();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void AdvancedSettingsEditing()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(Simulation.Strings.AttachSpecificDBLabel, EditorExtensionSettings.StandardEditorFieldWidth);
                    EditorExtensionSettings.StandardMargin();
                    EditorGUI.BeginChangeCheck();
                    myDatabase = EditorGUILayout.ObjectField(myDatabase, typeof(Simulation.DatabaseAsset), false, EditorExtensionSettings.MediumEditorFieldWidth) as Simulation.DatabaseAsset;
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (myDatabase != null)
                        {
                            DatabaseName.stringValue = myDatabase.DatabaseName;
                        }
                        else
                        {
                            DatabaseName.stringValue = Strings.DefaultDatabaseName;
                        }

                        myDatabase = Resources.Load(DatabaseName.stringValue) as Simulation.DatabaseAsset;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorExtensionSettings.StandardMargin();
                EditorGUILayout.LabelField(DatabaseName.stringValue, EditorExtensionSettings.SmallText);
            }
            EditorGUILayout.EndVertical();
        }
    }
}
#endif