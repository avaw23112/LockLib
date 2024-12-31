namespace LockLib.Lock.LockObject;

public interface INormalLock : ILock
{
    void Enter();
    void Exit();
}