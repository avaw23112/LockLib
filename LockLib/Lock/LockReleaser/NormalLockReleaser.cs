using System;
using LockLib.Lock.LockObject;
using LockLib.Thread.ThreadObject;

namespace LockLib.Lock.LockReleaser;

public class NormalLockReleaser(Action<INormalLock, IThreadObject> dispositionAction) : IDisposable
{
    private INormalLock _normalLock;
    private IThreadObject _threadObject;

    public void ReSet(INormalLock normalLock, IThreadObject threadObject)
    {
        _normalLock = normalLock;
        _threadObject = threadObject;
    }

    public void Dispose()
    {
        dispositionAction(_normalLock, _threadObject);
    }
}