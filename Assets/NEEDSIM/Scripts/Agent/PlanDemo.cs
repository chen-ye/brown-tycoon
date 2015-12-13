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
using System.Collections.Generic;

namespace NEEDSIM
{
    /// <summary>
    /// A simple behavior control solution. We tried to write this in a way that makes it easy to use our code samples in 
    /// Finite State Machines, Behavior Trees and Goal-oriented Action Planning. The idea is that you can run our simulation
    /// from within a different solution, for example in case you want to have agents with fighting capabilites.
    /// </summary>
    public class PlanDemo
    {
        Dictionary<string, Action> ActionLibrary;

        Stack<Action> Plan;

        private NEEDSIMNode.ExamplePlans selectedPlan;

        public PlanDemo(NEEDSIMNode agent)
        {
            ActionLibrary = new Dictionary<string, Action>();

            ActionLibrary.Add("DecideGoal", new DecideGoal(agent));
            ActionLibrary.Add("MoveToSlot", new MoveToSlot(agent));
            ActionLibrary.Add("SatisfyGoal", new SatisfyGoal(agent));
            ActionLibrary.Add("DecideValue", new DecideValue(agent));
            ActionLibrary.Add("SatisfyUrgentNeed", new SatisfyUrgentNeed(agent));
            ActionLibrary.Add("ChaseSlot", new ChaseSlot(agent));
            ActionLibrary.Add("InterruptionFuchsalarm", new InterruptionFuchsalarm(agent));
            
            selectedPlan = agent.selectedPlan;

            Plan = getNewPlan();
        }

        /// <summary>
        /// Update runs the plan.
        /// 
        /// If the currently running action returns, upon evaluation, Result.Running, we keep on running that action.
        /// If Result.Failure is returend we start a new sequence.
        /// If Result.Success is returned we go to the next step in the current sequence, or, if at the last step,
        /// start a new sequence.
        /// </summary>
        public void Update()
        {
            Action.Result result = Plan.Peek().Run();

            switch (result)
            {
                case Action.Result.Running:
                    break;
                case Action.Result.Failure:
                    Plan = getNewPlan();
                    break;
                case Action.Result.Success:
                    Plan.Pop();
                    if (Plan.Count == 0)
                        Plan = getNewPlan();
                    break;
            }
        }

        /// <summary>
        /// A simple sequence of actions is produced.
        /// </summary>
        /// <returns>A simple plan to decide on a goal to satisfy a need, go to a place that satisfies that need, and then
        /// interact with that place until satisfied.</returns>
        private Stack<Action> getNewPlan()
        {
            Stack<Action> plan = new Stack<Action>();

            switch (selectedPlan)
            {
                case (NEEDSIMNode.ExamplePlans.GoalOriented):
                    plan.Push(ActionLibrary["SatisfyGoal"]);
                    plan.Push(ActionLibrary["MoveToSlot"]);
                    plan.Push(ActionLibrary["DecideGoal"]);
                    break;
                case (NEEDSIMNode.ExamplePlans.ValueOriented):
                    plan.Push(ActionLibrary["SatisfyUrgentNeed"]);
                    plan.Push(ActionLibrary["MoveToSlot"]);
                    plan.Push(ActionLibrary["DecideValue"]);
                    break;
                case (NEEDSIMNode.ExamplePlans.GoalOrientedChase):
                    plan.Push(ActionLibrary["SatisfyGoal"]);
                    plan.Push(ActionLibrary["ChaseSlot"]);
                    plan.Push(ActionLibrary["DecideGoal"]);
                    break;
                case (NEEDSIMNode.ExamplePlans.InterruptionNachtDesFuchses):
                    plan.Push(ActionLibrary["SatisfyUrgentNeed"]);
                    plan.Push(ActionLibrary["MoveToSlot"]);
                    plan.Push(ActionLibrary["DecideValue"]);
                    plan.Push(ActionLibrary["InterruptionFuchsalarm"]);
                    break;
                default:
                    Debug.LogWarning("Plan option not handled");
                    break;
            }
            return plan;
        }

        public string printCurrentAction()
        {
            return Plan.Peek().Name;
        }

    }
}