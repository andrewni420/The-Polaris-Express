using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mina Pecheux
//https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e

namespace BehaviorTree
{
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool anyChildRunning = false;
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.failure:
                        state = NodeState.failure;
                        return state;
                    case NodeState.success:
                        continue;
                    case NodeState.running:
                        anyChildRunning = true;
                        continue;
                    default:
                        state = NodeState.success;
                        return state;
                }
            }

            state = anyChildRunning ? NodeState.running : NodeState.success;
            return state;
        }
    }
}
