/////////////////////////////////////////
// Export To ABSW_StorageData.xlsm
// Last Update : 2019-05-15:18:40:40
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Storage
    {
        public short ID { get; protected set; }
        public int Storage_Capacity { get; protected set; }
        public string Extend_Input_ItemIDs { get; protected set; }
        public string Extend_Input_Counts { get; protected set; }


        public static Dictionary<short, tb_Storage> map = new Dictionary<short, tb_Storage>();
        public static List<tb_Storage> list = new List<tb_Storage>();
        public static tb_Storage first = null;

        protected tb_Storage() {}
        public tb_Storage(tb_Storage from)
        {
            this.ID = from.ID;
            this.Storage_Capacity = from.Storage_Capacity;
            this.Extend_Input_ItemIDs = from.Extend_Input_ItemIDs;
            this.Extend_Input_Counts = from.Extend_Input_Counts;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Storage_internal
        {
            public short ID { get; set; }
            public int Storage_Capacity { get; set; }
            public string Extend_Input_ItemIDs { get; set; }
            public string Extend_Input_Counts { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt16();
                Storage_Capacity = reader.ReadInt32();
                Extend_Input_ItemIDs = reader.ReadString();
                Extend_Input_Counts = reader.ReadString();
            }
        }

        private tb_Storage(tb_Storage_internal from)
        {
            this.ID = from.ID;
            this.Storage_Capacity = from.Storage_Capacity;
            this.Extend_Input_ItemIDs = from.Extend_Input_ItemIDs;
            this.Extend_Input_Counts = from.Extend_Input_Counts;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Storage_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Storage_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Storage info = new tb_Storage(one);
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
                tb_Storage_internal data = new tb_Storage_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Storage info = new tb_Storage(data);
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

        public static tb_Storage Clone(tb_Storage from)
        {
            return new tb_Storage(from);
        }
    }
}
