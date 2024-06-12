using FixMath.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.NetVector
{
    [Serializable]
    public struct NetVector3
    {
        public Fix64 x;
        public Fix64 y;
        public Fix64 z;

        public static NetVector3 zero
        {
            get
            {
                return new NetVector3()
                {
                    x = Fix64.Zero,
                    y = Fix64.Zero,
                    z = Fix64.Zero
                };
            }
        }

        public static bool operator ==(NetVector3 left, NetVector3 right)
        {
            return left.x == right.x && left.y == right.y && left.z == right.z;
        }
        public static bool operator !=(NetVector3 left, NetVector3 right)
        {
            return !(left == right);
        }

    }

    [Serializable]
    public struct NetVector2
    {
        public Fix64 x;
        public Fix64 y;
    }
}
