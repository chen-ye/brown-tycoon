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
    /// For Value-oriented behaviors: Try to get a slot based on utility of all available slots (relative to the current satisfaction level of the needs of the agent).
    /// </summary>
    public class DecideValue : Action
    {
        public DecideValue(NEEDSIMNode agent)
                : base(agent)
        { }

        public override string Name
        {
            get
            {
                return "DecideValue";
            }
        }

        /// <summary>
        /// Try to get a slot based on utility of all available slots (relative to the current satisfaction level of the needs of the agent).
        /// </summary>
        /// <returns>Success if a slot has been distributed to the agent.</returns>
        public override Action.Result Run()
        {
            if (agent.Blackboard.currentState == Blackboard.AgentState.ExitNEEDSIMBehaviors)
            {
                //Actions should be interrupted until the agent state is dealt with.
                return Result.Failure;
            }

            //If previously a slot was allocated, try to consume it.
            if (agent.Blackboard.currentState == Blackboard.AgentState.WaitingForSlot)
            {
                if (agent.AcceptSlot())
                {
                    return Result.Success;
                }
                else
                {
                    agent.Blackboard.currentState = Blackboard.AgentState.PonderingNextAction;
                }
            }

            //Try to allocate a slot to the agent that has the highest value regardless of state.
            Simulation.Bidding.Result biddingResult = Simulation.Bidding.ValueOrientedBid(agent.AffordanceTreeNode);

            //Simulation.Bidding.BidParentGrandparentRoot(agent.AffordanceTreeNode);
            if (biddingResult == Simulation.Bidding.Result.Success)
            {
                agent.Blackboard.currentState = Blackboard.AgentState.WaitingForSlot;
                return Result.Running;
            }

            agent.Blackboard.currentState = Blackboard.AgentState.None;
            return Result.Failure;
        }
    }
}