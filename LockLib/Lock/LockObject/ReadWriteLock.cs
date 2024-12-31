using System;
using System.Collections;
using System.Threading;

namespace LockLib.Lock.LockObject;

public class ReadWriteLock : IReadWriteLock
{
    private readonly ReaderWriterLockSlim _lock = new();
    private volatile bool _isLocked;
    private volatile System.Threading.Thread _thread;
    private static volatile int lockNumber = 0;
    private int _lockIndex;

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

    public string LockName { get; }

    public int LockIndex
    {
        get => _lockIndex;
        set => _lockIndex = value;
    }

    public ReadWriteLock()
    {
        LockName = "ReadWriteLock" + lockNumber;
        LockIndex = lockNumber;
        Interlocked.Increment(ref lockNumber);
    }

    public ReadWriteLock(string lockName)
    {
        LockName = lockName;
    }

    public void EnterReadLock()
    {
        _lock.EnterReadLock();
    }

    public void EnterWriteLock()
    {
        _lock.EnterWriteLock();
    }

    public void ExitReadLock()
    {
        _lock.ExitReadLock();
    }

    public void ExitWriteLock()
    {
        _lock.ExitWriteLock();
    }

    public override string ToString()
    {
        return LockName;
    }

    public void Dispose()
    {
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