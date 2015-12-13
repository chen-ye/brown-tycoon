using UnityEngine;
using System.Collections;

namespace Tycoon
{
    [System.Serializable]
    public class SimpleRelativeSpawn : MonoBehaviour
    {
        [Tooltip("The game object that will be spawned.")]
        public GameObject Prefab;
        [Tooltip("If this is true the population will always be maxed.")]
        public bool KeepAllAlive;
        public float RandomDistance = 0.0f;
        [Tooltip("The length of this array is the max population number.")]
        public Vector3[] spawnRelativeLocations;
        public Transform SpawnOrigin;
        

        public int PopulationCount { get; private set; } //How many instances are alive.

        private GameObject[] spawnedObjects;

        void Awake()
        {
            spawnedObjects = new GameObject[spawnRelativeLocations.Length];
            for (int i = 0; i < spawnedObjects.Length; i++)
            {
                spawnedObjects[i] = null;
            }
        }

        void Update()
        {
            if (KeepAllAlive)
            {
                fillPopulation();
            }
        }

        /// <summary>
        /// Spawn an instance of the prefab for each spawning point at which the previously (if any) spawned instance is dead (null) 
        /// </summary>
        public void fillPopulation()
        {
            for (int i = 0; i < spawnedObjects.Length; i++)
            {
                if (spawnedObjects[i] == null)
                {
                    spawnedObjects[i] = GameObject.Instantiate(Prefab);
                    PopulationCount++;

                    NEEDSIM.NEEDSIMRoot.Instance.AddNEEDSIMNode(spawnedObjects[i].GetComponent<NEEDSIM.NEEDSIMNode>());

                    spawnedObjects[i].transform.position = SpawnOrigin.position + spawnRelativeLocations[i] + new Vector3(Random.Range(-RandomDistance,RandomDistance),0,Random.Range(-RandomDistance,RandomDistance));
                }
            }
        }

        /// <summary>
        /// Remove all agents from the slots they currently paticipate in and delete them.
        /// </summary>
        public void killAll()
        {
            for (int i = 0; i < spawnedObjects.Length; i++)
            {
                Simulation.Slot activeSlot = spawnedObjects[i].GetComponent<NEEDSIM.NEEDSIMNode>().Blackboard.activeSlot;

                if (activeSlot != null)
                {
                    activeSlot.AgentDeparture();
                }
                else { Debug.Log("Active slot was null"); }

                GameObject.Destroy(spawnedObjects[i]);
                PopulationCount--;
            }
        }
    }

}
