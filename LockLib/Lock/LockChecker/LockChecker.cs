using LockLib.CurrentOrderedSet;
using LockLib.Lock.LockObject;
using LockLib.Lock.LockReleaser;
using LockLib.Thread.ThreadErrorPrinter;
using LockLib.Thread.ThreadObject;
using LockLib.Thread.ThreadObjectManager;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace LockLib.Lock.LockChecker;

public class LockChecker
{
    private readonly ThreadObjectManager _threadObjectManager = new();
    private readonly ConcurrentOrderedSet<IThreadObject> _waitingThreadSet = new();
    private readonly ConcurrentDictionary<INormalLock, NormalLockReleaser> _normalLockReleaser = new();
    private readonly ConcurrentDictionary<IReadWriteLock, ReadLockReleaser> _readLockReleaser = new();
    private readonly ConcurrentDictionary<IReadWriteLock, WriteLockReleaser> _writeLockReleaser = new();
    private readonly object _debugFlagLock = new();
    private bool _debugFlag = true;

    //——————————————————————————————————————————————————————————————————————————
    private void DebugLog(Exception ex)
    {
        if (_debugFlag)
            lock (_debugFlagLock)
            {
                if (_debugFlag)
                {
                    Log.Debug.Log(ex);
                    _debugFlag = false;
                }
            }
    }

    private void DeadLockCheck(ILock lockObject, IThreadObject threadObject, LockType lockType)
    {
        bool isLocked;
        lock (lockObject)
        {
            isLocked = lockObject.IsLocked;
        }

        //如果放入上面的锁块中，会导致自身出现顺序死锁！
        if (!isLocked)
            if (Locker.DifferenceLock(lockObject, threadObject, _waitingThreadSet, lockType))
                return;
#if DEBUG
        PrepareLock(lockObject, threadObject, lockType);
#endif
    }

    private void PrepareLock(ILock lockObject, IThreadObject threadObject, LockType lockType)
    {
        lock (threadObject)
        {
            threadObject.IsWaiting = true;
            threadObject.PreparedLock = lockObject;
            _waitingThreadSet.Add(threadObject);
        }

        SelfLockCheck(threadObject);
        while (!Locker.DifferenceLock(lockObject, threadObject, _waitingThreadSet, lockType))
        {
            UnOrderedLockCheck(lockObject, threadObject);
            CycleDeadLockCheck(threadObject);
        }
    }

    private void UnOrderedLockCheck(ILock lockObject, IThreadObject threadObject)
    {
        var handledThreadObject = _threadObjectManager.GetThreadObject(lockObject.HandleThread);
        lock (handledThreadObject)
        {
            try
            {
                if (handledThreadObject.IsWaiting && threadObject.ContainLock(handledThreadObject.PreparedLock))
                {
                    var exception =
                        new Exception(
                            "出现顺序死锁" + ThreadErrorPrinter.UnOrderErrorPrint(threadObject, handledThreadObject));
                    throw exception;
                }
            }
            catch (Exception e)
            {
                DebugLog(e);
                throw;
            }
        }
    }

    private void SelfLockCheck(IThreadObject threadObject)
    {
        var handledThreadObject =
            _threadObjectManager.GetThreadObject(threadObject.PreparedLock.HandleThread);
        try
        {
            if (threadObject == handledThreadObject)
            {
                var exception = new Exception("出现自循环，对同一个锁进行嵌套上锁！" + ThreadErrorPrinter.ErrorPrint(threadObject));
                throw exception;
            }
        }
        catch (Exception e)
        {
            DebugLog(e);
            throw;
        }
    }

    private void CycleDeadLockCheck(IThreadObject threadObject)
    {
        var handledThreadObject =
            _threadObjectManager.GetThreadObject(threadObject.PreparedLock.HandleThread);
        if (handledThreadObject == null)
            return;
        var count = _waitingThreadSet.Count();
        for (var i = 0; i < count; i++)
        {
            lock (handledThreadObject)
            {
                if (handledThreadObject.PreparedLock == null || !_waitingThreadSet.Contains(handledThreadObject))
                    return;
                if (threadObject.ContainLock(handledThreadObject.PreparedLock))
                    try
                    {
                        var stringBuilder = new StringBuilder();
                        foreach (var variable in _waitingThreadSet)
                            stringBuilder.Append(ThreadErrorPrinter.CycleErrorPrint(variable));
                        var exception = new Exception(" 出现循环等待!" + stringBuilder + "\n ");
                        throw exception;
                    }
                    catch (Exception e)
                    {
                        DebugLog(e);
                        throw;
                    }
            }

            handledThreadObject =
                _threadObjectManager.GetThreadObject(handledThreadObject.PreparedLock.HandleThread);
        }
    }

    //———————————————————————————————————————————————————————————————————————————

    private IThreadObject LockCore(INormalLock normalLock)
    {
        var threadObject = _threadObjectManager.GetThreadObject(System.Threading.Thread.CurrentThread);
        DeadLockCheck(normalLock, threadObject, LockType.NormalLock);
        return threadObject;
    }

    private IThreadObject WriteLockCore(IReadWriteLock rwLock)
    {
        var threadObject = _threadObjectManager.GetThreadObject(System.Threading.Thread.CurrentThread);
        DeadLockCheck(rwLock, threadObject, LockType.WriteLock);
        return threadObject;
    }

    private void ReadLockCore(IReadWriteLock rwLock)
    {
        var threadObject = _threadObjectManager.GetThreadObject(System.Threading.Thread.CurrentThread);
        DeadLockCheck(rwLock, threadObject, LockType.ReadLock);
    }

    public NormalLockReleaser AutoLock(INormalLock normalLock)
    {
        var threadObject = LockCore(normalLock);
        var normalLockReleaser = _normalLockReleaser.GetOrAdd(normalLock,
            new NormalLockReleaser(UnLocker.UnNormalLock));
        normalLockReleaser.ReSet(normalLock, threadObject);
        return normalLockReleaser;
    }

    public ReadLockReleaser AutoReadLock(IReadWriteLock rwLock)
    {
        ReadLockCore(rwLock);
        var readLockReleaser = _readLockReleaser.GetOrAdd(rwLock, new ReadLockReleaser(UnLocker.UnReadLock));
        readLockReleaser.ReSet(rwLock);
        return readLockReleaser;
    }

    public WriteLockReleaser AutoWriteLock(IReadWriteLock rwLock)
    {
        var threadObject = WriteLockCore(rwLock);
        ;
        var writeLockReleaser = _writeLockReleaser.GetOrAdd(rwLock,
            new WriteLockReleaser(UnLocker.UnWriteLock));
        writeLockReleaser.ReSet(rwLock, threadObject);
        return writeLockReleaser;
    }

    public void Lock(INormalLock normalLock)
    {
        LockCore(normalLock);
    }

    public void ReadLock(IReadWriteLock rwLock)
    {
        ReadLockCore(rwLock);
    }

    public void WriteLock(IReadWriteLock rwLock)
    {
        WriteLockCore(rwLock);
    }

    public void UnLock(INormalLock normalLock)
    {
        var threadObject = _threadObjectManager.GetThreadObject(System.Threading.Thread.CurrentThread);
        UnLocker.UnNormalLock(normalLock, threadObject);
    }

    public void UnReadLock(IReadWriteLock rwLock)
    {
        UnLocker.UnReadLock(rwLock);
    }

    public void UnWriteLock(IReadWriteLock rwLock)
    {
        var threadObject = _threadObjectManager.GetThreadObject(System.Threading.Thread.CurrentThread);
        UnLocker.UnWriteLock(rwLock, threadObject);
    }
}