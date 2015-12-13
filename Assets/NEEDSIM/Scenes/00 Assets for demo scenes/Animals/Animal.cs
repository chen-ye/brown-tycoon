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

namespace NEEDSIMSampleSceneScripts
{
    /// <summary>
    /// This example suggests an idea for playing animations based on the states of NEEDSIMNodes.
    /// </summary>
    [RequireComponent(typeof(NEEDSIM.NEEDSIMNode))]
    public class Animal : MonoBehaviour
    {
        protected Animator animator;
        protected NEEDSIM.NEEDSIMNode needsimNode;

        public virtual void Start()
        {
            animator = GetComponentInChildren<Animator>();
            needsimNode = GetComponent<NEEDSIM.NEEDSIMNode>();
        }

        public virtual void Update()
        {
            if (needsimNode.AnimationsToPlay.Count > 0)
            {
                if (needsimNode.AnimationsToPlay.Peek() == NEEDSIM.NEEDSIMNode.AnimationOrders.MovementStartedByAgent)
                {
                    //Rotate agent into movement direction
                    gameObject.transform.LookAt(needsimNode.GetComponent<NavMeshAgent>().steeringTarget);
                    gameObject.transform.Rotate(90.0f, gameObject.transform.rotation.y, gameObject.transform.rotation.z);
                }
                else if (needsimNode.AnimationsToPlay.Peek() == NEEDSIM.NEEDSIMNode.AnimationOrders.InteractionStartedByAgent)
                {
                    //Rotate agent towards LookAt 
                    gameObject.transform.LookAt(needsimNode.Blackboard.activeSlot.LookAt);
                }
                //This method will call the SetTrigger method on the animator, thus triggering correctly named transitions into animation states.
                needsimNode.TryConsumingAnimationOrder(animator);
            }
        }
    }
}
