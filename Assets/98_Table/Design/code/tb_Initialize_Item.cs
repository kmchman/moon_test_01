/////////////////////////////////////////
// Export To ABSW_InitializeData.xlsm
// Last Update : 2019-07-12:18:44:33
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Initialize_Item
    {
        public short ID { get; protected set; }
        public int Item_Amount { get; protected set; }


        public static Dictionary<short, tb_Initialize_Item> map = new Dictionary<short, tb_Initialize_Item>();
        public static List<tb_Initialize_Item> list = new List<tb_Initialize_Item>();
        public static tb_Initialize_Item first = null;

        protected tb_Initialize_Item() {}
        public tb_Initialize_Item(tb_Initialize_Item from)
        {
            this.ID = from.ID;
            this.Item_Amount = from.Item_Amount;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Initialize_Item_internal
        {
            public short ID { get; set; }
            public int Item_Amount { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt16();
                Item_Amount = reader.ReadInt32();
            }
        }

        private tb_Initialize_Item(tb_Initialize_Item_internal from)
        {
            this.ID = from.ID;
            this.Item_Amount = from.Item_Amount;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Initialize_Item_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Initialize_Item_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Initialize_Item info = new tb_Initialize_Item(one);
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
                tb_Initialize_Item_internal data = new tb_Initialize_Item_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Initialize_Item info = new tb_Initialize_Item(data);
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

        public static tb_Initialize_Item Clone(tb_Initialize_Item from)
        {
            return new tb_Initialize_Item(from);
        }
    }
}
