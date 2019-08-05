/////////////////////////////////////////
// Export To ABSW_FishData.xlsm
// Last Update : 2019-07-15:12:27:45
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Fish_Price
    {
        public short ID { get; protected set; }
        public string Input_ItemIDs { get; protected set; }
        public string Input_Counts { get; protected set; }


        public static Dictionary<short, tb_Fish_Price> map = new Dictionary<short, tb_Fish_Price>();
        public static List<tb_Fish_Price> list = new List<tb_Fish_Price>();
        public static tb_Fish_Price first = null;

        protected tb_Fish_Price() {}
        public tb_Fish_Price(tb_Fish_Price from)
        {
            this.ID = from.ID;
            this.Input_ItemIDs = from.Input_ItemIDs;
            this.Input_Counts = from.Input_Counts;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Fish_Price_internal
        {
            public short ID { get; set; }
            public string Input_ItemIDs { get; set; }
            public string Input_Counts { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt16();
                Input_ItemIDs = reader.ReadString();
                Input_Counts = reader.ReadString();
            }
        }

        private tb_Fish_Price(tb_Fish_Price_internal from)
        {
            this.ID = from.ID;
            this.Input_ItemIDs = from.Input_ItemIDs;
            this.Input_Counts = from.Input_Counts;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Fish_Price_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Fish_Price_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Fish_Price info = new tb_Fish_Price(one);
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
                tb_Fish_Price_internal data = new tb_Fish_Price_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Fish_Price info = new tb_Fish_Price(data);
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

        public static tb_Fish_Price Clone(tb_Fish_Price from)
        {
            return new tb_Fish_Price(from);
        }
    }
}
