using LockLib.CurrentOrderedSet;
using LockLib.Lock.LockObject;
using LockLib.Thread.ThreadObject;
using System;

namespace LockLib.Lock.LockChecker;

public static class Locker
{
    public static bool NormalLock(INormalLock normalLock, IThreadObject threadObject,
        ConcurrentOrderedSet<IThreadObject> waitingThreadSet)
    {
#if DEBUG
        lock (normalLock)
        {
            if (normalLock.IsLocked) return false;
            normalLock.IsLocked = true;
            normalLock.HandleThread = threadObject.Thread;
        }
#endif
        normalLock.Enter();
#if DEBUG
        lock (threadObject)
        {
            threadObject.HandleLock(normalLock);
            if (!threadObject.IsWaiting) return true;
            threadObject.IsWaiting = false;
            threadObject.PreparedLock = null;
            waitingThreadSet.Remove(threadObject);
        }
#endif
        return true;
    }

    public static bool ReadLock(IReadWriteLock readLock)
    {
        readLock.EnterReadLock();
        return true;
    }

    public static bool WriteLock(IReadWriteLock writeLock, IThreadObject threadObject,
        ConcurrentOrderedSet<IThreadObject> waitingThreadSet)
    {
#if DEBUG
        lock (writeLock)
        {
            if (writeLock.IsLocked) return false;
            writeLock.IsLocked = true;
            writeLock.HandleThread = threadObject.Thread;
        }
#endif
        writeLock.EnterWriteLock();
#if DEBUG
        lock (threadObject)
        {
            threadObject.HandleLock(writeLock);
            if (!threadObject.IsWaiting) return true;
            threadObject.IsWaiting = false;
            threadObject.PreparedLock = null;
            waitingThreadSet.Remove(threadObject);
        }
#endif
        return true;
    }

    public static bool DifferenceLock(ILock lockObject, IThreadObject threadObject,
        ConcurrentOrderedSet<IThreadObject> waitingThreadSet, LockType lockType)
    {
        switch (lockType)
        {
            case LockType.NormalLock:
            {
                if (lockObject is NormalLock normalLock) return NormalLock(normalLock, threadObject, waitingThreadSet);
            }
                break;
            case LockType.ReadLock:
            {
                if (lockObject is ReadWriteLock readLock) return ReadLock(readLock);
            }
                break;
            case LockType.WriteLock:
            {
                if (lockObject is ReadWriteLock writeLock) return WriteLock(writeLock, threadObject, waitingThreadSet);
            }
                break;
            default:
                throw new Exception("Invalid lock type");
        }

        return false;
    }
}