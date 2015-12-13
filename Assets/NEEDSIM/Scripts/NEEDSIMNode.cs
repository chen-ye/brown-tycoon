/**
 * This is part of the NEEDSIM Life simulation plug in for Unity, version 1.0.1
 * Copyright 2014 - 2015 Fantasieleben UG (haftungsbeschraenkt)
 *
 * http;//www.fantasieleben.com for further details.
 *
 * For questions please get in touch with Tilman Geishauser: tilman@fantasieleben.com
 */

using UnityEngine;
using Simulation;
using System.Collections;
using System.Collections.Generic;

namespace NEEDSIM
{
    /// <summary>
    /// Every object and agent in NEEDSIM Life simulation has a NEEDSIMNode: This is the essential component for using NEEDSIM Life simulation.
    /// </summary>
    [System.Serializable]
    public class NEEDSIMNode : MonoBehaviour
    {
        #region Fields set in the inspector view

        public string[] InteractionData;
        public bool isAgent;
        public float o_Space;
        public bool drawGizmosInGame;
        public bool ModifyLookAt;
        public Vector3[] slotPositionsWorldSpace;
        public Vector3[] slotPositionsLocalSpace;
        public Vector3[] slotLocalLookAtTarget;
        public bool[] auctionableBool;
        public bool showDebugInGame;
        public bool showDebugInInspector;
        public string speciesName;
        public ExamplePlans selectedPlan;

        public float[] StartingSatisfactionLevels_FloatValues;
        public string[] StartingSatisfactionLevels_NeedKeys;
        public bool SpecificSatisfactionLevelsAtStart;

        #endregion

        #region Agent

        public Blackboard Blackboard { get; set; }
        private PlanDemo planDemo; //This is an example controller for agents, to make integration into other solutions faster.
        public enum ExamplePlans { GoalOriented, ValueOriented, GoalOrientedChase, InterruptionNachtDesFuchses }
        //Goal oriented behavior is more efficient, and allows to search the data structure specifically for interactions that help satisfy the respective need. 
        //Value oriented behavior puts more emphasis on the utility of any need satisfaction. 
        //The chase and interruption behaviors are examples to clarify how you could use sequences of actions in your game.

        public bool runningInteractions = false;

        #endregion

        public enum AnimationOrders { MovementStartedByAgent, InteractionStartedByAgent, InteractionRunning };
        public Stack<AnimationOrders> AnimationsToPlay; // A helper to get animation states from this class. 

        public AffordanceTreeNode AffordanceTreeNode { get; set; } // The simulation object used in the NEEDSIM Simulation

        void Awake()
        {
            AnimationsToPlay = new Stack<AnimationOrders>();
        }

        void Start()
        {
            if (NEEDSIMRoot.Instance.isSimulationInitialized)
            {
                if (AffordanceTreeNode == null)
                {
                    Debug.LogError(" AffordanceTreeNode not set up. (" + this.name + ")");
                }
                else
                {
                    SetUpInteractions();
                    SetUpSlots();
                    ValidateSpecies();

                    if (isAgent)
                    {
                        SetUpAgent();
                    }
                }
            }
        }

