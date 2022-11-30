using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class Attack : Node
    {
        private Transform _lastTarget;
        private float _attackTime = 1;
        private float _attackCounter = 0;

        public Attack(Transform transform)
        {
            //_animator = transform.GetComponent<Animator>();
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target != _lastTarget)
            {
                _lastTarget = target;
            }

            _attackCounter += Time.deltaTime;
            if (_attackCounter >= _attackTime)
            {
                //attack
                //if (enemyIsDead)
                //{
                //    ClearData("target");
                //    _animator.SetBool("Attacking", false);
                //    _animator.SetBool("Walking", true);
                //}
                //else{
                _attackCounter = 0;
                //}
            }

            state = NodeState.running;
            return state;
        }
    }
}
