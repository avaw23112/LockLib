namespace LockLib.Lock.LockObject;

public interface IReadWriteLock : ILock
{
    void EnterReadLock();
    void EnterWriteLock();
    void ExitReadLock();
    void ExitWriteLock();
}