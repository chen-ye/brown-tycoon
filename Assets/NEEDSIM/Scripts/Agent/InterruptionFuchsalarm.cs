/**
 * This is part of the NEEDSIM Life simulation plug in for Unity, version 1.0.1
 * Copyright 2014 - 2015 Fantasieleben UG (haftungsbeschraenkt)
 *
 * http;//www.fantasieleben.com for further details.
 *
 * For questions please get in touch with Tilman Geishauser: tilman@fantasieleben.com
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NEEDSIM
{
    /// <summary>
    /// This action demonstrates how interruption of typical NEEDSIM behaviors could look like.
    /// </summary>
    public class InterruptionFuchsalarm : Action
    {
        NEEDSIMSampleSceneScripts.FuchsalarmDemoScript scriptReference;
        bool movementStarted;

        public InterruptionFuchsalarm(NEEDSIMNode agent)
            : base(agent)
        {
            scriptReference = GameObject.FindObjectOfType<NEEDSIMSampleSceneScripts.FuchsalarmDemoScript>();
        }

        public override string Name
        {
            get
            {
                return "InterruptionFuchsalarm";
            }
        }

        /// <summary>
        /// Satisfying a goal at an AffordanceTree node.
        /// </summary>
        /// <returns>Success if need satsifaction goal was achieved, running whilst it is being satisfied. </returns>
        public override Action.Result Run()
        {
            if (scriptReference == null)
            {
                Debug.LogError("This action was specifically made to show interuptible behavior in the Nacht des Fuchses example scene.");
                return Result.Failure;
            }

            //If the fox has been survived it is time to go back to other plans
            if (!scriptReference.FoxAlive)
            {
                movementStarted = false;
                return Result.Success;
            }
            //if the agent is not yet running he/she should hurry to the safe zone
            if (!movementStarted)
            {
                agent.gameObject.GetComponentInChildren<Animator>().SetTrigger("Movement");

                agent.Blackboard.NavMeshAgent.SetDestination(scriptReference.SafeZone);
                movementStarted = true;
            }
           
            return Result.Running;
        }
    }
}