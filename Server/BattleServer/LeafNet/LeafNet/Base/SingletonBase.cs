using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeafNet.Base
{
    /// <summary>
    /// 单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonBase<T> where T : new()
    {
        private static T instance;
        private static object mtx = new object();
        protected SingletonBase() { }

        public static T GetInstance()
        {
            if (instance == null)
            {
                lock (mtx)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                }
            }
            return instance;
        }
    }
}
