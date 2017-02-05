using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathStringAnalyzer
{
    class TreeNode<T>
    {
        /* Author: Scott Wolfskill
         * Created:      11/04/2016
         * Last edited:  11/04/2016 */
        /*A top-down, customizable TreeNode allowing any natural number of child nodes. Each node holds one data variable of type T.
         *   (top-down meaning any given node doesn't have information about its parent; only about its child nodes) */ 
        public TreeNode<T>[] child; //array of child nodes
        public T data;
        private int treeOrder; //number of child nodes. Read-only.

        public TreeNode(int treeOrder = 2) {
            if (treeOrder < 1) throw new ArgumentOutOfRangeException("treeOrder must be > 0.");
            this.treeOrder = treeOrder;
            child = new TreeNode<T>[treeOrder];
            for (int i = 0; i < treeOrder; i++)
                child[i] = null; //init child nodes to null
        }

        public TreeNode(T nodeData, int treeOrder = 2)
        {
            if (treeOrder < 1) throw new ArgumentOutOfRangeException("treeOrder must be > 0.");
            this.data = nodeData;
            this.treeOrder = treeOrder;
            child = new TreeNode<T>[treeOrder];
            for (int i = 0; i < treeOrder; i++)
                child[i] = null; //init child nodes to null
        }

        /// <summary>
        /// Create a binary tree with child[0] set to Lchild and child[1] set to Rchild.
        /// </summary>
        public TreeNode(T nodeData, TreeNode<T> Lchild, TreeNode<T> Rchild)
        {
            this.treeOrder = 2;
            this.data = nodeData;
            child = new TreeNode<T>[treeOrder];
            child[0] = Lchild;
            child[1] = Rchild;
        }

        public int getTreeOrder() { return treeOrder; }

        public bool isLeaf()
        {
            for (int i = 0; i < treeOrder; i++)
                if (child[i] != null) return false;
            return true; //all child nodes are null
        }

    }
}
