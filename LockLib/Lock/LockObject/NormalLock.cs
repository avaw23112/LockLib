using System;
using System.Collections;
using System.Threading;

namespace LockLib.Lock.LockObject;

public class NormalLock : INormalLock
{
    private readonly object _lockObject = new();
    private volatile bool _isLocked;
    private volatile System.Threading.Thread _thread;
    private static volatile int _lockNumber = 0;
    private int _lockIndex;

    public string LockName { get; }

    public bool IsLocked
    {
        get => _isLocked;
        set => _isLocked = value;
    }

    public System.Threading.Thread HandleThread
    {
        get => _thread;
        set => _thread = value;
    }

    public int LockIndex
    {
        get => _lockIndex;
        set => _lockIndex = value;
    }

    public NormalLock()
    {
        LockName = "NormalLock" + _lockNumber;
        _lockIndex = _lockNumber;
        Interlocked.Increment(ref _lockNumber);
    }

    public NormalLock(string lockName)
    {
        LockName = lockName;
        _lockIndex = _lockNumber;
        Interlocked.Increment(ref _lockNumber);
    }

    public void Enter()
    {
        Monitor.Enter(_lockObject);
    }

    public void Exit()
    {
        Monitor.Exit(_lockObject);
    }

    public override string ToString()
    {
        return LockName;
    }

    public void Dispose()
    {
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (obj is ILock iLock) return LockIndex == iLock.LockIndex;
        return obj == this;
    }

    public override int GetHashCode()
    {
        var hash = 17;
        hash = hash * 23 + (_lockObject?.GetHashCode() ?? 0);
        return hash;
    }

    public int CompareTo(ILock other)
    {
        return LockIndex == other.LockIndex ? 0 : LockIndex < other.LockIndex ? -1 : 1;
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

    public ILock Current
    {
        get
        {
            if (_currentIndex != 0) throw new InvalidOperationException();
            return this; // Return the current ThreadOj instance
        }
    }

    object IEnumerator.Current => Current;
}