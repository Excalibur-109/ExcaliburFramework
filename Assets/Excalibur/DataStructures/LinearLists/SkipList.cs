using System;
using System.Collections.Generic;

namespace Excalibur.DataStructures
{
    public class SkipListNode<T> : IComparable<SkipListNode<T>> where T : IComparable<T>
    {
        private T _value;
        private SkipListNode<T>[] _forwards;

        public SkipListNode(T value, int level)
        {
            if (level < 0)
            {
                throw new ArgumentOutOfRangeException("Invalid value for level.");
            }

            Value = value;
            Forwards = new SkipListNode<T>[level];
        }

        public virtual T Value
        {
            get { return this._value; }
            private set { this._value = value; }
        }

        public virtual SkipListNode<T>[] Forwards
        {
            get { return this._forwards; }
            private set { this._forwards = value; }
        }

        public virtual int Level
        {
            get { return Forwards.Length; }
        }

        public int CompareTo(SkipListNode<T> other)
        {
            if (other == null)
            {
                return -1;
            }

            return this.Value.CompareTo(other.Value);
        }
    }

    /// <summary>
    /// The Skip-List Data Structure.
    /// </summary>
    public class SkipList<T> : ICollection<T>, IEnumerable<T> where T: IComparable<T>
    {
        private int _count { get; set; }
        private int _currentMaxLevel { get; set; }
        private Random _randomizer { get; set; }

        private SkipListNode<T> _firstNode { get; set; }

        private readonly int MaxLevel = 32;
        private readonly double Probability = 0.5;

        public SkipList()
        {
            _count = 0;
            _currentMaxLevel = 0;
            _randomizer = new Random();
            _firstNode = new SkipListNode<T>(default(T), MaxLevel);

            for (int i = 0; i < MaxLevel; ++i)
            {
                _firstNode.Forwards[i] = _firstNode;
            }
        }

        public SkipListNode<T> Root
        {
            get { return _firstNode; }
        }

        public int Count
        {
            get { return _count; }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        public int Level
        {
            get { return _currentMaxLevel; }
        }

        public T this[int index]
        {
            get 
            {
                // TODO:
                throw new NotImplementedException();
            }
        }

        private int _getNextLevel()
        {
            int lvl = 0;
            while (_randomizer.NextDouble() < Probability && lvl <=_currentMaxLevel && lvl < MaxLevel)
            {
                ++lvl;
            }

            return lvl;
        }

        public void Add(T item)
        {
            var current = _firstNode;
            var toBeUpdated = new SkipListNode<T>[MaxLevel];

            for (int i = _currentMaxLevel - 1; i >= 0; --i)
            {
                while (current.Forwards[i] != _firstNode && current.Forwards[i].Value.IsLessThan(item))
                {
                    current = current.Forwards[i];
                }

                toBeUpdated[i] = current;
            }

            current = current.Forwards[0];

            int lvl = _getNextLevel();
            if (lvl > _currentMaxLevel)
            {
                for (int i = _currentMaxLevel; i < lvl; ++i)
                {
                    toBeUpdated[i] = _firstNode;
                }

                _currentMaxLevel = lvl;
            }

            var newNode = new SkipListNode<T>(item, lvl);

            for (int i = 0; i < lvl; ++i)
            {
                newNode.Forwards[i] = toBeUpdated[i].Forwards[i];
                toBeUpdated[i].Forwards[i] = newNode;
            }

            ++_count;
        }

        public bool Remove(T item)
        {
            T deleted;
            return Remove(item, out deleted);
        }

        public bool Remove(T item, out T deleted)
        {
            var current = _firstNode;
            var toBeUpdated = new SkipListNode<T>[MaxLevel];

            for (int i = _currentMaxLevel - 1; i >= 0; --i)
            {
                while (current.Forwards[i] != _firstNode && current.Forwards[i].Value.IsLessThan(item))
                {
                    current = current.Forwards[i];
                }

                toBeUpdated[i] = current;
            }

            current = current.Forwards[0];

            if (current.Value.IsEqualTo(item) == false)
            {
                deleted = default(T);
                return false;
            }

            for (int i = 0; i < _currentMaxLevel; ++i)
            {
                if (toBeUpdated[i].Forwards[i] == current)
                {
                    toBeUpdated[i].Forwards[i] = current.Forwards[i];
                }
            }

            --_count;

            while (_currentMaxLevel > 1 && _firstNode.Forwards[_currentMaxLevel - 1] == _firstNode)
            {
                --_currentMaxLevel;
            }

            deleted = current.Value;
            return true;
        }

        public bool Contains(T item)
        {
            T itemOut;
            return Find(item, out itemOut);
        }

        public bool Find(T item, out T result)
        {
            var current = _firstNode;

            for (int i = _currentMaxLevel - 1; i >= 0; --i)
            {
                while (current.Forwards[i] != _firstNode && current.Forwards[i].Value.IsLessThan(item))
                {
                    current = current.Forwards[i];
                }
            }

            current = current.Forwards[0];

            if (current.Value.IsEqualTo(item))
            {
                result = current.Value;
                return true;
            }

            result = default(T);
            return false;
        }

        public T DeleteMin()
        {
            T min;

            if (!TryDeleteMin(out min))
            {
                throw new InvalidOperationException("SkipList is empty.");
            }

            return min;
        }

        public bool TryDeleteMin(out T result)
        {
            if (IsEmpty)
            {
                result = default(T);
                return false;
            }

            return Remove(_firstNode.Forwards[0].Value, out result);
        }

        public T Peek()
        {
            T peek;

            if (!TryPeek(out peek))
            {
                throw new InvalidOperationException("SkipList is empty.");
            }

            return peek;
        }

        public bool TryPeek(out T result)
        {
            if (IsEmpty)
            {
                result = default(T);
                return false;
            }

            result = _firstNode.Forwards[0].Value;
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var node = _firstNode;
            while (node.Forwards[0] != null && node.Forwards[0] != _firstNode)
            {
                node = node.Forwards[0];
                yield return node.Value;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }

            if (array.Length == 0 || arrayIndex >= array.Length || arrayIndex < 0)
            {
                throw new IndexOutOfRangeException();
            }

            var enumerator = this.GetEnumerator();

            for (int i = arrayIndex; i < array.Length; ++i)
            {
                if (enumerator.MoveNext())
                {
                    array[i] = enumerator.Current;
                }
                else
                {
                    break;
                }
            }
        }

        public void Clear()
        {
            _count = 0;
            _currentMaxLevel = 1;
            _randomizer = new Random();
            _firstNode = new SkipListNode<T>(default(T), MaxLevel);

            for (int i = 0; i < MaxLevel; ++i)
            {
                _firstNode.Forwards[i] = _firstNode;
            }
        }
    }
}
