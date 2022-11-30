using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class CheckAttack : Node
    {
        private Transform _transform;
        //private Animator _animator;

        public CheckAttack(Transform transform)
        {
            _transform = transform;
            //_animator = transform.GetComponent<Animator>();
        }

        public override NodeState Evaluate()
        {

            object t = GetData("target");
            if (t == null)
            {
                state = NodeState.failure;
                return state;
            }

            Transform target = (Transform)t;
            if (Vector3.Distance(_transform.position, target.position) <= GuardBT.attackrange)
            {
                //_animator.SetBool("Attacking", true);
                //_animator.SetBool("Walking", false);
                state = NodeState.success;
                return state;
            }

            state = NodeState.failure;
            return state;
        }
    }
}
