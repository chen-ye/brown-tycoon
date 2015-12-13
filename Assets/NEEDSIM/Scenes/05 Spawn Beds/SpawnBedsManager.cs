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
    /// This example shows how you could spawn all the objects and agents procedurally.
    /// </summary> 
    public class SpawnBedsManager : MonoBehaviour
    {
        [Tooltip("The characters that will be spawned. Prefab needs NEEDSIM Node.")]
        public GameObject PrefabSimpleVillager;
        [Tooltip("The script spawns one dinner table. Prefab needs NEEDSIM Node.")]
        public GameObject PrefabDinnerTable;
        [Tooltip("Bed prefabs will be spawned when the button is pressed. Prefab needs NEEDSIM Node.")]
        public GameObject PrefabBed;

        private GameObject[] simpleVillagers = new GameObject[4]; // Four characters will be spawned.

        private GameObject DinnerTable_01;
        // Whilst there is only one dinner table, there can be between 0 and 4 beds in the scene, at fixed positions.
        private int bedCounter = 0;
        private GameObject[] beds;
        private readonly int[] bedPositions = { -3, -1, 1, 3 };

        void Start()
        {
            // Spawn four simple villager NPCs.
            for (int i = 0; i < simpleVillagers.Length; i++)
            {
                simpleVillagers[i] = GameObject.Instantiate(PrefabSimpleVillager);
            }

            // At start, there are no beds to sleep in, so agents might stand around.
            beds = new GameObject[4];

            // Spawn a dinner table, so people can eat.
            DinnerTable_01 = GameObject.Instantiate(PrefabDinnerTable);
            DinnerTable_01.transform.Translate(0, 0, 4);

            // Because we spawned objects from the script, we manually build the Affordance Tree data structure for the simulation.
            // Later on we can call AddNEEDSIMNode to add further objects.
            NEEDSIM.NEEDSIMRoot.Instance.BuildFlatAffordanceTreeFromScene();
        }

        void Update()
        {

        }

        /// <summary>
        /// Create an instance of a bed, add it to the simulation, and translate it to the right position.
        /// </summary>
        public void SpawnBed()
        {
            if (bedCounter < 4) // There can only be four beds in the sample scene.
            {
                // Create a bed game object in Unity from the prefab.
                beds[bedCounter] = GameObject.Instantiate(PrefabBed);
                // Add the NEEDSIMNode of the bed to the simulation.
                NEEDSIM.NEEDSIMRoot.Instance.AddNEEDSIMNode(beds[bedCounter].GetComponent<NEEDSIM.NEEDSIMNode>());
                // Move the bed into the correct, predefined position.
                beds[bedCounter].transform.Translate(bedPositions[bedCounter], 0, -2);
                bedCounter++;
            }
        }

        /// <summary>
        /// Destroy an instance of a bed.
        /// </summary>
        public void DestroyBed()
        {
            if (bedCounter > 0)
            {
                GameObject.Destroy(beds[bedCounter - 1]);
                bedCounter--;
            }
        }
    }
}
;