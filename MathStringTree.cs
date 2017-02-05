using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms; //TEMP; for MessageBox

namespace MathStringAnalyzer
{
    class MathStringTree
    {
        /* Author: Scott Wolfskill
         * Created:      11/04/2016
         * Last edited:  11/04/2016 */
        /* A class for converting a string representing a mathematical expression (e.g. "4x + 27 * 3") into a binary tree where
         * each internal node represents an operator, and each leaf node is a constant and/or function of a variable. */
        public TreeNode<string> root;

        public MathStringTree()
        {
            root = new TreeNode<string>(2);
        }

        public MathStringTree(TreeNode<string> root)
        {
            this.root = root;
        }

        /// <summary>
        /// Algorithm which returns the complete MathStringTree representation of (toConvert). Calls separate() multiple times.
        /// </summary>
        public static MathStringTree stringToTree(string toConvert)
        {
            TreeNode<string> root = new TreeNode<string>(toConvert);
            return new MathStringTree(_stringToTree(root));
        }

        private static TreeNode<string> _stringToTree(TreeNode<string> root)
        {
            //TODO: Incorporate negative sign '-', ln, trig fxns! Maybe implied multiplication?
            //          Idea: b/c of parens in ln() and <trig>(), _separate already leaves them alone.
            //                All need to do is call separateLeaves() on the ln, trig nodes!
            root.data = trimParens(root.data, root.data.Length);
            root = separateLeaves(root, new char[] {'+', '-'}, new char[] {'*', '/'}, new char[] { '^' }); //separate leaves by multiplication and division; then by exponents
            return root;
        }

        //Helper for stringToTree; calls separate() using (toSeparateBy) on all leaf nodes of (curNode).
        // Then, calls separate() using (toSeparateByNext) on all leaf nodes again (there are possibly new ones)
        //      -Do this instead of calling separateLeaves on the root a second time because this is much more efficient.
        private static TreeNode<string> separateLeaves(TreeNode<string> curNode, char[] toSepBy1, char[] toSepBy2, char[] toSepBy3, bool didSep1 = false, bool didSep2 = false)
        {
            if (curNode.isLeaf()) //base-case
            {
                if (!didSep1)
                {
                    //MessageBox.Show("curNode BEFORE 1st sep:" + Environment.NewLine + treeString(0, curNode));
                    curNode = _separate(curNode.data, toSepBy1); //do 1st separation
                    if (!curNode.isLeaf())
                    {
                        //MessageBox.Show("curNode AFTER 1st sep: not leaf, so check children from beginning:" + Environment.NewLine + treeString(0, curNode));
                        curNode = separateLeaves(curNode, toSepBy1, toSepBy2, toSepBy3); //might have uncovered more of the current operation; go to leaves again
                    } else //want else b/c separateLeaves above will do the 2nd sep; redundant to call below
                    //MessageBox.Show("curNode AFTER sep*/:" + Environment.NewLine + curNode.data);
                    curNode = separateLeaves(curNode, toSepBy1, toSepBy2, toSepBy3, true); //now find new leaves (if any) and do 2nd separation
                }
                else
                {
                    if (!didSep2)
                    {
                        //MessageBox.Show("curNode BEFORE 2nd sep:" + Environment.NewLine + treeString(0, curNode));
                        curNode = _separate(curNode.data, toSepBy2); //do 2nd separation.
                        if (!curNode.isLeaf())
                        {
                            //MessageBox.Show("curNode AFTER 2nd sep: not leaf, so check children from beginning:" + Environment.NewLine + treeString(0, curNode));
                            curNode = separateLeaves(curNode, toSepBy1, toSepBy2, toSepBy3);
                        } else 
                        //MessageBox.Show("curNode AFTER sep^:" + Environment.NewLine + curNode.data);
                        curNode = separateLeaves(curNode, toSepBy1, toSepBy2, toSepBy3, true, true); //go to leaves again for 3rd sep.
                        //if (!curNode.isLeaf()) curNode = separateLeaves(curNode, toSepBy1, toSepBy2, toSepBy3);
                    }
                    else //only separation 3 left
                    {
                        //MessageBox.Show("curNode BEFORE 3rd sep:" + Environment.NewLine + treeString(0, curNode));
                        curNode = _separate(curNode.data, toSepBy3); //do 3rd separation
                        if (!curNode.isLeaf())
                        {
                            //MessageBox.Show("curNode AFTER 3rd sep: not leaf, so check children from beginning:" + Environment.NewLine + treeString(0, curNode));
                            curNode = separateLeaves(curNode, toSepBy1, toSepBy2, toSepBy3);
                        }
                    }
                }
                return curNode;
            }
            else
            {
                if(curNode.child[0] != null) curNode.child[0] = separateLeaves(curNode.child[0], toSepBy1, toSepBy2, toSepBy3, didSep1, didSep2);
                if (curNode.child[1] != null) curNode.child[1] = separateLeaves(curNode.child[1], toSepBy1, toSepBy2, toSepBy3, didSep1, didSep2);
                return curNode;
            }
        }

