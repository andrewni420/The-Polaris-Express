using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class Repeater : Node
    {
        private Node child;
        private float duration = 1;
        private float counter = 0;

        public Repeater() : base() { }
        public Repeater(Node node, float _duration) : base(new List<Node> { node}) { duration = _duration; }

        public override NodeState Evaluate()
        {
            children[0].Evaluate();


            counter += Time.deltaTime;
            if (counter >= duration)
            {
                state = NodeState.success;
                return state;
            }
            else
            {
                state = NodeState.running;
                return state;
            }
            
        }
    }
}
