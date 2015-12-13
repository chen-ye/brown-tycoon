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
using Simulation;
using System.Collections;
using System.Collections.Generic;
using NEEDSIMEditor;

namespace NEEDSIM
{
    /// <summary>
    /// The inspector exposes the fields of the NEEDSIM Node in a convenient way to the unity editor.
    /// </summary>
    [System.Serializable]
    [CustomEditor(typeof(NEEDSIMNode))]
    public class NEEDSIMNodeInspector : Editor
    {
        #region Fields

        #region SmartObject

        private NEEDSIMNode myTarget;
        private Transform transform;

        private SerializedProperty IsAgent;
        private string InteractionDataLength = "InteractionData.Array.size";
        private string InteractionDataAccess = "InteractionData.Array.data[{0}]";
        private string[] interactionOptions;
        private string[] speciesOptions;
        private string[] needs;
        private Species species;
        private bool territorySettings = true;

        #endregion

        #region Territory

        private SerializedProperty ModifyLookAt;

        private SerializedProperty ShowDebug;

        private string arraySizeWorldSlotPositions = "slotPositionsWorldSpace.Array.size";
        private string accessWorldSlotPositions = "slotPositionsWorldSpace.Array.data[{0}]";

        private string arraySizeLocalSlotPositions = "slotPositionsLocalSpace.Array.size";
        private string accessLocalSlotPositions = "slotPositionsLocalSpace.Array.data[{0}]";

        private string arraySizeAuctionable = "auctionableBool.Array.size";
        private string accessAuctionable = "auctionableBool.Array.data[{0}]";

        private string arraySizeLocalLookAtTarget = "slotLocalLookAtTarget.Array.size";
        private string accessLocalLookAtTarget = "slotLocalLookAtTarget.Array.data[{0}]";

        private Vector3 lastTransformPosition;
        private Vector3 lastEulerAngles;
        private Vector3 lastScale;

        #endregion

        #region Agent

        private SerializedProperty SpeciesName;
        private SerializedProperty ShowDebugInGame;
        //private SerializedProperty ShowDebugInSpector;
        private int speciesSelector;
        private SerializedProperty selectedPlan;

        private string StartingSatisfactionLevels_FloatValues_arraySize
            = "StartingSatisfactionLevels_FloatValues.Array.size";
        private string StartingSatisfactionLevels_FloatValues_access
         = "StartingSatisfactionLevels_FloatValues.Array.data[{0}]";
        private string StartingSatisfactionLevels_NeedKeys_arraySize
            = "StartingSatisfactionLevels_NeedKeys.Array.size";
        private string StartingSatisfactionLevels_NeedKeys_access
            = "StartingSatisfactionLevels_NeedKeys.Array.data[{0}]";

        private SerializedProperty StartingSatisfactionLevels_FloatValues;
        private SerializedProperty StartingSatisfactionLevels_NeedKeys;
        private SerializedProperty SpecificSatisfactionLevelsAtStart;

        #endregion

        #endregion

