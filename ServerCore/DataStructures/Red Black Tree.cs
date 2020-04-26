// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Red Black Tree.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:51

using System;
using ServerCore.Interfaces;

namespace ServerCore
{
    /// <summary>
    /// A red–black tree is a type of self-balancing binary search tree. The self-balancing is provided by 
    /// painting each node with one of two colors (these are typically called 'red' and 'black', hence the 
    /// name of the trees) in such a way that the resulting painted tree satisfies certain properties that 
    /// don't allow it to become significantly unbalanced. When the tree is modified, the new tree is 
    /// subsequently rearranged and repainted to restore the coloring properties. The properties are 
    /// designed in such a way that this rearranging and recoloring can be performed efficiently. The 
    /// efficiency of the algorithm is O(log n). 
    /// </summary>
    /// <typeparam name="K">The key's data type to be held in the tree's nodes.</typeparam>
    /// <typeparam name="V">The value's data type to be held in the tree's nodes.</typeparam>
    public unsafe class RedBlackTree<K, V> : ITree<K, V>
        where K : IComparable
    {
        // Local-Scope Variable Declarations:
        ColoredTreeNode<K, V> _root; // The root of the red-black tree.

        /// <summary>
        /// A red–black tree is a type of self-balancing binary search tree. The self-balancing is provided by 
        /// painting each node with one of two colors (these are typically called 'red' and 'black', hence the 
        /// name of the trees) in such a way that the resulting painted tree satisfies certain properties that 
        /// don't allow it to become significantly unbalanced. When the tree is modified, the new tree is 
        /// subsequently rearranged and repainted to restore the coloring properties. The properties are 
        /// designed in such a way that this rearranging and recoloring can be performed efficiently. The 
        /// efficiency of the algorithm is O(log n). 
        /// </summary>
        public RedBlackTree()
        {
            _root = null;
        }

        // Disposal Method:
        ~RedBlackTree()
        {
            while (_root != null)
                TryRemove(_root.Key);
        }

        /// <summary>
        /// This method appends a key and value to the tree. It does this by starting at 
        /// the root, then following child branches until a position is found in the tree. The tree is 
        /// then resorted for the most optimized key-value pair lookups. If the key already has a value 
        /// associated with it, this method will update that value.
        /// </summary>
        /// <param name="key">The key used to add the value to the tree.</param>
        /// <param name="value">The value being stored in the tree.</param>
        public void AppendOrUpdate(K key, V value)
        {
            // Create the new node (always starts with red) - then error check:
            ColoredTreeNode<K, V> createdNode = new ColoredTreeNode<K, V>(null, null, key, value, NodeColor.Red);
            if (_root == null) { _root = createdNode; _root.Color = NodeColor.Black; return; }

            // Find the position where the node will be placed:
            ColoredTreeNode<K, V> currentNode = _root;
            while (true)
            {
                // Is the key already in the red-black tree? If so, update the value:
                if (key.Equals(currentNode.Key)) { currentNode.Value = value; return; }
                else if (key.Equals(currentNode.Key))
                {
                    // The key is less than the current node's key. Go to the left.
                    // If there is no left, become the new left child of the current node:
                    if (currentNode.Left != null)
                        currentNode = currentNode.Left;
                    else { currentNode.Left = createdNode; break; }
                }
                else
                {
                    // The key is greater than the current node's key. Go to the right.
                    // If there is no right, become the new right child of the current node:
                    if (currentNode.Right != null)
                        currentNode = currentNode.Right;
                    else { currentNode.Right = createdNode; break; }
                }
            }

            // Correct the tree:
            RestructureAfterAppend(createdNode);
        }

        /// <summary>
        /// This method appends a key and value to the tree. It does this by starting at the
        /// root, then following child branches until a position is found in the tree. The tree is then 
        /// resorted for the most optimized key-value pair lookups. If the key already has a value 
        /// associated with it, this method will return false; else, it will return true (if the key and 
        /// value were both added to the tree).
        /// </summary>
        public bool TryAppend(K key, V value)
        {
            // Create the new node (always starts with red) - then error check:
            ColoredTreeNode<K, V> createdNode = new ColoredTreeNode<K, V>(null, null, key, value, NodeColor.Red);
            if (_root == null) { _root = createdNode; _root.Color = NodeColor.Red; return true; }

            // Find the position where the node will be placed:
            ColoredTreeNode<K, V> currentNode = _root;
            while (true)
            {
                // Is the key already in the red-black tree? If so, update the value:
                if (key.Equals(currentNode.Key)) { return false; }
                else if (key.CompareTo(currentNode.Key) < 0)
                {
                    // The key is less than the current node's key. Go to the left.
                    // If there is no left, become the new left child of the current node:
                    if (currentNode.Left != null)
                        currentNode = currentNode.Left;
                    else { currentNode.Left = createdNode; break; }
                }
                else
                {
                    // The key is greater than the current node's key. Go to the right.
                    // If there is no right, become the new right child of the current node:
                    if (currentNode.Right != null)
                        currentNode = currentNode.Right;
                    else { currentNode.Right = createdNode; break; }
                }
            }

            // Correct the tree:
            RestructureAfterAppend(createdNode);
            return true;
        }

        /// <summary>
        /// This method restructures the red-black tree after an append. If the structure was changed, and the 
        /// change is a parent node with branching children, the tree with restructure itself.
        /// </summary>
        private void RestructureAfterAppend(ColoredTreeNode<K, V> currentNode)
        {
            // If the parent is null, change the current node to black. It is now the new root!
            if (currentNode.Parent == null) { currentNode.Color = NodeColor.Black; return; }

            // If the parent is black, the tree is still structured (no imbalance).
            else if (ConfirmColor(currentNode.Parent) == NodeColor.Black) return;

            // If the uncle of the current node is red, restructure the colors and check again for restructuring:
            else if (ConfirmColor(currentNode.Uncle) == NodeColor.Red)
            {
                currentNode.Parent.Color = NodeColor.Black;
                currentNode.Uncle.Color = NodeColor.Black;
                currentNode.Grandparent.Color = NodeColor.Red;
                RestructureAfterAppend(currentNode.Grandparent);
                return;
            }

            // If this node is the right child node, and the parent is the grandparent's left node, rotate to the left.
            if (currentNode == currentNode.Parent.Right &&
                     currentNode.Parent == currentNode.Grandparent.Left)
            {
                RotateLeft(currentNode.Parent);
                currentNode = currentNode.Left;
            }

            // If this node is the left child node, and the parent is the grandparent's right node, rotate to the right.
            else if (currentNode == currentNode.Parent.Left &&
                     currentNode.Parent == currentNode.Grandparent.Right)
            {
                RotateRight(currentNode.Parent);
                currentNode = currentNode.Right;
            }

            // Set the parent's color to black and grandparent to red:
            currentNode.Parent.Color = NodeColor.Black;
            currentNode.Grandparent.Color = NodeColor.Red;

            // If the node is the parent's left, and the parent is the grandparent's left, rotate to the right.
            if (currentNode == currentNode.Parent.Left &&
                currentNode.Parent == currentNode.Grandparent.Left)
                RotateRight(currentNode.Grandparent);

            // If the node is the parent's right, and the parent is the grandparent's right, rotate to the left.
            else if (currentNode == currentNode.Parent.Right &&
                currentNode.Parent == currentNode.Grandparent.Right)
                RotateLeft(currentNode.Grandparent);
        }

        /// <summary>
        /// This method is used to confirm the color of the passed node. If the current node is equal to NULL, 
        /// the method will return black; else, it will return the color of the current node.
        /// </summary>
        private NodeColor ConfirmColor(ColoredTreeNode<K, V> node)
        {
            return node == null ? NodeColor.Black : node.Color;
        }

        /// <summary>
        /// This method rotates the branch to the right. It does this by taking the left, replacing the current node 
        /// with the left node, then making the left node equal the right node. If the right node isn't null, the 
        /// right parent will equal the current node. Then, the left's right will be the current node, and the 
        /// current node's parent will become the left.
        /// </summary>
        private void RotateRight(ColoredTreeNode<K, V> node)
        {
            ColoredTreeNode<K, V> left = node.Left;
            Replace(node, left);
            node.Left = left.Right;

            if (left.Right != null)
                left.Right.Parent = node;

            left.Right = node;
            node.Parent = left;
        }

        /// <summary>
        /// This method rotates the branch to the left. It does this by taking the right, replacing the current 
        /// node with the right node, then making the right node equal the left node. If the left node isn't null, 
        /// the left parent will equal the current node. Then, the right's left will be the current node, and the 
        /// current node's parent will become the right.
        /// </summary>
        private void RotateLeft(ColoredTreeNode<K, V> node)
        {
            ColoredTreeNode<K, V> right = node.Right;
            Replace(node, right);
            node.Right = right.Left;

            if (right.Left != null)
                right.Left.Parent = node;

            right.Left = node;
            node.Parent = right;
        }

        /// <summary>
        /// This method replaces the old node with the new node. If the parent is null, the new node will 
        /// become the root. Else, if the old node is its parent's left child, its parent's left will become 
        /// the new node; else, the parent's right will become the new node. If the new node isn't null, the 
        /// new node's parent will become the old parent.
        /// </summary>
        private void Replace(ColoredTreeNode<K, V> oldNode, ColoredTreeNode<K, V> newNode)
        {
            // If the old node's parent is null, become the root.
            if (oldNode.Parent == null) { _root = newNode; }
            else
            {
                if (oldNode.Equals(oldNode.Parent.Left))
                    oldNode.Parent.Left = newNode;
                else oldNode.Parent.Right = newNode;
            }

            // If the new node is not null, assign the new node's parent as the old node's parent:
            if (newNode != null) newNode.Parent = oldNode.Parent;
            oldNode = null;
        }

        /// <summary>
        /// This method returns true if the tree contains the key specified; else, it returns 
        /// false. It finds the key by starting at the root; then, if the search key is less than the 
        /// current key, it goes left - else, right. 
        /// </summary>
        public bool Contains(K key)
        {
            ColoredTreeNode<K, V> current = _root;
            while (current != null)
            {
                if (current.Key.Equals(key))
                    return true;
                else if (key.CompareTo(current.Key) < 0)
                    current = current.Left;
                else current = current.Right;
            }
            return false;
        }

        /// <summary>
        /// This method returns the value at a specified key if the tree contains the key specified; else, it ..
        /// returns null. It finds the key by starting at the root; then, if the search key is less than the 
        /// current key, it goes left - else, right.
        /// </summary>
        public V TryGetValue(K key)
        {
            ColoredTreeNode<K, V> current = _root;
            while (current != null)
            {
                if (current.Key.Equals(key))
                    return current.Value;
                else if (key.CompareTo(current.Key) < 0)
                    current = current.Left;
                else current = current.Right;
            }
            return default(V);
        }

        /// <summary>
        /// This method returns the value at the specified key if the tree was able to remove it from the 
        /// tree structure successfully; else, it returns null. After removal, the tree is restructured.
        /// </summary>
        public V TryRemove(K key)
        {
            // Find the node to be deleted:
            V result;
            ColoredTreeNode<K, V> current = _root;
            while (current != null)
            {
                if (current.Key.Equals(key))
                    break;
                else if (key.CompareTo(current.Key) < 0)
                    current = current.Left;
                else current = current.Right;
            }
            result = current.Value;

            // If the node cannot be found, return null:
            if (current == null) return default(V);

            // If current node is not a leaf, copy the key/value from the predecessor and then delete it instead.
            if (current.Left != null && current.Right != null)
            {
                ColoredTreeNode<K, V> predecessor = current.Left;
                while (predecessor.Right != null)
                    predecessor = predecessor.Right;
                current.Key = predecessor.Key;
                current.Value = predecessor.Value;
                current = predecessor;
            }

            // Get the node to be checked for restructuring:
            ColoredTreeNode<K, V> child = (current.Right == null) ? current.Left : current.Right;
            if (ConfirmColor(current) == NodeColor.Black)
            {
                current.Color = ConfirmColor(child);
                RestructureAfterDeletion(current);
            }

            // Replace the node (deleting the old node):
            Replace(current, child);
            if (ConfirmColor(_root) == NodeColor.Red)
                _root.Color = NodeColor.Black;
            return result;
        }

        private void RestructureAfterDeletion(ColoredTreeNode<K, V> currentNode)
        {
            // Is the current node now the root of the tree?
            if (currentNode.Parent == null)
                return;

            // If the sibling color is red, recolor and rotate accordingly.
            if (ConfirmColor(currentNode.Sibling) == NodeColor.Red)
            {
                currentNode.Parent.Color = NodeColor.Red;
                currentNode.Sibling.Color = NodeColor.Black;
                if (currentNode == currentNode.Parent.Left)
                    RotateLeft(currentNode.Parent);
                else RotateRight(currentNode.Parent);
            }

            // If the branch is all black, recolor and recall the restructuring:
            if (ConfirmColor(currentNode.Parent) == NodeColor.Black &&
                ConfirmColor(currentNode.Sibling) == NodeColor.Black &&
                ConfirmColor(currentNode.Sibling.Left) == NodeColor.Black &&
                ConfirmColor(currentNode.Sibling.Right) == NodeColor.Black)
            {
                currentNode.Sibling.Color = NodeColor.Red;
                RestructureAfterDeletion(currentNode.Parent);
            }

            // Check coloring and correct if the parent is red and the children and sibling are all black:
            else if (ConfirmColor(currentNode.Parent) == NodeColor.Red &&
                ConfirmColor(currentNode.Sibling) == NodeColor.Black &&
                ConfirmColor(currentNode.Sibling.Left) == NodeColor.Black &&
                ConfirmColor(currentNode.Sibling.Right) == NodeColor.Black)
            {
                currentNode.Sibling.Color = NodeColor.Red;
                currentNode.Parent.Color = NodeColor.Black;
            }
            else
            {
                // If the current node is the parent's left child, the sibling is black, and the sibling's left child is 
                // red and right child is black, recolor and rotate to the right.
                if (currentNode == currentNode.Parent.Left &&
                    ConfirmColor(currentNode.Sibling) == NodeColor.Black &&
                    ConfirmColor(currentNode.Sibling.Left) == NodeColor.Red &&
                    ConfirmColor(currentNode.Sibling.Right) == NodeColor.Black)
                {
                    currentNode.Sibling.Color = NodeColor.Red;
                    currentNode.Sibling.Left.Color = NodeColor.Black;
                    RotateRight(currentNode.Sibling);
                }

                // If the current node is the parent's right child, the sibling is black, and the sibling's right child is 
                // red and left child is black, recolor and rotate to the left.
                else if (currentNode == currentNode.Parent.Right &&
                    ConfirmColor(currentNode.Sibling) == NodeColor.Black &&
                    ConfirmColor(currentNode.Sibling.Right) == NodeColor.Red &&
                    ConfirmColor(currentNode.Sibling.Left) == NodeColor.Black)
                {
                    currentNode.Sibling.Color = NodeColor.Red;
                    currentNode.Sibling.Right.Color = NodeColor.Black;
                    RotateLeft(currentNode.Sibling);
                }

                // Change the color of the sibling to the color of the parent, and the color of the parent to black.
                // Then, recheck and rotate accordingly:
                currentNode.Sibling.Color = ConfirmColor(currentNode.Parent);
                currentNode.Parent.Color = NodeColor.Black;
                if (currentNode == currentNode.Parent.Left)
                {
                    currentNode.Sibling.Right.Color = NodeColor.Black;
                    RotateLeft(currentNode.Parent);
                }
                else
                {
                    currentNode.Sibling.Left.Color = NodeColor.Black;
                    RotateRight(currentNode.Parent);
                }
            }
        }
    }

