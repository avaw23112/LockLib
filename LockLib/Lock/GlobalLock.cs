using LockLib.Lock.LockReleaser;

namespace LockLib.Lock;

public static class GlobalLock
{
    private static readonly LockController.LockController LockController = new();

    public static NormalLockReleaser AutoLock(string normalLockName)
    {
        return LockController.AutoLock(normalLockName);
    }

    public static ReadLockReleaser AutoReadLock(string rwLockName)
    {
        return LockController.AutoReadLock(rwLockName);
    }

    public static WriteLockReleaser AutoWriteLock(string rwLockName)
    {
        return LockController.AutoWriteLock(rwLockName);
    }

    public static void Lock(string normalLockName)
    {
        LockController.Lock(normalLockName);
    }

    public static void ReadLock(string rwLockName)
    {
        LockController.ReadLock(rwLockName);
    }

    public static void WriteLock(string rwLockName)
    {
        LockController.WriteLock(rwLockName);
    }

    public static void UnLock(string normalLockName)
    {
        LockController.UnLock(normalLockName);
    }

    public static void UnReadLock(string rwLockName)
    {
        LockController.UnReadLock(rwLockName);
    }

    public static void UnWriteLock(string rwLockName)
    {
        LockController.UnWriteLock(rwLockName);
    }
}