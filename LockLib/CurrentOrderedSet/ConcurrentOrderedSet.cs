using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LockLib.CurrentOrderedSet;

public class ConcurrentOrderedSet<T> where T : notnull, IComparable<T>, IEnumerator<T>
{
    private readonly SortedSet<T> _sortedSet = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public void Add(T item)
    {
        _lock.EnterWriteLock();
        try
        {
            _sortedSet.Add(item);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool Contains(T item)
    {
        _lock.EnterReadLock();
        try
        {
            return _sortedSet.Contains(item);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public int Count()
    {
        _lock.EnterReadLock();
        try
        {
            return _sortedSet.Count;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public bool Remove(T item)
    {
        _lock.EnterWriteLock();
        try
        {
            return _sortedSet.Remove(item);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool TryGetValue(T item, out T value)
    {
        _lock.EnterReadLock();
        try
        {
            return _sortedSet.TryGetValue(item, out value);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        // 进入读锁
        _lock.EnterReadLock();
        try
        {
            foreach (var item in _sortedSet) yield return item;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void Clear()
    {
        _lock.EnterWriteLock();
        try
        {
            _sortedSet.Clear();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public T Last()
    {
        _lock.EnterReadLock();
        try
        {
            return _sortedSet.Last();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public T First()
    {
        _lock.EnterReadLock();
        try
        {
            return _sortedSet.First();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public T ElementAt(int index)
    {
        _lock.EnterReadLock();
        try
        {
            return _sortedSet.ElementAt(index);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public override string ToString()
    {
        _lock.EnterReadLock();
        try
        {
            var sb = new StringBuilder();
            sb.Append("[");
            for (var i = 0; i < _sortedSet.Count; i++)
                if (i < _sortedSet.Count - 1) sb.Append(_sortedSet.ElementAt(i) + ",");
                else sb.Append(_sortedSet.ElementAt(i));
            sb.Append("]");
            return sb.ToString();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
}