        /// <summary>
        /// Uses the Separate By Operator algorithm to return a MathStringTree representing a separation of (toSeparate)
        /// by operator (toSeparateBy). Operators such as multiplication (*) cannot be implied!
        /// '(' or ')' will be interpreted as Separate By Parens, which are grouped together.
        /// </summary>
        public static MathStringTree separate(string toSeparate, char toSeparateBy)
        {
            return new MathStringTree(_separate(toSeparate, new char[] { toSeparateBy }));
        }

        /// <summary>
        /// Uses the Separate By Operator algorithm to return a MathStringTree representing a separation of (toSeparate)
        /// by operators (toSeparateBy). Operators such as multiplication (*) cannot be implied!
        /// '(' or ')' will be interpreted as Separate By Parens, which are grouped together. Do NOT input {'(', ')'} !
        /// </summary>
        public static MathStringTree separate(string toSeparate, char toSeparateBy1, char toSeparateBy2)
        {
            return new MathStringTree(_separate(toSeparate, new char[] {toSeparateBy1, toSeparateBy2}));
        }

        /// <summary>
        /// Uses the Separate By Operator algorithm to return a MathStringTree representing a separation of (toSeparate)
        /// by operators (toSeparateBy). Operators such as multiplication (*) cannot be implied!
        /// '(' or ')' will be interpreted as Separate By Parens, which are grouped together. Do NOT input {'(', ')'} !
        /// </summary>
        private static TreeNode<string> _separate(string toSeparate, char[] toSeparateBy)
        {
            //toSeparate = trimParens(toSeparate, toSeparate.Length); //since creating a node does this too, just iterate enough times until the parens are necessary
            //TODO: Account for parens! Account for negative sign (-)!
            if (toSeparate == null) throw new ArgumentNullException("toSeparate");
            char[] arr = toSeparate.ToCharArray();
            int numOperators = 0;
            int openParenCount = 0;
            int closedParenCount = 0;
            //1. Preliminary count of how many operators there are to make things easier
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == '(')
                {
                    openParenCount++;
                }
                else if (arr[i] == ')')
                {
                    closedParenCount++;
                }
                if (openParenCount == closedParenCount) //don't want to separate things that are in parens!!!
                {
                    for (int j = 0; j < toSeparateBy.Length; j++)
                    {
                        if (arr[i] == toSeparateBy[j])
                        {
                            numOperators++;
                            break;
                        }
                    }
                }
            }
            if (numOperators == 0) return new TreeNode<string>(toSeparate);
            //2. Make a node for each individual operand
            int nodeStartIndex = 0;
            char[] operators = new char[numOperators]; //array of operators
            TreeNode<string>[] nodes = new TreeNode<string>[numOperators + 1];
            int nodesIndex = 0;
            openParenCount = 0;
            closedParenCount = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == '(')
                {
                    openParenCount++;
                }
                else if (arr[i] == ')')
                {
                    closedParenCount++;
                }
                if (openParenCount == closedParenCount)
                {
                    if (i == arr.Length - 1)
                    {
                        nodes[nodesIndex] = new TreeNode<string>(toSeparate.Substring(nodeStartIndex, i - nodeStartIndex + 1), 2);
                        nodes[nodesIndex].data = trimParens(nodes[nodesIndex].data, i - nodeStartIndex + 1);
                        nodesIndex++;
                        break;
                    }
                    for (int j = 0; j < toSeparateBy.Length; j++)
                    {
                        if (arr[i] == toSeparateBy[j])
                        {
                            nodes[nodesIndex] = new TreeNode<string>(toSeparate.Substring(nodeStartIndex, i - nodeStartIndex), 2);
                            nodes[nodesIndex].data = trimParens(nodes[nodesIndex].data, i - nodeStartIndex);
                            operators[nodesIndex] = arr[i];
                            nodesIndex++;
                            nodeStartIndex = i + 1;
                            break;
                        }
                    }
                }
            }
            //3. Group them together until have root
            while (nodesIndex > 1)
            {
                nodes = groupNodes(nodes, nodesIndex, operators);
                nodesIndex /= 2;
            }
            return nodes[0];
        }

        /// <summary>
        /// Uses the pair grouping algorithm (see comments) to return the root of the tree created with (nodes).
        /// Should be ceil(numNodes/2) #elements in (operators), which correspond to which data will be set for the node created for each pair.
        /// </summary>
        public static TreeNode<string>[] groupNodes(TreeNode<string>[] nodes, int numNodes, char[] operators)
        {
            /* Pair-Grouping Algorithm: For every pair of element in the array, create a node and set its 2 children to the pair.
             *      If there are an odd # elements (numNodes), then create a node for the last element (odd element out)
             *      and set one child to the last node we created and the other child to the odd element out.
             *          Thus at every pair grouping we will turn (numNodes) into floor(numNodes/2) pairs.  */
            if (numNodes < 2) throw new ArgumentOutOfRangeException("numNodes must be > 1.");
            TreeNode<string>[] pairs = new TreeNode<string>[numNodes / 2]; //integer division does floor() for us
            for (int i = 1; i <= numNodes; i+=2)
            {
                if (i == numNodes) //odd # elements
                {
                    TreeNode<string> tmp = new TreeNode<string>(operators[i - 2].ToString(), pairs[(i / 2) - 1], nodes[i - 1]);
                    pairs[(i / 2) - 1] = tmp;
                }
                else
                {
                    pairs[i / 2] = new TreeNode<string>(operators[i - 1].ToString(), nodes[i - 1], nodes[i]);
                    if(i < numNodes - 1) operators[i/2] = operators[i]; //set up for next call to groupNodes (next grouping)
                }
            }
            return pairs;
        }

        /// <summary>
        /// Evaluates a char array that may or may not contain parentheses and returns true if it has parens, but they're unnecessary.
        /// So if true, (toEvaluate) is of the form "(...)"; the beginning and end are a pair of parens.
        /// If false, either has no parens, or it does have parens but they are necessary.
        /// </summary>
        public static bool parensUnnecessary(char[] toEvaluate, int len) //sadly O(n) :(
        {
            /* Parens only unnecessary if parenCount is only set to 0 at the last closed paren,
             * AND there is only space before the 1st open paren and after the last closed paren. */
            int lastClosedParenIndex = len;
            int parenCount = 0;
            for (int i = len - 1; i >= 0; i--)
            {
                if (toEvaluate[i] == ')') { lastClosedParenIndex = i; break; }
                else if (toEvaluate[i] != ' ') return false; //found meaningful char (non-space) after last closed paren (if any)
            }
            if(lastClosedParenIndex == len) return false; //didn't find a closed paren
            for (int i = 0; i <= lastClosedParenIndex; i++)
            {
                if (toEvaluate[i] == '(') parenCount++;
                else if (toEvaluate[i] == ')') parenCount--;
                if (parenCount == 0) {
                    if (i == lastClosedParenIndex) return true;
                    if (toEvaluate[i] != ' ') return false; //found meaningful char before 1st open paren (if any)
                }
            }
            return false;
        }

        /// <summary>
        /// Will trim all outermost pairs of unnecessary parentheses. If (toTrim) doesn't have any, return original (toTrim).
        /// </summary>
        /// <param name="toTrim"></param>
        /// <returns></returns>
        private static string trimParens(string toTrim, int len)
        {
            char[] arr = toTrim.ToCharArray();
            while(parensUnnecessary(arr, len))
            {
                int startIndex = 0;
                int endIndex = len - 1;
                for (int i = 0; i < len; i++)
                {
                    if (arr[i] == '(') { startIndex = i + 1; arr[i] = ' '; break; }
                }
                for (int i = len - 1; i >= 0; i--)
                {
                    if (arr[i] == ')') { endIndex = i - 1; arr[i] = ' '; break; }
                }
                return toTrim.Substring(startIndex, endIndex - startIndex + 1);
            }
            return toTrim;
        }

        /// <summary>
        /// Get a string representing the MathStringTree.
        /// </summary>
        public string treeString()
        {
            return treeString(0, root);
        }

        private static string treeString(int level, TreeNode<string> node) //private recursive helper fxn
        {
            if(node == null) return ""; //base case: at leaf node
            string info;
            if (level == 0) info = "ROOT: " + node.data;
            else
            {
                info = Environment.NewLine;
                for (int i = 0; i < level; i++)
                {
                    info += "        "; //tab for every level
                }
                info += "Level " + level.ToString() + ": " + node.data;
            }
            return info + treeString(level+1, node.child[0]) + treeString(level+1, node.child[1]);
        }

        public override string ToString()
        {
            return treeString();
        }

    }
}
