using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mina Pecheux
//https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e

namespace BehaviorTree
{
    public class CheckFOV : Node
    {
        private Transform _transform;
        private GameObject _player;
        

        public CheckFOV(Transform transform, GameObject player)
        {
            _transform = transform;
            _player = player;
        }

        public override NodeState Evaluate()
        {
            object t = GetData("target");
            if (t == null)
            {
                if (canSee(_player.transform.position, "Player"))
                {
                    parent.parent.SetData("target", _player.transform);
                    //_animator.SetBool("Walking",true);
                    state = NodeState.success;
                    return state;
                }
                state = NodeState.failure;
                return state;
            }

            state = NodeState.success;
            return state;
        }

        public bool canSee(Vector3 other, string tag)
        {
            Vector3 dir = other - _transform.position;
            float angle = Vector3.Angle(_transform.forward, dir);
            float distance = dir.magnitude;
            if (distance > GuardBT.FOVrange || angle > GuardBT.angle) return false;

            RaycastHit hit;
            if (Physics.Raycast(_transform.position, dir, out hit, GuardBT.FOVrange))
            {
                if (hit.collider.gameObject.tag != tag) return false;
            }

            return true;
        }
    }
}