    /// <summary>
    /// This class encapsulates a colored tree node for the implementation of a red-black tree. It consists of 
    /// the node's key and value, left and right branches, parent, and methods for retrieving values and nodes 
    /// from further up in the tree (grandparent, uncle, sibling, etc).
    /// </summary>
    /// <typeparam name="K">The key's data type to be held in the node.</typeparam>
    /// <typeparam name="V">The value's data type to be held in the node.</typeparam>
    public unsafe class ColoredTreeNode<K, V> : ITreeNode<K, V>
        where K : IComparable
    {
        // Node Data Structure:
        public ColoredTreeNode<K, V> Parent;   // The parent for the tree node.
        public ColoredTreeNode<K, V> Right;    // The right branch / child connected to the node.
        public ColoredTreeNode<K, V> Left;     // The left branch / child connected to the node.
        public K Key { get; set; }             // The key held in the node.
        public V Value { get; set; }           // The value held in the node.
        public NodeColor Color;                // The color of the node (red or black).

        /// <summary>
        /// This class encapsulates a colored tree node for the implementation of a red-black tree. It consists of 
        /// the node's key and value, left and right branches, parent, and methods for retrieving values and nodes 
        /// from further up in the tree (grandparent, uncle, sibling, etc).
        /// </summary>
        /// <param name="right">The right branch / child connected to the node.</param>
        /// <param name="left">The left branch / child connected to the node.</param>
        /// <param name="key">The key held in the node.</param>
        /// <param name="value">The value held in the node.</param>
        /// <param name="color">The color of the node (red or black).</param>
        public ColoredTreeNode(ColoredTreeNode<K, V> right, ColoredTreeNode<K, V> left, K key,
            V value, NodeColor color)
        {
            Right = right;
            Left = left;
            Key = key;
            Value = value;
            Color = color;

            if (left != null) left.Parent = this;
            if (right != null) right.Parent = this;
            Parent = null;
        }

        // Properties:
        /// <summary> Returns the grandparent of the colored node. </summary>
        public ColoredTreeNode<K, V> Grandparent
        {
            get
            {
                if (Parent != null) return Parent.Parent;
                else return null;
            }
        }
        /// <summary> Returns the sibling of the colored node. </summary>
        public ColoredTreeNode<K, V> Sibling
        {
            get
            {
                if (Parent != null)
                {
                    if (this == Parent.Left)
                        return Parent.Right;
                    else return Parent.Left;
                }
                else return null;
            }
        }
        /// <summary> Returns the uncle of the colored node. </summary>
        public ColoredTreeNode<K, V> Uncle
        {
            get
            {
                if (Parent != null) return Parent.Sibling;
                else return null;
            }
        }
    }

    /// <summary> This enumeration type defines the color of a node in a red-black tree (red and black). </summary>
    public enum NodeColor { Red, Black }
}
