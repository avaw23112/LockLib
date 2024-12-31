using System;
using LockLib.Lock.LockObject;
using LockLib.Thread.ThreadObject;

namespace LockLib.Lock.LockReleaser;

public class WriteLockReleaser(Action<IReadWriteLock, IThreadObject> dispositionAction) : IDisposable
{
    private IReadWriteLock _writeLock;
    private IThreadObject _threadObject;

    public void ReSet(IReadWriteLock writeLock, IThreadObject threadObject)
    {
        _writeLock = writeLock;
        _threadObject = threadObject;
    }

    public void Dispose()
    {
        dispositionAction(_writeLock, _threadObject);
    }
}