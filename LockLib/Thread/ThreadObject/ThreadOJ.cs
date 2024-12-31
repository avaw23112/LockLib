using LockLib.CurrentOrderedSet;
using LockLib.Lock.LockObject;
using System;
using System.Collections;
using System.Threading;

namespace LockLib.Thread.ThreadObject;

public class ThreadOj : IThreadObject
{
    private volatile bool _isWaiting;
    private volatile System.Threading.Thread _thread;
    private volatile ILock _preparedLock;
    private readonly ConcurrentOrderedSet<ILock> _locksSet = new();
    private static volatile int _threadNumber;
    private readonly int _threadIndex;

    private readonly string _threadName;

    public int ThreadIndex => _threadIndex;
    public ConcurrentOrderedSet<ILock> LocksSet => _locksSet;

    public ThreadOj(System.Threading.Thread thread)
    {
        _isWaiting = false;
        _thread = thread;
        _preparedLock = null;
        if (string.IsNullOrEmpty(thread.Name))
            _threadName = "thread " + _threadNumber;
        else
            _threadName = thread.Name;
        _threadIndex = _threadNumber;
        Interlocked.Increment(ref _threadNumber);
    }

    public System.Threading.Thread Thread
    {
        get => _thread;
        set => _thread = value;
    }

    public ILock PreparedLock
    {
        get => _preparedLock;
        set => _preparedLock = value;
    }

    public bool IsWaiting
    {
        get => _isWaiting;
        set => _isWaiting = value;
    }

    //————————————————————————————————————————————————————————————————————————————————

    public bool ContainLock(ILock keyLock)
    {
        return _locksSet.Contains(keyLock);
    }

    public void HandleLock(ILock keyLock)
    {
        _locksSet.Add(keyLock);
    }

    public void RemoveLock(ILock keyLock)
    {
        _locksSet.Remove(keyLock);
    }

    //————————————————————————————————————————————————————————————————————————————————

    public override string ToString()
    {
        return _threadName;
    }

    public void Dispose()
    {
        _locksSet.Clear();
    }

    public int CompareTo(IThreadObject other)
    {
        return ThreadIndex == other.ThreadIndex ? 0 : ThreadIndex < other.ThreadIndex ? -1 : 1;
    }

    private int _currentIndex = -1;

    public bool MoveNext()
    {
        _currentIndex++;
        return _currentIndex < 1;
    }

    public void Reset()
    {
        _currentIndex = -1;
    }

    public IThreadObject Current
    {
        get
        {
            if (_currentIndex != 0) throw new InvalidOperationException();
            return this; // Return the current ThreadOj instance
        }
    }

    object IEnumerator.Current => Current;
}