using System;
using System.Threading.Tasks;
using NUnit.Framework;
using LockLib.Lock.LockChecker;
using LockLib.Lock.LockObject;

namespace Test.LockCheckerTest
{
    [TestFixture]
    public class LockCheckerTest
    {

        [Test]
        public void Test()
        {
            //测试资源能否被正常释放
            LockChecker checker = new LockChecker();
            INormalLock normalLock = new NormalLock();
            using (checker.AutoLock(normalLock))
            {
                Console.WriteLine(normalLock);
            }
            Console.WriteLine(normalLock);
        }

        private void Lock(LockChecker checker,INormalLock normalLock,string threadName)
        {
            using (checker.AutoLock(normalLock))
            {
                Console.WriteLine(normalLock + " " + threadName);
            }
        }
        private static void UnOrderLock1(LockChecker checker,INormalLock normalLock1,  INormalLock normalLock2, string thread1)
        {
            using (checker.AutoLock(normalLock1))
            {
                using (checker.AutoLock(normalLock2))
                {
                    
                }
            }
        }
        //核心测试
            //1.能否顺利上锁
            //2.能否顺利解锁
            //2.能否检测出顺序死锁，循环死锁
            //3.自身会不会出现死锁？
        
        //双线程
        //线程池
        [Test]
        public async Task Test2()
        {
            LockChecker checker = new LockChecker();
            INormalLock normalLock = new NormalLock();
            Boolean flag = true;
            //测试多线程下能否运行
            Task task1 = Task.Run(() =>
            {
                while (flag)
                {
                    Lock(checker, normalLock, "Thread1");
                }
            });
            Task task2 = Task.Run(() =>
            {
                while (flag)
                {
                    Lock(checker, normalLock, "Thread2");
                }
            });
            Console.ReadLine();
            flag = false;
        }
        [Test]
        public async Task Test3()
        {
            //测试资源能否被正常释放
            LockChecker checker = new LockChecker();
            INormalLock normalLock1 = new NormalLock();
            INormalLock normalLock2 = new NormalLock();
            Boolean flag = true;
            //测试多线程下能否运行
            Task task1 = Task.Run(() =>
            {
                for (Int32 i = 0; i < 1000000; i++)
                    UnOrderLock1(checker, normalLock1,normalLock2, "Thread1");
            });
            Task task2 = Task.Run(() =>
            {
                for (Int32 i = 0; i < 1000000; i++)
                    UnOrderLock1(checker, normalLock2,normalLock1, "Thread2");
            });
            await Task.WhenAll(task1, task2);
            Console.WriteLine("end");
        }
    }
}