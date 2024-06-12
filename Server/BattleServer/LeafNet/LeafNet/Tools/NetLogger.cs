using LeafNet.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeafNet
{
    public class NetLogger : SingletonBase<NetLogger>
    {
        public void Log(string message)
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
            UnityEngine.Debug.Log(message);
#else
            Console.WriteLine(message);
#endif
        }
    }
}