        /// <summary>
        /// Recursively build an Affordance Tree from the scene hierarchy. This method will not work for intermediate objects in the hierarchy - there is only deeper search if a direct ancestor is a NEEDSIMNode.
        /// </summary>
        public void BuildTreeBasedOnSceneHierarchy()
        {
            NEEDSIM.NEEDSIMNode[] children = GetComponentsInChildren<NEEDSIM.NEEDSIMNode>();
            Debug.Log(children.Length.ToString());

            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].transform.parent == this.transform)
                {
                    NEEDSIMRoot.Instance.AddNEEDSIMNode(children[i], this);
                    children[i].BuildTreeBasedOnSceneHierarchy();
                }
            }
        }

        /// <summary>
        /// Catch errors. If you are sure your data is correct you can strip most of it out for release versions.
        /// </summary>
        #region setting up general NEEDSIM Node

        private void SetUpInteractions()
        {
            foreach (string name in InteractionData)
            {
                if (name != "" && name != Simulation.Strings.None)
                {
                    if (!Manager.Instance.Data.InteractionByNameDictionary.ContainsKey(name))
                    {
                        Debug.LogError("At " + gameObject.name + ": Interaction " + name + " not found. Please check whether the correct database is selected in the SimulationManager.");
                    }
                    else
                    {
                        Interaction interaction = Manager.Instance.Data.InteractionByNameDictionary[name];

                        bool hasUnassignedInteraction = false;
                        foreach (string affectedNeed in interaction.SatisfactionRates.Keys)
                        {
                            if (affectedNeed == Simulation.Strings.AssignNeedLabel)
                            {
                                Debug.LogError("There is an unassigned need in the interaction " + interaction + ". The interaction" +
                                    "was not added to objects in the scene.");
                                hasUnassignedInteraction = true;
                            }
                        }
                        if (!hasUnassignedInteraction)
                        {
                            // Add the interaction to the simulation
                            AffordanceTreeNode.Affordance.AddInteraction(interaction);
                        }
                    }
                }
                else if (!isAgent)
                {
                    Debug.LogWarning("Unnamed Interaction at object that is not an agent. Please assign an interaction.");
                }
            }
        }

        private void SetUpSlots()
        {
            for (int i = 0; i < slotPositionsWorldSpace.Length; i++)
            {
                // Try to add the slot to the simulation
                if (!AffordanceTreeNode.AddSlot(slotPositionsWorldSpace[i], slotPositionsLocalSpace[i], slotLocalLookAtTarget[i], auctionableBool[i]))
                {
                    Debug.LogWarning("Slot not added");
                }
            }
        }

        private void ValidateSpecies()
        {
            if (isAgent && !(GameDataManager.SpeciesLoaded(speciesName)))
            {
                isAgent = false;
                Debug.LogWarning("Species not found.");
            }
        }

        #endregion

        private bool SetUpAgent()
        {
            if (!SpecificSatisfactionLevelsAtStart)
            {
                // Start with the agent's need at random levels
                AffordanceTreeNode.SatisfactionLevels.RandomizeValues();
            }
            else {
                if (StartingSatisfactionLevels_FloatValues.Length == 0 ||
   StartingSatisfactionLevels_NeedKeys.Length == 0)
                {
                    Debug.LogError("No start values set.");
                    return false;
                }

                Dictionary<string, float> result = new Dictionary<string, float>();

                for (int i = 0; i < StartingSatisfactionLevels_FloatValues.Length; i++)
                {
                    result.Add(StartingSatisfactionLevels_NeedKeys[i], StartingSatisfactionLevels_FloatValues[i]);
                }

                AffordanceTreeNode.SatisfactionLevels.SetSpecficSatisfactionLevels(result);
            }

            //Blackboard and planDemo are examples on how to control agents.
            //You might want use parts of the code for integration into your solution.
            Blackboard = new Blackboard(gameObject);

            planDemo = new PlanDemo(this);

            return true;
        }

        void Update()
        {
            if (NEEDSIMRoot.Instance.isSimulationInitialized && AffordanceTreeNode != null)
            {
                //Positions are updated in case the object was moved at runtime.
                foreach (Slot slot in AffordanceTreeNode.Slots)
                {
                    slot.Position = transform.TransformPoint(slot.LocalPosition);
                    slot.LookAt = transform.TransformPoint(slot.LocalLookAt);
                }

                //Whether an interaction is currently running
                runningInteractions = AffordanceTreeNode.Affordance.CurrentInteraction != null;

                if (isAgent)
                {
                    // This applies need change rates, for example a decay for the hunger need makes the agent more hungry over time.
                    AffordanceTreeNode.SatisfactionLevels.ApplyChangePerSecond();

                    // Our example agent control. You might not have to call this if you use your own solution.
                    if (planDemo != null)
                    {
                        planDemo.Update();
                    }
                }
            }
            else if (AffordanceTreeNode == null)
            {
                Debug.LogWarning("AffordanceTreeNode not yet set up.");
            }
        }

        /// <summary>
        /// If the simulation distributed a slot to this agent try to accept it.
        /// </summary>
        /// <returns>Whether the slot was accepted</returns>
        public bool AcceptSlot()
        {
            Slot slot = AffordanceTreeNode.AvailableSlot(true);

            if (slot == null || !isAgent)
            {
                return false;
            }

            bool slotAccepted = Blackboard.AcceptSlot(slot);
            if (slotAccepted)
            {
                AnimationsToPlay.Push(AnimationOrders.MovementStartedByAgent);
            }

            return slotAccepted;
        }

        /// <summary>
        /// This method tries to call the AgentArrivalEasy() method at the slot it is passed to.
        /// </summary>
        /// <param name="slot">The slot this agent arrives to</param>
        /// <returns>Whether the arrival at a slot by this agent was successful</returns>
        public bool ArrivalAtSlot(Slot slot)
        {
            // Only when the smart object is an agent it can arrive at slots that belong to other smart objects.
            if (slot == null || !isAgent)
            {
                return false;
            }

            Slot.Result result = slot.AgentArrivalEasy(this.AffordanceTreeNode);
            if (!(result == Slot.Result.Success))
            {
                if (result == Slot.Result.InteractionAlreadyRunning)
                {
                    //In some situations this might be useful debug information
                    //Debug.Log("No new interaction started - participating in currently running interaction.");
                }
                else
                {
                    Debug.LogWarning("agent arrival failure: " + slot.Position.ToString());
                    return false;
                }
            }

            //When arriving at a slot an interaction should be started, unless one was already running.
            if (slot.currentInteraction == null)
            {
                Debug.LogWarning("no interaction: " + slot.Position.ToString());
                return false;
            }

            Blackboard.currentState = Blackboard.AgentState.ParticipatingSlot;
            AnimationsToPlay.Push(AnimationOrders.InteractionStartedByAgent);

            return true;
        }

        /// <summary>
        /// This method assumes that in the animator a trigger named "Movement" exists, and that for each interaction
        /// a trigger with the same name exists in the animator, e.g. that for the interaction "Eat" a trigger named "Eat" is
        /// in the animation and is used to transition into a state or sub-state-machine that plays animation(s) for eating.
        /// </summary>
        /// <param name="animator"></param>
        /// <returns>Whether the most recent animation order was consumed.</returns>
        public bool TryConsumingAnimationOrder(Animator animator)
        {
            if (animator == null)
            {
                NEEDSIMRoot.Instance.WriteToSimulationDebugLog("Animator was null. At: " + gameObject.name);
                return false;
            }

            if (AnimationsToPlay.Count > 0)
            {
                if (AnimationsToPlay.Peek() == NEEDSIM.NEEDSIMNode.AnimationOrders.MovementStartedByAgent)
                {
                    animator.SetTrigger("Movement");
                    AnimationsToPlay.Pop();
                    return true;
                }
                else if (AnimationsToPlay.Peek() == NEEDSIM.NEEDSIMNode.AnimationOrders.InteractionStartedByAgent)
                {
                    if (Blackboard.activeSlot.currentInteraction != null)
                    {
                        animator.SetTrigger(Blackboard.activeSlot.currentInteraction.Name);
                        AnimationsToPlay.Pop();
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("Interaction not running.");
                    }
                }
            }
            return false;
        }

        void OnDestroy()
        {
            if (NEEDSIMRoot.Instance != null && NEEDSIMRoot.Instance.isSimulationInitialized)
            {
                //Only agents have blackboards. If they have active slots, they should be freed up for other agents.
                if (Blackboard != null && Blackboard.activeSlot != null)
                {
                    Blackboard.activeSlot.AgentDeparture();
                    NEEDSIMRoot.Instance.Blackboards.Remove(Blackboard);
                }

                foreach (Slot slot in AffordanceTreeNode.Slots)
                {
                    slot.SetSlotBlocked();
                    slot.InterruptInteraction();
                }
                AffordanceTreeNode.Remove();
            }
        }

        void OnGUI()
        {
            if (showDebugInGame && NEEDSIMRoot.Instance.isSimulationInitialized && AffordanceTreeNode != null)
            {
                Vector3 GUIposition = Camera.main.WorldToScreenPoint(this.transform.position);
                int textAreaHeight = 50;
                string textAreaContent = speciesName + "\n\n";

                if (AffordanceTreeNode.Species == null)
                {
                    textAreaHeight += 15;
                    textAreaContent += "No species found";
                }
                else
                {
                    foreach (string need in AffordanceTreeNode.Species.needs)
                    {
                        try
                        {
                            textAreaContent += need + ": " + AffordanceTreeNode.SatisfactionLevels.GetValue(need).ToString("F2") + "\n";
                        }
                        catch (KeyNotFoundException)
                        {
                            Debug.LogError("Species data not in sync with Affordance Tree node.");
                        }
                        textAreaHeight += 20;
                    }

                    textAreaHeight += 25;
                    if (AffordanceTreeNode.Goal != null)
                    {
                        textAreaContent += "\nGoal: " + AffordanceTreeNode.Goal.NeedToSatisfy;
                        textAreaContent += "\nState: " + AffordanceTreeNode.Goal.SatisfactionState.ToString();
                    }
                    else
                    {
                        Debug.LogWarning("Goal was null.");
                    }

                    //planDemo is an optional tool for agent control
                    if (planDemo != null)
                    {
                        textAreaHeight += 15;
                        textAreaContent += "\nAction: " + planDemo.printCurrentAction();
                    }
                }

                textAreaContent = GUI.TextArea(new Rect(GUIposition.x, Screen.height - GUIposition.y - 120, 140, textAreaHeight), textAreaContent);
            }
        }

#if UNITY_EDITOR

        void OnDrawGizmos()
        {
            if (Application.isPlaying
                && drawGizmosInGame
                && NEEDSIMRoot.Instance.isSimulationInitialized)
            {
                DrawSlots();
            }
        }

        private void DrawSlots()
        {
            foreach (Slot slot in AffordanceTreeNode.Slots)
            {
                switch (slot.SlotState)
                {
                    case Slot.SlotStates.ReadyForAuction:
                        Gizmos.color = GeneralSettings.AuctionableSlotColor;
                        break;
                    case Slot.SlotStates.Blocked:
                        Gizmos.color = GeneralSettings.BlockedSlotColor;
                        break;
                    case Slot.SlotStates.ReadyCharacter:
                        Gizmos.color = GeneralSettings.ReadyCharacterSlotColor;
                        break;
                    case Slot.SlotStates.Reserved:
                        Gizmos.color = GeneralSettings.ReservedSlotColor;
                        break;
                    default:
                        Gizmos.color = GeneralSettings.AuctionableSlotColor;
                        break;
                }

                Gizmos.DrawWireSphere(slot.Position, GeneralSettings.SlotRepresentationRadius);
            }
        }
#endif
    }
}
