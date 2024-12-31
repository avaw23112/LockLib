using System.Collections.Concurrent;
using LockLib.Thread.ThreadObject;
using System.Threading;

namespace LockLib.Thread.ThreadObjectManager;

public class ThreadObjectManager
{
    private readonly ConcurrentDictionary<System.Threading.Thread, IThreadObject> _threadObjectDc = new();

    public IThreadObject GetThreadObject(System.Threading.Thread thread)
    {
        if (!_threadObjectDc.ContainsKey(thread))
            lock (_threadObjectDc)
            {
                if (!_threadObjectDc.ContainsKey(thread)) _threadObjectDc.TryAdd(thread, new ThreadOj(thread));
            }

        return _threadObjectDc[thread];
    }
}