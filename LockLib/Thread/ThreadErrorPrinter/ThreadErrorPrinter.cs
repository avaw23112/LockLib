using System;
using LockLib.CurrentOrderedSet;
using LockLib.Lock.LockObject;
using LockLib.Thread.ThreadObject;
using System.Text;

namespace LockLib.Thread.ThreadErrorPrinter;

public static class ThreadErrorPrinter
{
    private static string PrintLockOrder(IThreadObject threadObject)
    {
        var sb = new StringBuilder();
        var set = threadObject.LocksSet;
        var count = set.Count();
        for (var i = 0; i < count; i++)
            if (i != count - 1)
                sb.Append(set.ElementAt(i) + " -> ");
            else sb.Append(set.ElementAt(i));
        return sb.ToString();
    }

    public static string ErrorPrint(IThreadObject threadObject)
    {
        var sb = new StringBuilder();
        sb.Append("\n { 线程 " + threadObject + " 持有：");
        sb.Append(" [ " + PrintLockOrder(threadObject) + " ] } ");
        sb.Append(" = 预备 > " + threadObject.PreparedLock);
        return sb.ToString();
    }

    public static string CycleErrorPrint(IThreadObject threadObject)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(ErrorPrint(threadObject));
        stringBuilder.Append(",");
        stringBuilder.Append(threadObject);
        stringBuilder.Append("对");
        stringBuilder.Append(threadObject.PreparedLock);
        stringBuilder.Append("的等待导致饥饿");
        return stringBuilder.ToString();
    }

    public static string UnOrderErrorPrint(IThreadObject threadObject, IThreadObject handledThreadObject)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(ErrorPrint(threadObject));
        stringBuilder.Append(ErrorPrint(handledThreadObject));
        stringBuilder.Append("\n");
        stringBuilder.Append(threadObject);
        stringBuilder.Append("与 ");
        stringBuilder.Append(handledThreadObject);
        stringBuilder.Append(" 持锁顺序并不相容！");
        return stringBuilder.ToString();
    }
}