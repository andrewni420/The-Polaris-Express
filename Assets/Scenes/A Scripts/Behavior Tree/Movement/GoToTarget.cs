using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mina Pecheux
//https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e

namespace BehaviorTree
{
    //Goes to the transform with key "target" in shared data
    //Requires a "target" transform
    public class GoToTarget : Node
    {
        protected Transform _transform;
        protected UnityEngine.AI.NavMeshAgent _agent;

        public GoToTarget(Transform transform, UnityEngine.AI.NavMeshAgent agent)
        {
            _transform = transform;
            _agent = agent;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");

            if (Vector3.Distance(_transform.position,target.position) > 0.01f)
            {
                _agent.SetDestination(target.position);
            }
            state = NodeState.running;
            return state;
        }
    }
}
