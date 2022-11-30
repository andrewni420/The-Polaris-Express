using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mina Pecheux
//https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e

namespace BehaviorTree
{
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.failure:
                        continue;
                    case NodeState.success:
                        state = NodeState.success;
                        return state;
                    case NodeState.running:
                        state = NodeState.running;
                        return state;
                    default:
                        continue;
                }
            }

            state = NodeState.failure;
            return state;
        }
    }
}
