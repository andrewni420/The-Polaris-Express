using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mina Pecheux
//https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e

namespace BehaviorTree
{
    public class GuardBT : Tree
    {
        public Transform[] waypoints;
        public GameObject _player;
        public UnityEngine.AI.NavMeshAgent agent;
        public static float speed = 2f;
        public static float FOVrange = 10;
        public static float attackrange = 2;
        public static float angle = 90;

        protected override Node SetupTree()
        {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            Node root = new Selector(new List<Node>
            {
                //new Sequence(new List<Node>
                //{
                //    new CheckAttack(transform),
                //    new Attack(transform),
                //}),
                new Sequence(new List<Node>
                {
                    new CheckFOV(transform, _player),
                    new GoToTarget(transform, agent)
                }),
                new Patrol(agent, transform, waypoints)
            }) ;
            
            return root;
        }

    }
}
