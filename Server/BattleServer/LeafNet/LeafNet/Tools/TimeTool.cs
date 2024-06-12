using LeafNet.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeafNet
{
    public class TimeTool : SingletonBase<TimeTool>
    {
        /// <summary>
        /// 获取时间戳,精确到毫秒
        /// </summary>
        /// <returns></returns>
        public long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
    }
}
