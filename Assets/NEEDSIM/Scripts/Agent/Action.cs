/**
 * This is part of the NEEDSIM Life simulation plug in for Unity, version 1.0.1
 * Copyright 2014 - 2015 Fantasieleben UG (haftungsbeschraenkt)
 *
 * http;//www.fantasieleben.com for further details.
 *
 * For questions please get in touch with Tilman Geishauser: tilman@fantasieleben.com
 */

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace NEEDSIM
{
    /// <summary>
    /// We hope we wrote our example actions in a way that they can be integrated into your
    /// Finite State Machine, Behavior Tree, or Planner. We provide a sample use of our sample
    /// actions in the PlanDemo.cs class.
    /// </summary>
    public abstract class Action
    {
        protected NEEDSIMNode agent;

        public enum Result { Failure = 0, Success, Running }

        public abstract string Name { get; }

        public Action(NEEDSIMNode agent)
        {
            this.agent = agent;
        }

        public abstract Result Run();
    }
}