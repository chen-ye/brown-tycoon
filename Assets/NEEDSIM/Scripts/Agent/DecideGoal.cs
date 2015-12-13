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
    /// For goal oriented behaviors: Get a goal from the simulation, and try to get a slot where the goal can be satisfied.
    /// </summary>
    public class DecideGoal : Action
    {
        public DecideGoal(NEEDSIMNode agent)
            : base(agent)
        { }

        public override string Name
        {
            get
            {
                return "DecideGoal";
            }
        }

        /// <summary>
        /// Get a goal from the simulation, and try to get a slot where the goal can be satisfied.
        /// </summary>
        /// <returns>Success if a slot has been distributed to the agent.</returns>
        public override Action.Result Run()
        {
            if (agent.Blackboard.currentState == Blackboard.AgentState.ExitNEEDSIMBehaviors)
            {
                //Actions should be interrupted until the agent state is dealt with.
                return Result.Failure;
            }

            //Get the goal to satisfy the need with the lowest satisfaction. You can replace the goals for your specific game.
            agent.AffordanceTreeNode.Goal = agent.AffordanceTreeNode.SatisfactionLevels.GoalToSatisfyLowestNeed();

            //If previously a slot was allocated to this agent, try to consume/use it.
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

            //Try to allocate a slot to the agent that will satisfy the goal.
            Simulation.Bidding.Result biddingResult = Simulation.Bidding.GoalOrientedBid(agent.AffordanceTreeNode);

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