using UnityEngine;
using System.Collections;
using System;

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
        public Transform SpawnParent;

        public int Stagger = 4;
        private int currentIndex = 0;
        

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

        void Start()
        {
            if (SpawnParent == null)
            {
                SpawnParent = GameObject.FindGameObjectWithTag("AgentManager").transform;
            }
        }

        void Update()
        {
            if (KeepAllAlive)
            {
                StartCoroutine(StaggeredFillPopulation());
            }
        }

        public IEnumerator StaggeredFillPopulation()
        {
            while (currentIndex < spawnedObjects.Length) {
                for (int i = currentIndex; i < Math.Min(spawnedObjects.Length, currentIndex + Stagger); i++)
                {
                    if (spawnedObjects[i] == null)
                    {
                        SpawnObject(i);
                    }
                }
                currentIndex += Stagger;
                yield return null;
            }
        }

        ///// <summary>
        ///// Spawn an instance of the prefab for each spawning point at which the previously (if any) spawned instance is dead (null) 
        ///// </summary>
        //public void fillPopulation()
        //{
        //    for (int i = 0; i < spawnedObjects.Length; i++)
        //    {
        //        if (spawnedObjects[i] == null)
        //        {
        //            SpawnObject(i);
        //        }
        //    }
        //}

        public void SpawnObject(int i)
        {
            spawnedObjects[i] = GameObject.Instantiate(Prefab, SpawnOrigin.position + spawnRelativeLocations[i] + new Vector3(UnityEngine.Random.Range(-RandomDistance, RandomDistance), 0, UnityEngine.Random.Range(-RandomDistance, RandomDistance)), Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up)) as GameObject;
            PopulationCount++;


            if (SpawnParent != null)
            {
                spawnedObjects[i].transform.parent = SpawnParent;
                NEEDSIM.NEEDSIMRoot.Instance.AddNEEDSIMNode(spawnedObjects[i].GetComponent<NEEDSIM.NEEDSIMNode>(), SpawnParent.GetComponent<NEEDSIM.NEEDSIMNode>());
            }
            else
            {
                NEEDSIM.NEEDSIMRoot.Instance.AddNEEDSIMNode(spawnedObjects[i].GetComponent<NEEDSIM.NEEDSIMNode>());
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
