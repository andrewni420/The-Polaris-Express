using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mina Pecheux
//https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e

namespace BehaviorTree
{
    public class Patrol : Node
    {
        private UnityEngine.AI.NavMeshAgent _agent;
        private Transform _transform;
        private Transform[] _waypoints;
        private int _curWaypoint = 0;
        private float _waitTime = 1f;
        private float _waitCounter = 0f;
        private bool _waiting = false;
        //private Animator _animator;

        //patrol extends selector
            //UpdateWaypoint
            //Go to Destination


        public Patrol(UnityEngine.AI.NavMeshAgent agent, Transform transform, Transform[] waypoints)
        {
            _agent = agent;
            _transform = transform;
            _waypoints = waypoints;
            _agent.speed = GuardBT.speed;
            //_animator = transform.GetComponent<Animator>();
        }

        public override NodeState Evaluate()
        {
            if (_waiting)
            {
                _waitCounter += Time.deltaTime;
                if (_waitCounter >= _waitTime)
                {
                    _waiting = false;
                    _agent.SetDestination(_waypoints[_curWaypoint].position);
                    //_animator.SetBool("Walking", true);
                }
            }
            else
            {
                Transform wp = _waypoints[_curWaypoint];
                if (Vector3.Distance(_transform.position, wp.position) < 0.01f)
                {
                    _agent.Warp(wp.position);
                    _waitCounter = 0f;
                    _waiting = true;
                    _curWaypoint = (_curWaypoint + 1) % _waypoints.Length;
                    //_animator.SetBool("Walking",false);
                }
            }

            state = NodeState.running;
            return state;
        }


    }
}
