using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.AspNetCore.Http;

namespace UnEInternal.Service
{
    public static class SessionManager
    {
        public static void SetData(ISession session, string strKey, object data)
        {
            if (data == null)
            {
                session.Remove(strKey);
            }
            else
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();

                bf.Serialize(stream, data);
                byte[] bytes = stream.ToArray();

                stream.Close();
                session.Set(strKey, bytes);
            }
        }

        public static bool TryGetData<DataType>(ISession session, string strKey, ref DataType data)
        {
            byte[] bytes;

            if (session.TryGetValue(strKey, out bytes))
            {
                MemoryStream stream = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();

                stream.Write(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                data = (DataType)bf.Deserialize(stream);

                stream.Close();
                return true;
            }

            return false;
        }

        public static bool HasData(ISession session, string strKey)
        {
            byte[] bytes;
            return session.TryGetValue(strKey, out bytes);
        }
    }
}
