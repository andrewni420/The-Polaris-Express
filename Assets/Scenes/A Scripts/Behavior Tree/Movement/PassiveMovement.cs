using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class PassiveMovement : Node
    {
        private UnityEngine.AI.NavMeshAgent _agent;
        private Vector3 destination = new Vector3();
        public float maxRotSpeed = 1.0F;
        private Transform _transform;
        protected Queue<Vector3> predTraj = new Queue<Vector3>();
        protected Queue<float> predAVel = new Queue<float>();

        public PassiveMovement(UnityEngine.AI.NavMeshAgent agent, Transform transform)
        {
            _agent = agent;
            _transform = transform;

            //_animator = transform.GetComponent<Animator>();
        }

        public override NodeState Evaluate()
        {

            (Vector3 dir, Vector3 rot) move = passiveMove();
            if (move.dir != destination)
            {
                _agent.SetDestination(move.dir);
                destination = _agent.destination;
            }

            _transform.Rotate(move.rot);

            state = NodeState.running;
            return state;
        }

        private (Vector3 dir, Vector3 rot) passiveMove()
        {
            if (predTraj.Count < 5) predictMovement();
            Vector3 nextTraj = popPredTraj();
            float nextAngle = popPredAVel();

            return (nextTraj, new Vector3(0, nextAngle, 0));
        }

        public Queue<Vector3> getPredTraj() { return predTraj; }
        public Vector3 popPredTraj() { return predTraj.Dequeue(); }
        public Queue<float> getPredAVel() { return predAVel; }
        public float popPredAVel() { return predAVel.Dequeue(); }

        public void addForwardTraj()
        {
            int moveLength = UnityEngine.Random.Range(60, 100);
            int moveDist = UnityEngine.Random.Range(10, 30);
            Vector3 forward = _transform.position + _transform.forward * moveDist;
            for (int i = 0; i < moveLength; i++)
            {
                predTraj.Enqueue(forward);
                predAVel.Enqueue(0);
            }
        }
        public void addRotationTraj()
        {
            int rotLength = UnityEngine.Random.Range(20, 60);
            float leftRight = 2 * UnityEngine.Random.Range(0, 2) - 1;
            float rotSpeed = UnityEngine.Random.Range(0.8F, 1F) * maxRotSpeed * leftRight;
            for (int i = 0; i < rotLength; i++)
            {
                predAVel.Enqueue(rotSpeed);
                predTraj.Enqueue(_transform.position);
            }
        }
        public void addWaitTraj()
        {
            int waitLength = UnityEngine.Random.Range(20, 40);
            for (int i = 0; i < waitLength; i++)
            {
                predTraj.Enqueue(_transform.position);
                predAVel.Enqueue(0);
            }
        }
        public void predictMovement()
        {
            //Movement types: move forward / rotate / wait
            float moveType = UnityEngine.Random.Range(0F, 1F);

            if (moveType < 0.33) addForwardTraj();
            else if (moveType < 0.66) addRotationTraj();
            else addWaitTraj();
        }
    }
}
