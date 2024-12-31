using System;
using LockLib.Lock.LockObject;
using LockLib.Thread.ThreadObject;

namespace LockLib.Lock.LockReleaser;

public class ReadLockReleaser(Action<IReadWriteLock> dispositionAction) : IDisposable
{
    private IReadWriteLock _readLock;

    public void ReSet(IReadWriteLock readLock)
    {
        _readLock = readLock;
    }

    public void Dispose()
    {
        dispositionAction(_readLock);
    }
}