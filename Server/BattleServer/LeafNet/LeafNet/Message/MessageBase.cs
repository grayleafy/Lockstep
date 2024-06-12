using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LeafNet
{
    class AllowAllAssemblyVersionsDeserializationBinder : System.Runtime.Serialization.SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            String currentAssembly = System.Reflection.Assembly.GetExecutingAssembly().FullName;
            typeName = typeName.Replace("oldnamespace", "newnamespace");          //修改以替换命名空间
            assemblyName = currentAssembly;

            typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
                typeName, assemblyName));
            return typeToDeserialize;
        }
    }




    /// <summary>
    /// 消息基类，子类需要标记为可序列化
    /// </summary>
    [Serializable]
    public abstract class MessageBase
    {


        /// <summary>
        /// 序列化消息并返回字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
                formatter.Serialize(memoryStream, this);
                //memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 序列化消息并返回字节数组,包括长度消息头
        /// </summary>
        /// <returns></returns>
        public byte[] SerializeWithHead()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(new byte[2] { 0, 0 }, 0, 2);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
                formatter.Serialize(memoryStream, this);
                //memoryStream.Seek(0, SeekOrigin.Begin);
                uint length = (uint)memoryStream.Length;
                memoryStream.Position = 0;
                memoryStream.WriteByte((byte)(length >> 4));
                memoryStream.WriteByte((byte)(length & 0x0f));
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 从字节数组中反序列化消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MessageBase Deserialize(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
                return formatter.Deserialize(memoryStream) as MessageBase;
            }
        }

        /// <summary>
        /// 从字节数组中反序列化消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MessageBase Deserialize(byte[] data, int offset, int count)
        {
            using (MemoryStream memoryStream = new MemoryStream(data, offset, count))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
                return formatter.Deserialize(memoryStream) as MessageBase;
            }
        }



        /// <summary>
        /// 序列化并且添加到byteArray中
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public void SerializeToByteArray(ByteArray byteArray)
        {
            var body = Serialize();
            //计算长度
            uint length = (uint)body.Length + 2;
            //大端法
            var head = new byte[2];
            head[0] = (byte)(length >> 4);
            head[1] = (byte)(length & 0x0f);
            byteArray.Write(head, 0, 2);
            byteArray.Write(body, 0, body.Length);
        }

        /// <summary>
        /// 尝试从字节数组中反序列化一个消息，如果字节数组长度不足则返回null
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static MessageBase DeserializeFromByteArray(ByteArray byteArray)
        {
            if (byteArray.length <= 2)
            {
                return null;
            }

            uint length = ((uint)byteArray.bytes[byteArray.readIdx] << 4) + ((uint)byteArray.bytes[byteArray.readIdx + 1]);
            if (byteArray.length < length)
            {
                return null;
            }
            byteArray.readIdx += 2;

            //byte[] bytes = new byte[length - 2];
            //byteArray.Read(bytes, 0, (int)length - 2);
            //MessageBase msg =  Deserialize(bytes);
            MessageBase msg = Deserialize(byteArray.bytes, byteArray.readIdx, (int)length - 2);
            byteArray.readIdx += (int)length - 2;

            return msg;
        }

        /// <summary>
        /// 接受消息后的处理过程
        /// </summary>
        /// <param name="senderSocket"></param>
        /// <param name="networkEntity"></param>
        public void HandleProcess(Session senderSession, NetworkEntity networkEntity)
        {
            NetEventCenter.GetInstance().EventTrigger<(MessageBase msg, Session senderSession, NetworkEntity networkEntity)>(this.GetType().Name, (this, senderSession, networkEntity));
        }



    }
}
