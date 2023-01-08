using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mina Pecheux
//https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e
namespace BehaviorTree
{
    public abstract class Tree : MonoBehaviour
    {
        private Node _root = null;

        protected void Start()
        {
            _root = SetupTree();
        }

        private void Update()
        {
            if (_root != null) _root.Evaluate();
        }

        protected abstract Node SetupTree();
    }
}
