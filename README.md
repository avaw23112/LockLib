# LockLib
- 可以检测死锁的轻量级锁库，包含读写锁，智能锁，普通锁三种可选类型
- 分Debug和Release两种模式，可直接通过IDE改变版本。Release模式下不提供死锁检测，但会辅助管理锁，比如依然可以用字符串自定义锁名。
- 检测出死锁时会直接Crash程序，并且将导致死锁的原因打印在日志里

***
- 2024-12-31 21:44:26 | Error | System.Exception:  出现循环等待!
- { 线程 thread 0 持有： [ lock2 ] }  = 预备 > lock3,thread 0对lock3的等待导致饥饿
- { 线程 thread 1 持有： [ lock1 ] }  = 预备 > lock2,thread 1对lock2的等待导致饥饿
- { 线程 thread 2 持有： [ lock3 ] }  = 预备 > lock1,thread 2对lock1的等待导致饥饿

或者

- 2024-12-31 16:03:19 | Error | System.Exception: 出现顺序死锁
- { 线程 thread 1 持有： [ lock1 ] }  = 预备 > lock2
- { 线程 thread 0 持有： [ lock2 ] }  = 预备 > lock1
- thread 1与thread 0持锁顺序并不相容！

***

- 用LockController管理锁库

        private static void Lock(LockController lockController, string thread1)
        {
            using (lockController.AutoLock("锁名1"))
            {
                using (lockController.AutoLock("锁名2"))
                {
                
                }
            }
        }

        private static void Lock2(LockController lockController, string thread1)
        {
            using (lockController.AutoReadLock("锁名1"))
            {
                using (lockController.AutoReadLock("锁名2"))
                {
                
                }
            }
        }
        private static void Lock3(LockController lockController, string thread1)
        {
            using (lockController.AutoWriteLock("锁名1"))
            {
                using (lockController.AutoWriteLock("锁名2"))
                {
                
                }
            }
        }

  - 也可以用GlobalLock管理全局锁
    
        private static void UnOrderLock3(string thread1)
        {
            using (GlobalLock.AutoLock("测试锁1"))
            {
                using (GlobalLock.AutoLock("测试锁2"))
                {
                    
                }
            }
        }

- 自定义锁(同样可以管理写读锁)

        private static void UnOrderLock2( LockController lockController, string thread1)
        {
            lockController.Lock("normalLock1");
            Console.WriteLine(thread1 + " " + "normalLock1");
            lockController.Lock("normalLock2");
            Console.WriteLine(thread1 + "normalLock2");
            lockController.UnLock("normalLock2");
            lockController.UnLock("normalLock1");
        }

        private static void UnOrderLock4(string thread1)
        {
            GlobalLock.Lock("normalLock1");
            Console.WriteLine(thread1 + " " + "normalLock1");
            GlobalLock.Lock("normalLock2");
            Console.WriteLine(thread1 + "normalLock2");
            GlobalLock.UnLock("normalLock2");
            GlobalLock.UnLock("normalLock1");
        }
  
