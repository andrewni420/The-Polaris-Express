using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mina Pecheux
//https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e

namespace BehaviorTree
{
    public enum NodeState
    {
        running, success, failure
    }
    public class Node
    {
        protected NodeState state;
        public Node parent;
        protected List<Node> children = new List<Node>();
        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        public Node()
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            foreach (Node child in children) { _Attach(child); }
        }

        private void _Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.failure;

        public void SetData(string key, object value) { _dataContext[key] = value; }

        public object GetData(string key)
        {
            object value = null;
            if (_dataContext.TryGetValue(key, out value)) return value;

            if (parent == null) return null;

            return parent.GetData(key);
        }

        public bool RemoveKey(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            if (parent == null) return false;

            return parent.RemoveKey(key);
        }
    }

}
