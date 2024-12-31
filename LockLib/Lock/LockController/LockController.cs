using LockLib.Lock.LockObject;
using LockLib.Lock.LockReleaser;

namespace LockLib.Lock.LockController;

public class LockController
{
    private LockChecker.LockChecker _LockChecker = new();
    private LockManager.LockManager _LockManager = new();

    public NormalLockReleaser AutoLock(string normalLockName)
    {
        var normalLock = _LockManager.GetNormalLock(normalLockName);
        return _LockChecker.AutoLock(normalLock);
    }

    public ReadLockReleaser AutoReadLock(string rwLockName)
    {
        var readWriteLock = _LockManager.GetReadWriteLock(rwLockName);
        return _LockChecker.AutoReadLock(readWriteLock);
    }

    public WriteLockReleaser AutoWriteLock(string rwLockName)
    {
        var readWriteLock = _LockManager.GetReadWriteLock(rwLockName);
        return _LockChecker.AutoWriteLock(readWriteLock);
    }

    public void Lock(string normalLockName)
    {
        var normalLock = _LockManager.GetNormalLock(normalLockName);
        _LockChecker.Lock(normalLock);
    }

    public void ReadLock(string rwLockName)
    {
        var readWriteLock = _LockManager.GetReadWriteLock(rwLockName);
        _LockChecker.ReadLock(readWriteLock);
    }

    public void WriteLock(string rwLockName)
    {
        var readWriteLock = _LockManager.GetReadWriteLock(rwLockName);
        _LockChecker.WriteLock(readWriteLock);
    }

    public void UnLock(string normalLockName)
    {
        var normalLock = _LockManager.GetNormalLock(normalLockName);
        _LockChecker.UnLock(normalLock);
    }

    public void UnReadLock(string rwLockName)
    {
        var readWriteLock = _LockManager.GetReadWriteLock(rwLockName);
        _LockChecker.UnReadLock(readWriteLock);
    }

    public void UnWriteLock(string rwLockName)
    {
        var readWriteLock = _LockManager.GetReadWriteLock(rwLockName);
        _LockChecker.UnWriteLock(readWriteLock);
    }
}