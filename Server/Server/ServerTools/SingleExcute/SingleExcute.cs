using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerTools.SingleExcute
{
    /// <summary>
    /// 线程方法
    /// </summary>
    public delegate void ExcuteDelegate();
    public delegate void ExcuteDelegate<T>(T parms1);
    public delegate void ExcuteDelegate<T,A>(T parms1,A parms2);
    public delegate void ExcuteDelegate<T,A,B>(T parms1, A parms2,B parms3);
    public delegate void ExcuteDelegate<T,A,B,C>(T parms1,A parms2,B parms3,C parms4);
    /// <summary>
    /// 单线程池
    /// </summary>
    public class SingleExcute
    {
        /// <summary>
        /// 互斥锁
        /// </summary>
        public Mutex Mutex;

        public SingleExcute()
        {
            Mutex = new Mutex();
        }

        /// <summary>
        /// 单线程执行处理
        /// </summary>
        public void Excute(ExcuteDelegate excute)
        {
            lock (this)
            {
                Mutex.WaitOne();
                excute();
                Mutex.ReleaseMutex();
            }
        }
        public void Excute<T>(ExcuteDelegate<T> excute,T t)
        {
            lock (this)
            {
                Mutex.WaitOne();
                excute(t);
                Mutex.ReleaseMutex();
            }
        }
        public void Excute<T,A>(ExcuteDelegate<T,A> excute,T t,A a)
        {
            lock (this)
            {
                Mutex.WaitOne();
                excute(t,a);
                Mutex.ReleaseMutex();
            }
        }
        public void Excute<T,A,B>(ExcuteDelegate<T,A,B> excute,T t,A a,B b)
        {
            lock (this)
            {
                Mutex.WaitOne();
                excute(t,a,b);
                Mutex.ReleaseMutex();
            }
        }
        public void Excute<T,A,B,C>(ExcuteDelegate<T,A,B,C> excute,T t,A a,B b,C c)
        {
            lock (this)
            {
                Mutex.WaitOne();
                excute(t,a,b,c);
                Mutex.ReleaseMutex();
            }
        }
    }
}
