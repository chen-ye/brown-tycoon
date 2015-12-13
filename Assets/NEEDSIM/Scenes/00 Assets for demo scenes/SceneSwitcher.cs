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
    /// public methods to switch scenes via a button click. You have to add the scenes to your build settings to use the prefab that uses this script.
    /// </summary>
    public class SceneSwitcher : MonoBehaviour
    {
        string[] levelNames = { "01 Naturleben", "02 Hasenjagd", "03 Fuchsalarm", "04 Simple Room", "05 Spawn Beds", "06 Simple Time"  };

        public void loadLevel00()
        {
            Application.LoadLevel(levelNames[0]);
        }

        public void loadLevel01()
        {
            Application.LoadLevel(levelNames[1]);
        }

        public void loadLevel02()
        {
            Application.LoadLevel(levelNames[2]);
        }

        public void loadLevel03()
        {
            Application.LoadLevel(levelNames[3]);
        }

        public void loadLevel04()
        {
            Application.LoadLevel(levelNames[4]);
        }

        public void loadLevel05()
        {
            Application.LoadLevel(levelNames[5]);
        }
    }
}