        #region UNITY RUNTIME
        void OnEnable()
        {
            myTarget = this.target as NEEDSIMNode;
            transform = myTarget.transform;
            IsAgent = serializedObject.FindProperty("isAgent");
            ShowDebug = serializedObject.FindProperty("drawGizmosInGame");
            ModifyLookAt = serializedObject.FindProperty("ModifyLookAt");
            SpeciesName = serializedObject.FindProperty("speciesName");
            ShowDebugInGame = serializedObject.FindProperty("showDebugInGame");
            //ShowDebugInSpector = serializedObject.FindProperty("showDebugInInspector");
            selectedPlan = serializedObject.FindProperty("selectedPlan");
            SpecificSatisfactionLevelsAtStart = serializedObject.FindProperty("SpecificSatisfactionLevelsAtStart");

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();

            CreateOptionsBasedOnDatabase();

            lastTransformPosition = transform.position;
            lastEulerAngles = transform.rotation.eulerAngles;
            lastScale = transform.lossyScale;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SmartObjectGUI();

            serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI()
        {
            serializedObject.Update();
            UpdateSlotPostionsToTransformMovement();
            DisplayHandles();
            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region Data Handling

        private void SetSimulationData(NEEDSIMManager config)
        {
            if (GameDataManager.Data == null)
            {
                if (config != null)
                {
                    GameDataManager.InitSimData(config.databaseName, false);
                }
                else
                {
                    GameDataManager.InitSimData(NEEDSIMEditor.DataManager.DefaultDatabase.DatabaseName, true);
                }
            }
        }

        private void CreateOptionsBasedOnDatabase()
        {
            DatabaseAsset data;
            NEEDSIMManager config = (NEEDSIMManager)FindObjectOfType<NEEDSIMManager>();

            SetSimulationData(config);

            if (config == null)
            {
                data = DataManager.DefaultDatabase;
                Debug.LogWarning("No NEEDSIM configuration in scene. Default database will be used, unless you add a NEEDSIM Manager prefab");
            }
            else
            {
                data = Resources.Load(config.databaseName) as DatabaseAsset;
            }

            interactionOptions = MakeInteractionOptions(data);
            speciesOptions = MakeSpeciesOptions(data);
            needs = MakeNeedsArray(data);
            UpdateNeeds();

            foreach (Species species in data.Species)
            {
                if (species.speciesName == SpeciesName.stringValue)
                {
                    this.species = species;
                }
            }
        }

        private string[] MakeNeedsArray(DatabaseAsset data)
        {
            if (data != null)
            {
                return data.GetNeedNames();
            }
            else return null;
        }

        private string[] MakeInteractionOptions(DatabaseAsset data)
        {
            int i = 1;
            int count;
            string firstInArray;

            if (data != null)
            {
                count = data.Interactions.Count;
                firstInArray = Strings.None;
            }
            else
            {
                count = 0;
                firstInArray = "Something went wrong loading a database.";
                Debug.LogWarning("No default database!");
            }

            string[] temp = new string[count + i];
            temp[0] = firstInArray;

            if (data != null)
            {
                foreach (InteractionData interaction in data.Interactions)
                {
                    temp[i] = interaction.interactionName;
                    i++;
                }
            }

            return temp;
        }

        private string[] MakeSpeciesOptions(DatabaseAsset data)
        {
            int i = 2;
            int count;
            string firstInArray;

            if (data != null)
            {
                count = data.Species.Count;
                firstInArray = Strings.AssignSpeciesOptionToAgentLabel;
            }
            else
            {
                count = 0;
                firstInArray = "Something went wrong loading a database.";
                Debug.LogWarning("No default database present!");
            }

            string[] temp = new string[count + i];
            temp[0] = firstInArray;
            temp[1] = Simulation.Strings.None;

            if (data != null)
            {
                foreach (Species species in data.Species)
                {
                    temp[i] = species.speciesName;
                    i++;
                }
            }
            return temp;
        }

        private Vector3 SpawningPosition(int i)
        {
            if (myTarget != null)
            {
                float x = (float)i * 0.125f;
                float y = 0.0f;
                float z = -1.0f;

                return new Vector3(x, y, z);
            }
            else
            {
                return Vector3.zero;
            }
        }

        private int SelectionPosition(string[] array, string compare)
        {
            int position = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == compare)
                {
                    position = i;
                    break;
                }
            }

            return position;
        }

        #endregion

        #region GUI Drawing

        private void SmartObjectGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorExtensionSettings.StandardMargin();
                //This can be used if you want to have more than one interaction on a NEEDSIM Node, but we do not yet officially
                //support this.
                /*EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(Simulation.Strings.InspectorNumberOfInteractionsLabel, EditorSettings.StandardEditorFieldWidth);
                    CustomGUI.LargeMargin();
                    EditorGUI.BeginChangeCheck();
                    serializedObject.FindProperty(InteractionDataLength).intValue
                        = EditorGUILayout.IntSlider(serializedObject.FindProperty(InteractionDataLength).intValue,
                                                    1,
                                                    EditorSettings.MaxNumberOfInteractionPerSmartObject,
                                                    EditorSettings.StandardEditorFieldWidth);
                } EditorGUILayout.EndHorizontal();
                CustomGUI.LargeMargin();*/

                if (serializedObject.FindProperty(InteractionDataLength).intValue < 1)
                {
                    serializedObject.FindProperty(InteractionDataLength).intValue = 1;
                }

                for (int i = 0; i < serializedObject.FindProperty(InteractionDataLength).intValue; i++)
                {
                    InteractionEditing(i);
                }

                EditorExtensionSettings.StandardMargin();
                IsAgent.boolValue = EditorGUILayout.ToggleLeft(Simulation.Strings.IsAgentLabel, IsAgent.boolValue, EditorExtensionSettings.StandardEditorFieldWidth);
                if (IsAgent.boolValue)
                {
                    AgentEditing();
                }

