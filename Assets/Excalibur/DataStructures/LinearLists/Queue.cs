using System;
using System.Collections.Generic;
using System.Linq;

namespace Excalibur.DataStructures
{
    /// <summary>
    /// The (FIFO) Queue Data Structure
    /// </summary>
    public class Queue<T> : IEnumerable<T> where T : IComparable<T>
    {
        private int _size { get; set; }
        private int _headPointer { get; set; }
        private int _tailPointer { get; set; }

        private T[] _collection { get; set; }

        private const int _defaultCapacity = 8;

        bool DefaultMaxCapacityIsx64 = true;
        bool IsMaximumCapacityReached = false;

        public const int MAXIMUM_ARRAY_LENGTH_x64 = 0X7FEFFFFF; //x64
        public const int MAXIMUM_ARRAY_LENGTH_x86 = 0x8000000;  //x86

        public Queue() : this(_defaultCapacity) { }

        public Queue(int initialCapacity)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            _size = 0;
            _headPointer = 0;
            _tailPointer = 0;
            _collection = new T[initialCapacity];
        }

        public int Count
        {
            get { return _size; }
        }

        public bool IsEmpty
        {
            get { return _size == 0; }
        }

        public T Top
        {
            get 
            {
                if (IsEmpty)
                {
                    throw new Exception("Queue is empty.");
                }

                return _collection[_headPointer];
            }
        }

        private void _resize(int newSize)
        {
            if (newSize > _size && !IsMaximumCapacityReached)
            {
                int capacity = (_collection.Length == 0 ? _defaultCapacity : _collection.Length * 2);

                int maxCapacity = (DefaultMaxCapacityIsx64 == true ? MAXIMUM_ARRAY_LENGTH_x64 : MAXIMUM_ARRAY_LENGTH_x86);

                if (capacity < newSize)
                {
                    capacity = newSize;
                }

                if (capacity >= maxCapacity)
                {
                    capacity = maxCapacity - 1;
                    IsMaximumCapacityReached = true;
                }

                try
                {
                    var tempCollection = new T[newSize];
                    Array.Copy(_collection, _headPointer, tempCollection, 0, _size);
                    _collection = tempCollection;
                }
                catch (OutOfMemoryException)
                {
                    if (DefaultMaxCapacityIsx64 == true)
                    {
                        DefaultMaxCapacityIsx64 = false;
                        _resize(capacity);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public void Enqueue(T dataItem)
        {
            if (_size == _collection.Length)
            {
                try
                {
                    _resize(_collection.Length * 2);
                }
                catch (OutOfMemoryException ex)
                {
                    throw ex;
                }
            }

            _collection[_tailPointer++] = dataItem;

            if (_tailPointer == _collection.Length)
                _tailPointer = 0;

            _size++;
        }

        public T Dequeue()
        {
            if (IsEmpty)
            {
                throw new Exception("Queue is empty.");
            }

            var topItem = _collection[_headPointer];
            _collection[_headPointer] = default(T);

            _size--;

            _headPointer++;

            if (_headPointer == _collection.Length)
            {
                _headPointer = 0;
            }

            if (_size > 0 && _collection.Length > _defaultCapacity && _size <= _collection.Length / 4)
            {
                var head = _collection[_headPointer];
                var tail = _collection[_tailPointer];

                _resize((_collection.Length / 3) * 2);

                _headPointer = Array.IndexOf(_collection, head);
                _tailPointer = Array.IndexOf(_collection, tail);
            }

            return topItem;
        }

        public T[] ToArray()
        {
            var array = new T[_size];

            int j = 0;
            for (int i = 0; i < _size; ++i)
            {
                array[j] = _collection[_headPointer + i];
                j++;
            }

            return array;
        }

        public string ToReadable()
        {
            var array = ToArray();
            string listAsString = string.Empty;

            for (int i = 0; i < Count; ++i)
            {
                listAsString = String.Format("{0}[{1}] => {2}\r\n", listAsString, i, array[i]);
            }

            return listAsString;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator() as IEnumerator<T>;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
