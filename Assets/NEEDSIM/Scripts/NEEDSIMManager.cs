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

namespace NEEDSIM
{
    /// <summary>
    /// This class stores the values that the NEEDSIMROOT will use for running the simulation
    /// </summary>
    [System.Serializable]
    public class NEEDSIMManager : MonoBehaviour
    {
        public bool buildAffordanceTreeFromScene = true; // Whether to build the Affordance Tree, the data structure for the simulation, upon awake.
        public bool LogSimulation = true; // Set this to false or strip out the code if you need every tiny bit of performance
        public bool PrintSimulationDebugLog = false; // Can be activated in the inspector via a button
        public string databaseName = Simulation.Strings.DefaultDatabaseName; //This database will be loaded
        public bool reportDetailedInformation = true; //This is not yet shown in the Inspector, as we did not want to clutter the UI.

        void Awake()
        {
            NEEDSIMRoot.Instance.processScene();
        }

        public static void PrintSimulationDebugLogToConsole()
        {
            NEEDSIMRoot.Instance.PrintSimulationDebugLogToConsole();
        }
    }

}
