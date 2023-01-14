using System;

namespace Excalibur.DataStructures
{
    public class BSTNode<T> : IComparable<BSTNode<T>> where T : IComparable<T>
    {
        private T _value;
        private BSTNode<T> _parent;
        private BSTNode<T> _left;
        private BSTNode<T> _right;

        public BSTNode() : this(default(T), 0, null, null, null) { }
        public BSTNode(T value) : this(value, 0, null, null, null) { }
        public BSTNode(T value, int subTreeSize, BSTNode<T> parent, BSTNode<T> left, BSTNode<T> right)
        {
            Value = value;
            Parent = parent;
            LeftChild = left;
            RightChild = right;
        }

        public virtual T Value
        {
            get { return _value; }
            set { this._value = value; }
        }

        public virtual BSTNode<T> Parent
        {
            get { return _parent; }
            set { this._parent = value; }
        }

        public virtual BSTNode<T> LeftChild
        {
            get { return _left; }
            set { this._left = value; }
        }

        public virtual BSTNode<T> RightChild
        {
            get { return _right; }
            set { this._right = value; }
        }

        public virtual bool HasChildren
        {
            get { return (this.ChildrenCount > 0); }
        }

        public virtual bool HasLeftChild
        {
            get { return (this.LeftChild != null); }
        }

        public virtual bool HasRightChild
        {
            get { return (this.RightChild != null); }
        }

        public virtual bool HasOnlyLeftChild => !this.HasRightChild && this.HasLeftChild;

        public virtual bool HasOnlyRightChild => !this.HasLeftChild && this.HasRightChild;

        public virtual bool IsLeftChild
        {
            get { return (this.Parent != null) && this.Parent.LeftChild == this; }
        }

        public virtual bool IsRightChild
        {
            get { return (this.Parent != null && this.Parent.RightChild == this); }
        }

        public virtual bool IsLeafNode
        {
            get { return (this.ChildrenCount == 0); }
        }

        public virtual int ChildrenCount
        {
            get
            {
                int count = 0;

                if (this.HasLeftChild)
                    count++;
                if (this.HasRightChild)
                    count++;

                return count;
            }
        }

        public int CompareTo(BSTNode<T> other)
        {
            if (other == null)
            {
                return -1;
            }

            return this.Value.CompareTo(other.Value);
        }
    }
}
