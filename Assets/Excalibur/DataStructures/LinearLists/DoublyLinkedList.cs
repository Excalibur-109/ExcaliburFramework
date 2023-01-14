using System;
using System.Linq;
using System.Collections.Generic;

namespace Excalibur.DataStructures
{
    /// <summary>
    /// The Doubly-Linked List Node.
    /// </summary>
    public class DLinkedListNode<T> : IComparable<DLinkedListNode<T>> where T : IComparable<T>
    {
        private T _data;
        private DLinkedListNode<T> _next;
        private DLinkedListNode<T> _previous;

        public DLinkedListNode() : this(default(T)) { }
        public DLinkedListNode(T dataItem) : this(dataItem, null, null) { }

        public DLinkedListNode(T dataItem, DLinkedListNode<T> next, DLinkedListNode<T> previous)
        {
            Data = dataItem;
            Next = next;
            Previous = previous;
        }

        public T Data
        {
            get { return _data; }
            set { this._data = value; }
        }

        public DLinkedListNode<T> Next
        {
            get { return _next; }
            set { this._next = value; }
        }

        public DLinkedListNode<T> Previous
        {
            get { return _previous; }
            set { this._previous = value; }
        }

        public int CompareTo(DLinkedListNode<T> other)
        {
            if (other == null)
            {
                return -1;
            }

            return this.Data.CompareTo(other.Data);
        }
    }

    /// <summary>
    /// The Doubly-Linked List Data Structure.
    /// </summary>
    public class DLinkedList<T> : IEnumerable<T> where T : IComparable<T>
    {
        private int _count;
        private DLinkedListNode<T> _firstNode;
        private DLinkedListNode<T> _lastNode;

        public DLinkedList()
        {
            _firstNode = null;
            _lastNode = null;
            _count = 0;
        }

        public virtual DLinkedListNode<T> Head
        {
            get { return this._firstNode; }
        }

        public virtual int Count
        {
            get { return this._count; }
        }

        public virtual bool IsEmpty()
        {
            return (Count == 0);
        }

        public virtual T First
        {
            get
            {
                if (IsEmpty())
                {
                    throw new Exception("Empty List.");
                }

                return _firstNode.Data;
            }
        }

        public virtual T Last
        {
            get
            {
                if (IsEmpty())
                {
                    throw new Exception("Empty List.");
                }

                if (_lastNode == null)
                {
                    var currentNode = _firstNode;
                    while (currentNode.Next != null)
                    {
                        currentNode = currentNode.Next;
                    }
                    _lastNode = currentNode;
                    return currentNode.Data;
                }

                return _lastNode.Data;
            }
        }

        public virtual T this[int index]
        {
            get { return this._getElementAt(index); }
            set { this._setElementAt(index, value); }
        }

