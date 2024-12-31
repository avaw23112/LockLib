using System;
using System.Collections.Generic;
using LockLib.CurrentOrderedSet;
using LockLib.Lock;
using LockLib.Lock.LockObject;

namespace LockLib.Thread.ThreadObject;

public interface IThreadObject : IComparable<IThreadObject>, IEnumerator<IThreadObject>
{
    System.Threading.Thread Thread { get; set; }
    ConcurrentOrderedSet<ILock> LocksSet { get; }
    ILock PreparedLock { get; set; }
    bool IsWaiting { get; set; }
    public int ThreadIndex { get; }
    bool ContainLock(ILock keyLock);
    void HandleLock(ILock keyLock);
    void RemoveLock(ILock keyLock);
}