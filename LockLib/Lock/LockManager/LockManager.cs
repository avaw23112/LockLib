using System.Collections.Concurrent;
using LockLib.Lock.LockObject;

namespace LockLib.Lock.LockManager;

public class LockManager
{
    private readonly ConcurrentDictionary<string, INormalLock> _normalLocksDc = new();
    private readonly ConcurrentDictionary<string, IReadWriteLock> _readWriteLocksDc = new();

    public INormalLock GetNormalLock(string key)
    {
        if (!_normalLocksDc.ContainsKey(key))
            lock (_normalLocksDc)
            {
                if (!_normalLocksDc.ContainsKey(key)) _normalLocksDc.TryAdd(key, new NormalLock(key));
            }

        return _normalLocksDc[key];
    }

    public IReadWriteLock GetReadWriteLock(string key)
    {
        if (!_readWriteLocksDc.ContainsKey(key))
            lock (_readWriteLocksDc)
            {
                if (!_readWriteLocksDc.ContainsKey(key)) _readWriteLocksDc.TryAdd(key, new ReadWriteLock(key));
            }

        return _readWriteLocksDc[key];
    }
}