        protected virtual T _getElementAt(int index)
        {
            if (IsEmpty() || index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (index == 0)
            {
                return First;
            }

            if (index == (Count - 1))
            {
                return Last;
            }

            DLinkedListNode<T> currentNode;

            if (index > Count / 2)
            {
                currentNode = this._lastNode;
                for (int i = (Count - 1); i > index; --i)
                {
                    currentNode = currentNode.Previous;
                }
            }
            else
            {
                currentNode = this._firstNode;
                for (int i = 0; i < index; ++i)
                {
                    currentNode = currentNode.Next;
                }
            }

            return currentNode.Data;
        }

        protected virtual void _setElementAt(int index, T value)
        {
            if (IsEmpty() || index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (index == 0)
            {
                _firstNode.Data = value;
            }
            else if (index == (Count - 1))
            {
                _lastNode.Data = value;
            }
            else
            {
                DLinkedListNode<T> currentNode;

                if (index > Count / 2)
                {
                    currentNode = this._lastNode;
                    for (int i = (Count - 1); i > index; --i)
                    {
                        currentNode = currentNode.Previous;
                    }
                }
                else
                {
                    currentNode = this._firstNode;
                    for (int i = 0; i < index; ++i)
                    {
                        currentNode = currentNode.Next;
                    }
                }
                currentNode.Data = value;
            }
        }

        public virtual int IndexOf(T dataItem)
        {
            int i = 0;
            bool found = false;
            var currentNode = _firstNode;

            while (i < Count)
            {
                if (currentNode.Data.IsEqualTo(dataItem))
                {
                    found = true;
                    break;
                }

                currentNode = currentNode.Next;
                ++i;
            }

            return (found == true ? i : -1);
        }

        public virtual void Prepend(T dataItem)
        {
            var newNode = new DLinkedListNode<T>(dataItem);

            if (_firstNode == null)
            {
                _firstNode = _lastNode = newNode;
            }
            else
            {
                var currentNode = _firstNode;
                newNode.Next = currentNode;
                currentNode.Previous = newNode;
                _firstNode = newNode;
            }

            _count++;
        }

        public virtual void Append(T dataItem)
        {
            var newNode = new DLinkedListNode<T>(dataItem);

            if (_firstNode == null)
            {
                _firstNode = _lastNode = newNode;
            }
            else
            {
                var currentNode = _lastNode;
                currentNode.Next = newNode;
                newNode.Previous = currentNode;
                _lastNode = newNode;
            }

            _count++;
        }

        public virtual void InsertAt(T dataItem, int index)
        {
            if (index < 0 || index > Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (index == 0)
            {
                Prepend(dataItem);
            }
            else if (index == Count)
            {
                Append(dataItem);
            }
            else
            {
                DLinkedListNode<T> currentNode;
                var newNode = new DLinkedListNode<T>(dataItem);

                currentNode = this._firstNode;
                for (int i = 0; i < index - 1; ++i)
                {
                    currentNode = currentNode.Next;
                }

                var oldNext = currentNode.Next;

                if (oldNext != null)
                {
                    currentNode.Next.Previous = newNode;
                }

                newNode.Next = oldNext;
                currentNode.Next = newNode;
                newNode.Previous = currentNode;

                _count++;
            }
        }

        public virtual void InsertAfter(T dataItem, int index)
        {
            InsertAt(dataItem, index - 1);
        }

        public virtual void Remove(T dataItem)
        {
            if (IsEmpty())
            {
                throw new IndexOutOfRangeException();
            }

            if (_firstNode.Data.IsEqualTo(dataItem))
            {
                _firstNode = _firstNode.Next;
                
                if (_firstNode != null)
                {
                    _firstNode.Previous = null;
                }
            }
            else if (_lastNode.Data.IsEqualTo(dataItem))
            {
                _lastNode = _lastNode.Previous;

                if (_lastNode != null)
                {
                    _lastNode.Next = null;
                }
            }
            else
            {
                var currentNode = _firstNode;

                while (currentNode.Next != null)
                {
                    if (currentNode.Data.IsEqualTo(dataItem))
                    {
                        break;
                    }

                    currentNode = currentNode.Next;
                }

                if (!currentNode.Data.IsEqualTo(dataItem))
                {
                    throw new Exception("Item was not found!");
                }

                var newPrevious = currentNode.Previous;
                var newNext = currentNode.Next;

                if (newPrevious != null)
                {
                    newPrevious.Next = newNext;
                }

                if (newNext != null)
                {
                    newNext.Previous = newPrevious;
                }

                currentNode = newPrevious;
            }

            _count--;
        }

        public virtual void RemoveFirstMatch(Predicate<T> match)
        {
            if (IsEmpty())
            {
                throw new IndexOutOfRangeException();
            }

            if (match(_firstNode.Data))
            {
                _firstNode = _firstNode.Next;
                if (_firstNode != null)
                {
                    _firstNode.Previous = null;
                }
            }
            else if (match(_lastNode.Data))
            {
                _lastNode = _lastNode.Previous;
                if (_lastNode != null)
                {
                    _lastNode.Next = null;
                }
            }
            else
            {
                var currentNode = _firstNode;

                while (currentNode.Next != null)
                {
                    if (match(currentNode.Data))
                    {
                        break;
                    }

                    currentNode = currentNode.Next;
                }

                if (!match(currentNode.Data))
                {
                    throw new Exception("Item was not found!");
                }

                var newPrevious = currentNode.Previous;
                var newNext = currentNode.Next;

                if (newPrevious != null)
                {
                    newPrevious.Next = newNext;
                }

                if (newNext != null)
                {
                    newNext.Previous = newPrevious;
                }

                currentNode = newPrevious;
            }

            _count--;
        }

        public virtual void RemoveAt(int index)
        {
            if (IsEmpty() || index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (index == 0)
            {
                _firstNode = _firstNode.Next;

                if (_firstNode != null)
                {
                    _firstNode.Previous = null;
                }
            }
            else if (index == Count - 1)
            {
                _lastNode = _lastNode.Previous;

                if (_lastNode != null)
                {
                    _lastNode.Next = null;
                }
            }
            else
            {
                int i = 0;
                var currentNode = _firstNode;

                while (i < index)
                {
                    currentNode = currentNode.Next;
                    ++i;
                }

                var newPrevious = currentNode.Previous;
                var newNext = currentNode.Next;
                newPrevious.Next = newNext;

                if (newNext != null)
                {
                    newNext.Previous = newPrevious;
                }

                currentNode = newPrevious;
            }

            _count--;
        }

        public virtual void Clear()
        {
            _count = 0;
            _firstNode = _lastNode = null;
        }

        public virtual bool Contains(T dataItem)
        {
            try
            {
                return Find(dataItem).IsEqualTo(dataItem);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual T Find(T dataItem)
        {
            if (IsEmpty())
            {
                throw new Exception("List is empty.");
            }

            var currentNode = _firstNode;
            while (currentNode != null)
            {
                if (currentNode.Data.IsEqualTo(dataItem))
                {
                    return currentNode.Data;
                }

                currentNode = currentNode.Next;
            }

            throw new Exception("Item was not found.");
        }

        public virtual bool TryFindFirst(Predicate<T> match, out T found)
        {
            found = default(T);

            if (IsEmpty())
            {
                return false;
            }

            var currentNode = _firstNode;

            try
            {
                while (currentNode != null)
                {
                    if (match(currentNode.Data))
                    {
                        found = currentNode.Data;
                        return true;
                    }

                    currentNode = currentNode.Next;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public virtual T FindFirst(Predicate<T> match)
        {
            if (IsEmpty())
            {
                throw new Exception("List is empty.");
            }

            var currentNode = _firstNode;

            while (currentNode != null)
            {
                if (match(currentNode.Data))
                {
                    return currentNode.Data;
                }

                currentNode = currentNode.Next;
            }

            throw new KeyNotFoundException();
        }

        public virtual List<T> FindAll(Predicate<T> match)
        {
            if (IsEmpty())
            {
                throw new Exception("List is empty.");
            }

            var currentNode = _firstNode;
            var list = new List<T>();

            while (currentNode != null)
            {
                if (match(currentNode.Data))
                {
                    list.Add(currentNode.Data);
                }

                currentNode = currentNode.Next;
            }

            return list;
        }

        public virtual DLinkedList<T> GetRange(int index, int countOfElements)
        {
            DLinkedListNode<T> currentNode;
            DLinkedList<T> newList = new DLinkedList<T>();

            if (Count == 0)
            {
                return newList;
            }

            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (index > (Count / 2))
            {
                currentNode = this._lastNode;
                for (int i = (Count - 1); i > index; ++i)
                {
                    currentNode = currentNode.Previous;
                }
            }
            else
            {
                currentNode = this._firstNode;
                for (int i = 0; i < index; ++i)
                {
                    currentNode = currentNode.Next;
                }
            }

            while (currentNode != null && newList.Count <= countOfElements)
            {
                newList.Append(currentNode.Data);
                currentNode = currentNode.Next;
            }

            return newList;
        }

        public virtual void SelectionSort()
        {
            if (IsEmpty())
            {
                return;
            }

            var currentNode = _firstNode;

            while (currentNode != null)
            {
                var minNode = currentNode;
                var nextNode = currentNode.Next;
                while (nextNode != null)
                {
                    if (nextNode.Data.IsLessThan(minNode.Data))
                    {
                        minNode = nextNode.Next;
                    }

                    minNode = nextNode.Next;
                }

                if (minNode != currentNode)
                {
                    var temp = minNode.Data;
                    minNode.Data = currentNode.Data;
                    currentNode.Data = temp;
                }

                currentNode = currentNode.Next;
            }
        }

        public virtual T[] ToArray()
        {
            T[] array = new T[Count];

            var currentNode = _firstNode;

            for (int i = 0; i < Count; ++i)
            {
                if (currentNode != null)
                {
                    array[i] = currentNode.Data;
                    currentNode = currentNode.Next;
                }
                else
                {
                    break;
                }
            }

            return array;
        }

        public virtual List<T> ToList()
        {
            List<T> list = new List<T>();

            var currentNode = _firstNode;
            for (int i = 0; i < Count; ++i)
            {
                if (currentNode != null)
                {
                    list.Add(currentNode.Data);
                    currentNode = currentNode.Next;
                }
                else
                {
                    break;
                }
            }

            return list;
        }

        public virtual string ToReadable()
        {
            string listAsString = string.Empty;
            int i = 0;
            var currentNode = _firstNode;

            while (currentNode != null)
            {
                listAsString = String.Format("{0}[{1}] => {2}\r\n", listAsString, i, currentNode.Data);
                currentNode = currentNode.Next;
                ++i;
            }

            return listAsString;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new DLinkedListEnumerator(this);
            
            /*var node = _firstNode;
            while (node != null)
            {
                yield return node.Data;
                node = node.Next;
            }*/
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new DLinkedListEnumerator(this);

            /*return this.GetEnumerator();*/
        }

        internal class DLinkedListEnumerator : IEnumerator<T>
        {
            private DLinkedListNode<T> _current;
            private DLinkedList<T> _doublyLinkedList;

            public DLinkedListEnumerator(DLinkedList<T> list)
            {
                this._current = list.Head;
                this._doublyLinkedList = list;
            }

            public T Current
            {
                get { return this._current.Data; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (_current.Next != null)
                {
                    _current = _current.Next;
                }
                else
                {
                    return false;
                }

                return true;
            }

            public bool MovePrevious()
            {
                if (_current.Previous != null)
                {
                    _current = _current.Previous;
                }
                else
                {
                    return false;
                }

                return true;
            }

            public void Reset()
            {
                _current = _doublyLinkedList.Head;
            }

            public void Dispose()
            {
                _current = null;
                _doublyLinkedList = null;
            }
        }
    }
}