                EditorExtensionSettings.MediumMargin();
                if (territorySettings = EditorGUILayout.Foldout(territorySettings, Simulation.Strings.TerritoryControlHeadline))
                {
                    SlotEditing();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void InteractionEditing(int loop)
        {
            int indexer = 0;

            for (int i = 1; i < interactionOptions.Length; i++)
            {
                if (serializedObject.FindProperty(string.Format(InteractionDataAccess, loop)).stringValue == interactionOptions[i])
                {
                    indexer = i;
                    break;
                }
            }

            if (CustomGUI.ChangeCheckingPopup(ref indexer, interactionOptions, Simulation.Strings.InteractionLabelToExtend(loop + 1)))
            {
                serializedObject.FindProperty(string.Format(InteractionDataAccess, loop)).stringValue = interactionOptions[indexer];
            }
        }

        private void AgentEditing()
        {
            EditorExtensionSettings.StandardMargin();
            speciesSelector = SelectionPosition(speciesOptions, SpeciesName.stringValue);
            if (CustomGUI.ChangeCheckingPopup(ref speciesSelector, speciesOptions, ""))
            {
                if (speciesSelector > 0)
                {
                    SpeciesName.stringValue = speciesOptions[speciesSelector];
                }
                else
                {
                    SpeciesName.stringValue = "";
                }
            }

            selectedPlan.enumValueIndex = (int)(NEEDSIMNode.ExamplePlans)EditorGUILayout.EnumPopup((NEEDSIMNode.ExamplePlans)selectedPlan.enumValueIndex, EditorExtensionSettings.MediumEditorFieldWidth);

            EditorExtensionSettings.StandardMargin();
            ShowDebugInGame.boolValue = EditorGUILayout.ToggleLeft(Simulation.Strings.DebugIngameLabel, ShowDebugInGame.boolValue, EditorExtensionSettings.MediumEditorFieldWidth);
            EditorExtensionSettings.StandardMargin();
            //ShowDebugInSpector.boolValue = EditorGUILayout.ToggleLeft(Simulation.Strings.DebugInSpectorLabel, ShowDebugInSpector.boolValue, EditorExtensionSettings.MediumEditorFieldWidth);
            //CustomGUI.StandardMargin();
            SpecificSatisfactionLevelsAtStart.boolValue = EditorGUILayout.ToggleLeft(Simulation.Strings.RandomStartLevelsLabel, SpecificSatisfactionLevelsAtStart.boolValue, EditorExtensionSettings.MediumEditorFieldWidth);

            if (SpecificSatisfactionLevelsAtStart.boolValue)
            {
                StartNeedSatisfactionEditing();
            }

            EditorExtensionSettings.StandardMargin();
        }

        private void UpdateNeeds()
        {
            //1. make dict from serialized objects
            //1.1 make a list with all keys at the same time
            //2. iterate through needs and dict:
            //2.1 ask the dict whether it contains the key. 
            //2.2 if dict does not contain the key, add a new key with 0.0f - a need was not in the keys
            // If the need was found in the dict remove it from the second list
            // all keys that remain in the second list have no needs, and the kvPairs can be removed from the dict
            //"serialize" the dict

            int oldSize = serializedObject.FindProperty(StartingSatisfactionLevels_NeedKeys_arraySize).intValue;

            //Create temporary data structures out of which the new data will be compiled
            Dictionary<string, float> temporaryStartSatisfactionLevels = new Dictionary<string, float>();
            List<string> temporaryAllKeys = new List<string>();
            for (int i = 0; i < oldSize; i++)
            {
                string name = serializedObject.FindProperty(string.Format(StartingSatisfactionLevels_NeedKeys_access, i)).stringValue;
                float value = serializedObject.FindProperty(string.Format(StartingSatisfactionLevels_FloatValues_access, i)).floatValue;

                temporaryStartSatisfactionLevels.Add(name, value);
                temporaryAllKeys.Add(name);
            }

            for (int i = 0; i < needs.Length; i++)
            {
                if (temporaryStartSatisfactionLevels.ContainsKey(needs[i]))
                {
                    temporaryAllKeys.Remove(needs[i]);
                }
                else
                {
                    //Add all the new needs with a default value
                    temporaryStartSatisfactionLevels.Add(needs[i], 100.0f);
                }
            }
            //Delete all those needs that no longer exist
            foreach (string oldNeed in temporaryAllKeys)
            {
                temporaryStartSatisfactionLevels.Remove(oldNeed);
            }

            //"Serialize" the new data
            serializedObject.FindProperty(StartingSatisfactionLevels_NeedKeys_arraySize).intValue = temporaryStartSatisfactionLevels.Count;
            serializedObject.FindProperty(StartingSatisfactionLevels_FloatValues_arraySize).intValue = temporaryStartSatisfactionLevels.Count;

            int counter = 0;

            foreach (KeyValuePair<string, float> kvPair in temporaryStartSatisfactionLevels)
            {
                serializedObject.FindProperty(string.Format(StartingSatisfactionLevels_NeedKeys_access, counter)).stringValue = kvPair.Key;
                serializedObject.FindProperty(string.Format(StartingSatisfactionLevels_FloatValues_access, counter)).floatValue = kvPair.Value;

                counter++;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void StartNeedSatisfactionEditing()
        {
            EditorExtensionSettings.StandardMargin();

            for (int i = 0; i < serializedObject.FindProperty(StartingSatisfactionLevels_NeedKeys_arraySize).intValue; i++)
            {
                //Only show the needs in the species
                if (species.needs.Contains(serializedObject.FindProperty(string.Format(StartingSatisfactionLevels_NeedKeys_access, i)).stringValue))
                {

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(serializedObject.FindProperty(string.Format(StartingSatisfactionLevels_NeedKeys_access, i)).stringValue, EditorExtensionSettings.StandardEditorFieldWidth);

                        EditorGUI.BeginChangeCheck();

                        float value = serializedObject.FindProperty(string.Format(StartingSatisfactionLevels_FloatValues_access, i)).floatValue;

                        value = EditorGUILayout.FloatField(value, EditorExtensionSettings.StandardEditorFieldWidth);

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (value > GeneralSettings.DefaultNeedMaxValue)
                            {
                                value = GeneralSettings.DefaultNeedMaxValue;
                            }
                            if (value < GeneralSettings.DefaultNeedMinValue)
                            {
                                value = GeneralSettings.DefaultNeedMinValue;
                            }
                            serializedObject.FindProperty(string.Format(StartingSatisfactionLevels_FloatValues_access, i)).floatValue = value;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

            }
        }

        private void SlotEditing()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorExtensionSettings.StandardMargin();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(Simulation.Strings.NumberOfSlotsLabel, EditorExtensionSettings.StandardEditorFieldWidth);
                    EditorExtensionSettings.StandardMargin();
                    EditorGUI.BeginChangeCheck();
                    int oldSize = serializedObject.FindProperty(arraySizeWorldSlotPositions).intValue;

                    if (serializedObject.FindProperty(arraySizeWorldSlotPositions).intValue != serializedObject.FindProperty(arraySizeLocalLookAtTarget).intValue)
                    {
                        Debug.LogError("A mismatch in array lenghts has been detected for slots.");
                    }

                    int newSize = serializedObject.FindProperty(arraySizeWorldSlotPositions).intValue;

                    newSize = EditorGUILayout.IntSlider(newSize, 0, EditorExtensionSettings.MaxSlots, EditorExtensionSettings.StandardEditorFieldWidth);

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.FindProperty(arraySizeLocalSlotPositions).intValue = newSize;
                        serializedObject.FindProperty(arraySizeAuctionable).intValue = newSize;
                        serializedObject.FindProperty(arraySizeLocalLookAtTarget).intValue = newSize;
                        serializedObject.FindProperty(arraySizeWorldSlotPositions).intValue = newSize;

                        if (oldSize < newSize)
                        {
                            for (int i = oldSize; i < newSize; i++)
                            {
                                //world position
                                serializedObject.FindProperty(string.Format(accessWorldSlotPositions, i)).vector3Value = transform.TransformPoint(SpawningPosition(i));
                                //local position
                                serializedObject.FindProperty(string.Format(accessLocalSlotPositions, i)).vector3Value = SpawningPosition(i);
                                serializedObject.FindProperty(string.Format(accessLocalLookAtTarget, i)).vector3Value = Vector3.zero;
                                serializedObject.FindProperty(string.Format(accessAuctionable, i)).boolValue = true;
                            }
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorExtensionSettings.StandardMargin();

                for (int i = 0; i < serializedObject.FindProperty(arraySizeLocalSlotPositions).intValue; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUI.BeginChangeCheck();

                        Vector3 localPosition = serializedObject.FindProperty(string.Format(accessLocalSlotPositions, i)).vector3Value;
                        //local position
                        serializedObject.FindProperty(string.Format(accessLocalSlotPositions, i)).vector3Value
                            = EditorGUILayout.Vector3Field(Simulation.Strings.SlotNumberLabel(i), localPosition);

                        if (EditorGUI.EndChangeCheck())
                        {
                            //world position
                            serializedObject.FindProperty(string.Format(accessWorldSlotPositions, i)).vector3Value
                                = transform.TransformPoint(serializedObject.FindProperty(string.Format(accessLocalSlotPositions, i)).vector3Value);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (ModifyLookAt.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            serializedObject.FindProperty(string.Format(accessLocalLookAtTarget, i)).vector3Value
                                = EditorGUILayout.Vector3Field("Look at target", serializedObject.FindProperty(string.Format(accessLocalLookAtTarget, i)).vector3Value);

                            //Use this if you want the option to make slots not available to agents.
                            //CustomGUI.StandardMargin();
                            //serializedObject.FindProperty(string.Format(accessAuctionable, i)).boolValue
                            //    = EditorGUILayout.ToggleLeft(Simulation.Strings.IsAuctionableLabel,
                            //                                 serializedObject.FindProperty(string.Format(accessAuctionable, i)).boolValue,
                            //                                 EditorSettings.StandardEditorFieldWidth);

                        }
                        EditorGUILayout.EndHorizontal();
                        EditorExtensionSettings.StandardMargin();
                    }
                }
                ModifyLookAt.boolValue = EditorGUILayout.ToggleLeft(Simulation.Strings.ModifyLookAt, ModifyLookAt.boolValue, EditorExtensionSettings.MediumEditorFieldWidth);
                ShowDebug.boolValue = EditorGUILayout.ToggleLeft(Simulation.Strings.ShowDebugGizmosLabel, ShowDebug.boolValue, EditorExtensionSettings.MediumEditorFieldWidth);
            }
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Handle Drawing

        private void DisplayHandles()
        {
            int size = serializedObject.FindProperty(arraySizeWorldSlotPositions).intValue;
            if (size > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    EditorGUI.BeginChangeCheck();

                    //world position
                    serializedObject.FindProperty(string.Format(accessWorldSlotPositions, i)).vector3Value
                    = CustomGUI.DrawSlot(serializedObject.FindProperty(string.Format(accessWorldSlotPositions, i)).vector3Value, serializedObject.FindProperty(string.Format(accessAuctionable, i)).boolValue);

                    if (EditorGUI.EndChangeCheck())
                    {
                        //local position
                        serializedObject.FindProperty(string.Format(accessLocalSlotPositions, i)).vector3Value = transform.InverseTransformPoint(serializedObject.FindProperty(string.Format(accessWorldSlotPositions, i)).vector3Value);
                    }
                }
            }
        }

        private bool TransformWasMoved()
        {
            Vector3 comparePosition = transform.position;
            Vector3 compareEulerAngles = transform.rotation.eulerAngles;
            Vector3 compareLossyScale = transform.lossyScale;

            if (lastTransformPosition.x != comparePosition.x || lastTransformPosition.y != comparePosition.y || lastTransformPosition.z != comparePosition.z)
            {
                lastTransformPosition = comparePosition;
                return true;
            }

            if (lastEulerAngles.x != compareEulerAngles.x || lastEulerAngles.y != compareEulerAngles.y || lastEulerAngles.z != compareEulerAngles.z)
            {
                lastEulerAngles = compareEulerAngles;
                return true;
            }

            if (lastScale.x != compareLossyScale.x || lastScale.y != compareLossyScale.y || lastScale.z != compareLossyScale.z)
            {
                lastScale = compareLossyScale;
                return true;
            }

            return false;
        }

        private void UpdateSlotPostionsToTransformMovement()
        {
            if (!TransformWasMoved())
            {
                return;
            }

            if (serializedObject.FindProperty(arraySizeWorldSlotPositions).intValue > 0)
            {
                for (int i = 0; i < serializedObject.FindProperty(arraySizeWorldSlotPositions).intValue; i++)
                {
                    EditorGUI.BeginChangeCheck();

                    serializedObject.FindProperty(string.Format(accessWorldSlotPositions, i)).vector3Value
                        = transform.TransformPoint(serializedObject.FindProperty(string.Format(accessLocalSlotPositions, i)).vector3Value);

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.FindProperty(string.Format(accessLocalSlotPositions, i)).vector3Value
                            = transform.InverseTransformPoint(serializedObject.FindProperty(string.Format(accessWorldSlotPositions, i)).vector3Value);
                    }
                }
            }

            lastTransformPosition = transform.position;
            lastEulerAngles = transform.rotation.eulerAngles;
            lastScale = transform.lossyScale;
        }

        #endregion
    }
}
#endif
