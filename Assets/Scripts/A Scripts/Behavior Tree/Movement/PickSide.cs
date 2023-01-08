using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    //Requires "target", possibly list of "allies"
    public class PickSide : Node
    {
        protected Transform _transform;

        public PickSide(Transform transform)
        {
            _transform = transform;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            List<GameObject> allies = (List<GameObject>)GetData("allies");
            if (target == null)
            {
                state = NodeState.failure;
                return state;
            }

            Vector3 dir = _transform.position - target.position;
            Vector3 left = Quaternion.Euler(0, 90, 0)*target.forward ;
            float leftangle = Vector3.Angle(left, dir);
            float rightangle = Vector3.Angle(-left, dir);
            float backangle = Vector3.Angle(-target.forward, dir);
            string side = "left";
            if (rightangle < leftangle)
            {
                side = "right";
                leftangle = rightangle;
            }
            if (backangle < leftangle)
            {
                side = "back";
            }


            if (allies == null)
            {
                SetData("side", side);
                state = NodeState.success;
                return state;
            }


            state = NodeState.failure;
            return state;
        }

    }
}
