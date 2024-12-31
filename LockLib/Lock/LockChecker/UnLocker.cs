using System;
using LockLib.CurrentOrderedSet;
using LockLib.Lock.LockObject;
using LockLib.Thread.ThreadObject;

namespace LockLib.Lock.LockChecker;

public static class UnLocker
{
    public static void UnNormalLock(INormalLock normalLock, IThreadObject threadObject)
    {
#if DEBUG
        lock (normalLock)
        {
            normalLock.IsLocked = false;
        }

        lock (threadObject)
        {
            threadObject.RemoveLock(normalLock);
        }
#endif
        normalLock.Exit();
    }

    public static void UnReadLock(IReadWriteLock readLock)
    {
        readLock.ExitReadLock();
    }

    public static void UnWriteLock(IReadWriteLock writeLock, IThreadObject threadObject)
    {
#if DEBUG
        lock (writeLock)
        {
            writeLock.IsLocked = false;
        }

        lock (threadObject)
        {
            threadObject.RemoveLock(writeLock);
        }
#endif
        writeLock.ExitWriteLock();
    }

    public static void DifferenceUnLock(ILock lockObject, IThreadObject threadObject,
        ConcurrentOrderedSet<IThreadObject> waitingThreadSet, LockType lockType)
    {
        switch (lockType)
        {
            case LockType.NormalLock:
            {
                if (lockObject is NormalLock normalLock) UnNormalLock(normalLock, threadObject);
            }
                break;
            case LockType.ReadLock:
            {
                if (lockObject is ReadWriteLock readLock) UnReadLock(readLock);
            }
                break;
            case LockType.WriteLock:
            {
                if (lockObject is ReadWriteLock writeLock) UnWriteLock(writeLock, threadObject);
            }
                break;
            default:
                throw new Exception("Invalid lock type");
        }
    }
}