using LeafNet.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeafNet.LeafNet
{
    public class TimerTask : IComparable<TimerTask>
    {
        public bool loop;
        public long waitTime;
        public Action<NetworkEntity> callback;

        public int CompareTo(TimerTask other)
        {
            if (loop == other.loop && waitTime == other.waitTime && callback == other.callback)
            {
                return 0;
            }

            if (waitTime < other.waitTime)
            {
                return -1;
            }

            return 1;
        }
    }

    public class ServerTimer : SingletonBase<ServerTimer>
    {
        private SortedSet<(long, TimerTask)> tasks = new SortedSet<(long, TimerTask)>();

        public void Tick(NetworkEntity networkEntity)
        {
            long currentTime = TimeTool.GetInstance().GetTimeStamp();
            while (tasks.Count > 0 && tasks.First().Item1 <= currentTime)
            {
                TimerTask task = tasks.First().Item2;
                tasks.Remove(tasks.First());

                task.callback?.Invoke(networkEntity);

                if (task.loop)
                {
                    AddTask(task);
                }
            }
        }

        public void AddTask(TimerTask task)
        {
            long currentTime = TimeTool.GetInstance().GetTimeStamp();
            tasks.Add((currentTime + task.waitTime, task));
        }

        public void RemoveTask(TimerTask task)
        {
            foreach (var pair in tasks)
            {
                if (pair.Item2 == task)
                {
                    tasks.Remove(pair);
                    break;
                }
            }
        }
    }
}
