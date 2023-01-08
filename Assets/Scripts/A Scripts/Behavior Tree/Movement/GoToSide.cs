using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    //Requires data "target" and "side"
    public class GoToSide : GoToTarget
    {
        public GoToSide(Transform transform, UnityEngine.AI.NavMeshAgent agent) : base(transform,agent) { }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            string side = (string)GetData("side");

            if (side==null || target == null)
            {
                state = NodeState.failure;
                return state;
            }
            
            Vector3 destination = getDestination(side, target, 2);

            if (_agent.destination != destination) _agent.SetDestination(target.position);

            state = NodeState.running;
            return state;
        }

        private Vector3 getDestination(string side, Transform target, float offset)
        {
            Vector3 pos = target.position;
            Vector3 forward = target.forward;

            switch (side)
            {
                case "left":
                    //player's left
                    return pos + Quaternion.Euler(0, 90, 0) * forward * offset;
                case "right":
                    return pos + Quaternion.Euler(0, -90, 0) * forward * offset;
                case "back":
                    return pos - forward * offset;
                case "front":
                    return pos + forward * offset;
                default:
                    return pos;
            }



        }


    }
}
