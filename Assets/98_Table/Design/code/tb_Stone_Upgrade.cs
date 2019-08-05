/////////////////////////////////////////
// Export To ABSW_StoneData.xlsm
// Last Update : 2019-04-29:14:42:20
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Stone_Upgrade
    {
        public short ID { get; protected set; }
        public short Unlock_Level { get; protected set; }


        public static Dictionary<short, tb_Stone_Upgrade> map = new Dictionary<short, tb_Stone_Upgrade>();
        public static List<tb_Stone_Upgrade> list = new List<tb_Stone_Upgrade>();
        public static tb_Stone_Upgrade first = null;

        protected tb_Stone_Upgrade() {}
        public tb_Stone_Upgrade(tb_Stone_Upgrade from)
        {
            this.ID = from.ID;
            this.Unlock_Level = from.Unlock_Level;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Stone_Upgrade_internal
        {
            public short ID { get; set; }
            public short Unlock_Level { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt16();
                Unlock_Level = reader.ReadInt16();
            }
        }

        private tb_Stone_Upgrade(tb_Stone_Upgrade_internal from)
        {
            this.ID = from.ID;
            this.Unlock_Level = from.Unlock_Level;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Stone_Upgrade_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Stone_Upgrade_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Stone_Upgrade info = new tb_Stone_Upgrade(one);
                list.Add(info);
                map.Add(info.ID, info);
            }
            first = list.Count > 0 ? list[0] : null;
        }

        public static void LoadFromJsonFile(string path)
        {
            StreamReader streamReader = new StreamReader(path);
            Load(streamReader.ReadToEnd());
            streamReader.Close();
        }

        public static void LoadBinary(byte[] bin)
        {
            MemoryStream stream = new MemoryStream(bin);
            LoadFromSteam(stream);
            stream.Close();
        }

        public static void LoadFromBinaryFile(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            LoadFromSteam(stream);
            stream.Close();
        }

        static void LoadFromSteam(Stream stream)
        {
            Clear();

            using (BinaryReader reader = new BinaryReader(stream))
            {
                tb_Stone_Upgrade_internal data = new tb_Stone_Upgrade_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Stone_Upgrade info = new tb_Stone_Upgrade(data);
                    list.Add(info);
                    map.Add(info.ID, info);
                }
                first = list.Count > 0 ? list[0] : null;
            }
        }

        public static void Clear()
        {
            map.Clear();
            list.Clear();
            first = null;
        }

        public static tb_Stone_Upgrade Clone(tb_Stone_Upgrade from)
        {
            return new tb_Stone_Upgrade(from);
        }
    }
}
