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
    /// This example script shows how the bunny can deal with the 'EatBunny' interaction, that can be performed by a fox at the slot provided by a bunny.
    /// </summary>
    [RequireComponent(typeof(NEEDSIM.NEEDSIMNode))]
    public class Bunny : Animal
    {
        bool isBeingEaten = false;
        bool hasBeenBeingEaten = false; //i.e. is totally dead.

        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();

            //if the bunny is running interactions it means he is being eaten.
            if (needsimNode.runningInteractions)
            {
                isBeingEaten = true;

                if (!hasBeenBeingEaten)
                {
                    animator.SetTrigger("Die");
                    GetComponent<NavMeshAgent>().Stop();
                    needsimNode.isAgent = false;
                    hasBeenBeingEaten = true;
                }
            }

            //Once the interaction of eating the bunny is not running anymore the bunny can be destroyed.
            if (isBeingEaten && !needsimNode.runningInteractions)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
