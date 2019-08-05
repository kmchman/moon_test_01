/////////////////////////////////////////
// Export To ABSW_MapData.xlsm
// Last Update : 2019-06-28:14:18:58
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Map_Price
    {
        public int ID { get; protected set; }
        public int Population { get; protected set; }
        public int InputLife_Count { get; protected set; }
        public int Input01_ItemID { get; protected set; }
        public int Input01_Count { get; protected set; }
        public int Input02_ItemID { get; protected set; }
        public int Input02_Count { get; protected set; }
        public int Input03_ItemID { get; protected set; }
        public int Input03_Count { get; protected set; }
        public int AreaExtend_Time { get; protected set; }
        public int Acquire_XP { get; protected set; }


        public static Dictionary<int, tb_Map_Price> map = new Dictionary<int, tb_Map_Price>();
        public static List<tb_Map_Price> list = new List<tb_Map_Price>();
        public static tb_Map_Price first = null;

        protected tb_Map_Price() {}
        public tb_Map_Price(tb_Map_Price from)
        {
            this.ID = from.ID;
            this.Population = from.Population;
            this.InputLife_Count = from.InputLife_Count;
            this.Input01_ItemID = from.Input01_ItemID;
            this.Input01_Count = from.Input01_Count;
            this.Input02_ItemID = from.Input02_ItemID;
            this.Input02_Count = from.Input02_Count;
            this.Input03_ItemID = from.Input03_ItemID;
            this.Input03_Count = from.Input03_Count;
            this.AreaExtend_Time = from.AreaExtend_Time;
            this.Acquire_XP = from.Acquire_XP;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Map_Price_internal
        {
            public int ID { get; set; }
            public int Population { get; set; }
            public int InputLife_Count { get; set; }
            public int Input01_ItemID { get; set; }
            public int Input01_Count { get; set; }
            public int Input02_ItemID { get; set; }
            public int Input02_Count { get; set; }
            public int Input03_ItemID { get; set; }
            public int Input03_Count { get; set; }
            public int AreaExtend_Time { get; set; }
            public int Acquire_XP { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt32();
                Population = reader.ReadInt32();
                InputLife_Count = reader.ReadInt32();
                Input01_ItemID = reader.ReadInt32();
                Input01_Count = reader.ReadInt32();
                Input02_ItemID = reader.ReadInt32();
                Input02_Count = reader.ReadInt32();
                Input03_ItemID = reader.ReadInt32();
                Input03_Count = reader.ReadInt32();
                AreaExtend_Time = reader.ReadInt32();
                Acquire_XP = reader.ReadInt32();
            }
        }

        private tb_Map_Price(tb_Map_Price_internal from)
        {
            this.ID = from.ID;
            this.Population = from.Population;
            this.InputLife_Count = from.InputLife_Count;
            this.Input01_ItemID = from.Input01_ItemID;
            this.Input01_Count = from.Input01_Count;
            this.Input02_ItemID = from.Input02_ItemID;
            this.Input02_Count = from.Input02_Count;
            this.Input03_ItemID = from.Input03_ItemID;
            this.Input03_Count = from.Input03_Count;
            this.AreaExtend_Time = from.AreaExtend_Time;
            this.Acquire_XP = from.Acquire_XP;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Map_Price_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Map_Price_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Map_Price info = new tb_Map_Price(one);
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
                tb_Map_Price_internal data = new tb_Map_Price_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Map_Price info = new tb_Map_Price(data);
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

        public static tb_Map_Price Clone(tb_Map_Price from)
        {
            return new tb_Map_Price(from);
        }
    }
}
