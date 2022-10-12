using System;
using System.Collections.Generic;

namespace Excalibur.DataStructures
{
    /// <summary>
    /// The Singly-Linked List Node.
    /// </summary>
    public class SLinkedListNode<T> : IComparable<SLinkedListNode<T>> where T : IComparable<T>
    {
        private T _data;
        private SLinkedListNode<T> _next;

        public SLinkedListNode()
        {
            Next = null;
            Data = default(T);
        }

        public SLinkedListNode(T dataItem)
        {
            Next = null;
            Data = dataItem;
        }

        public T Data
        {
            get { return _data; }
            set { this._data = value; }
        }

        public SLinkedListNode<T> Next
        {
            get { return _next; }
            set { this._next = value; }
        }

        public int CompareTo(SLinkedListNode<T> other)
        {
            if (other == null)
            {
                return -1;
            }

            return this.Data.CompareTo(other.Data);
        }
    }

    /// <summary>
    /// The Singly-Linked List Data Structure.
    /// </summary>
    public class SLinkedList<T> : IEnumerable<T> where T : IComparable<T>
    {
        private int _count;
        private SLinkedListNode<T> _firstNode { get; set; }
        private SLinkedListNode<T> _lastNode { get; set; }

        public SLinkedList()
        {
            _firstNode = null;
            _lastNode = null;
            _count = 0;
        }

        public int Count { get { return _count; } }

        public virtual SLinkedListNode<T> Head { get { return this._firstNode; } }

        public  T First
        {
            get { return (_firstNode == null ? default(T) : _firstNode.Data); }
        }

        public T Last
        {
            get
            {
                if (Count == 0)
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

        public bool IsEmpty()
        {
            return (Count == 0);
        }

        public void Prepend(T dataItem)
        {
            SLinkedListNode<T> newNode = new SLinkedListNode<T>(dataItem);

            if (_firstNode == null)
            {
                _firstNode = _lastNode = newNode;
            }
            else
            {
                var currentNode = _firstNode;
                newNode.Next = currentNode;
                _firstNode = newNode;
            }

            _count++;
        }

        public void Append(T dataItem)
        {
            SLinkedListNode<T> newNode = new SLinkedListNode<T>(dataItem);

            if (_firstNode == null)
            {
                _firstNode = _lastNode = newNode;
            }
            else
            {
                var currentNode = _lastNode;
                currentNode.Next = newNode;
                _lastNode = newNode;
            }

            _count++;
        }

        public void InsertAt(T dataItem, int index)
        {
            if (index == 0)
            {
                Prepend(dataItem);
            }
            else if (index == Count)
            {
                Append(dataItem);
            }
            else if (index > 0 && index < Count)
            {
                var currentNode = _firstNode;
                var newNode = new SLinkedListNode<T>(dataItem);

                for (int i = 0; i < index; ++i)
                {
                    currentNode = currentNode.Next;
                }

                newNode.Next = currentNode.Next;
                currentNode.Next = newNode;

                _count++;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public void RemoveAt(int index)
        {
            if (IsEmpty() || index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (index == 0)
            {
                _firstNode = _firstNode.Next;

                _count--;
            }
            else if (index == Count - 1)
            {
                var currentNode = _firstNode;
                while (currentNode.Next != null && currentNode.Next != _lastNode)
                {
                    currentNode = currentNode.Next;
                }

                currentNode.Next = null;
                _lastNode = currentNode;

                _count--;
            }
            else
            {
                int i = 0;
                var currentNode = _firstNode;
                while (currentNode.Next != null)
                {
                    if (i + 1 == index)
                    {
                        currentNode.Next = currentNode.Next.Next;

                        _count--;
                        break;
                    }

                    ++i;
                    currentNode = currentNode.Next;
                }
            }
        }

        public void Clear()
        {
            _firstNode = null;
            _lastNode = null;
            _count = 0;
        }

        public T GetAt(int index)
        {
            if (index == 0)
            {
                return First;
            }

            if (index == Count - 1)
            {
                return Last;
            }

            if (index > 0 && index < (Count - 1))
            {
                var currentNode = _firstNode;
                for (int i = 0; i < index; ++i)
                {
                    currentNode = currentNode.Next;
                }
                return currentNode.Data;
            }

            throw new IndexOutOfRangeException();
        }

        public SLinkedList<T> GetRange(int index, int countOfElements)
        {
            SLinkedList<T> newList = new SLinkedList<T>();
            var currentNode = this._firstNode;

            if (Count == 0)
            {
                return newList;
            }

            if (index < 0 || index >= Count) // index > Count
            {
                throw new IndexOutOfRangeException();
            }

            for (int i = 0; i < index; ++i)
            {
                currentNode = currentNode.Next;
            }

            while (currentNode != null && newList.Count != countOfElements)
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
                        minNode = nextNode;
                    }

                    nextNode = nextNode.Next;
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

        public T[] ToArray()
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

        public List<T> ToList()
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

        public string ToReadable()
        {
            int i = 0;
            var currentNode = _firstNode;
            string listAsString = string.Empty;

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
            return new SLinkedListEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SLinkedListEnumerator(this);
        }

        internal class SLinkedListEnumerator : IEnumerator<T>
        {
            private SLinkedListNode<T> _current;
            private SLinkedList<T> _singlyLinedList;

            public SLinkedListEnumerator(SLinkedList<T> list)
            {
                this._singlyLinedList = list;
                this._current = list.Head;
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
                _current = _current.Next;
                return (this._current != null);
            }

            public void Reset()
            {
                _current = _singlyLinedList.Head;
            }

            public void Dispose()
            {
                _current = null;
                _singlyLinedList = null;
            }
        }
    }
}