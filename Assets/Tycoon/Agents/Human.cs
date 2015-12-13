using UnityEngine;
using System.Collections;

namespace Tycoon
{
    public class Human : MonoBehaviour
    {

        protected Animator animator;
        protected NEEDSIM.NEEDSIMNode needsimNode;

        private NavMeshAgent navMeshAgent;

        public virtual void Start()
        {
            animator = GetComponentInChildren<Animator>();
            needsimNode = GetComponent<NEEDSIM.NEEDSIMNode>();
            navMeshAgent = needsimNode.GetComponent<NavMeshAgent>();

            //navMeshAgent.updatePosition = false;
        }

        public virtual void Update()
        {
            if (needsimNode.AnimationsToPlay.Count > 0)
            {
                if (needsimNode.AnimationsToPlay.Peek() == NEEDSIM.NEEDSIMNode.AnimationOrders.MovementStartedByAgent)
                {
                    //Rotate agent into movement direction
                    gameObject.transform.LookAt(navMeshAgent.steeringTarget);
                    gameObject.transform.Rotate(90.0f, gameObject.transform.rotation.y, gameObject.transform.rotation.z);
                    //animator.rootPosition = navMeshAgent.desiredVelocity;
                    animator.SetFloat("Speed", navMeshAgent.desiredVelocity.magnitude);

                }
                else if (needsimNode.AnimationsToPlay.Peek() == NEEDSIM.NEEDSIMNode.AnimationOrders.InteractionStartedByAgent)
                {
                    //Rotate agent towards LookAt 
                    gameObject.transform.LookAt(needsimNode.Blackboard.activeSlot.LookAt);
                }
                //This method will call the SetTrigger method on the animator, thus triggering correctly named transitions into animation states.
                needsimNode.TryConsumingAnimationOrder(animator);
            }
        }

        //public virtual void OnAnimatorMove()
        //{
        //    Vector3 velocity = animator.deltaPosition / Time.deltaTime;
        //    if (velocity.magnitude > 0.0)
        //    {
        //        navMeshAgent.velocity = animator.deltaPosition / Time.deltaTime;
        //        Quaternion lookRotation = Quaternion.LookRotation(navMeshAgent.desiredVelocity);
        //        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, navMeshAgent.angularSpeed * Time.deltaTime);
        //    }

        //}
    }

}
