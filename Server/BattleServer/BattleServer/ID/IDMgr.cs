using LeafNet.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class IDMgr : SingletonBase<IDMgr>
{
    int maxId = 1000;
    Queue<string> freeids;

    public IDMgr()
    {
        freeids = new Queue<string>();
        for (int i = 0; i < maxId; i++)
        {
            freeids.Enqueue(i.ToString());
        }
    }

    public string GetFreeId()
    {
        if (freeids.Count == 0)
        {
            return null;
        }
        return freeids.Dequeue();
    }

    public void PushFreeId(string id)
    {
        freeids.Enqueue(id);
    }
}

