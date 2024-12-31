using System;
using System.Collections.Generic;

namespace LockLib.Lock.LockObject;

public interface ILock : IComparable<ILock>, IEnumerator<ILock>
{
    public int LockIndex { get; set; }
    public string LockName { get; }
    bool IsLocked { get; set; }
    System.Threading.Thread HandleThread { get; set; }
}