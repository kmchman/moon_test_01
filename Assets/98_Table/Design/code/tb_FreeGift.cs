/////////////////////////////////////////
// Export To ABSW_FreeGiftData.xlsm
// Last Update : 2019-05-31:17:26:46
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_FreeGift
    {
        public short ID { get; protected set; }
        public eItemType ItemType_01 { get; protected set; }
        public int Probability_01 { get; protected set; }
        public int MinCount_01 { get; protected set; }
        public int MaxCount_01 { get; protected set; }
        public eItemType ItemType_02 { get; protected set; }
        public int Probability_02 { get; protected set; }
        public int MaxCount_02 { get; protected set; }
        public eItemType ItemType_03 { get; protected set; }
        public int Probability_03 { get; protected set; }
        public int MaxCount_03 { get; protected set; }
        public eItemType ItemType_04 { get; protected set; }
        public int Probability_04 { get; protected set; }
        public int MaxCount_04 { get; protected set; }
        public eItemType ItemType_05 { get; protected set; }
        public int Probability_05 { get; protected set; }
        public int MaxCount_05 { get; protected set; }
        public eItemType ItemType_06 { get; protected set; }
        public int Probability_06 { get; protected set; }
        public int MaxCount_06 { get; protected set; }


        public static Dictionary<short, tb_FreeGift> map = new Dictionary<short, tb_FreeGift>();
        public static List<tb_FreeGift> list = new List<tb_FreeGift>();
        public static tb_FreeGift first = null;

        protected tb_FreeGift() {}
        public tb_FreeGift(tb_FreeGift from)
        {
            this.ID = from.ID;
            this.ItemType_01 = from.ItemType_01;
            this.Probability_01 = from.Probability_01;
            this.MinCount_01 = from.MinCount_01;
            this.MaxCount_01 = from.MaxCount_01;
            this.ItemType_02 = from.ItemType_02;
            this.Probability_02 = from.Probability_02;
            this.MaxCount_02 = from.MaxCount_02;
            this.ItemType_03 = from.ItemType_03;
            this.Probability_03 = from.Probability_03;
            this.MaxCount_03 = from.MaxCount_03;
            this.ItemType_04 = from.ItemType_04;
            this.Probability_04 = from.Probability_04;
            this.MaxCount_04 = from.MaxCount_04;
            this.ItemType_05 = from.ItemType_05;
            this.Probability_05 = from.Probability_05;
            this.MaxCount_05 = from.MaxCount_05;
            this.ItemType_06 = from.ItemType_06;
            this.Probability_06 = from.Probability_06;
            this.MaxCount_06 = from.MaxCount_06;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_FreeGift_internal
        {
            public short ID { get; set; }
            public eItemType ItemType_01 { get; set; }
            public int Probability_01 { get; set; }
            public int MinCount_01 { get; set; }
            public int MaxCount_01 { get; set; }
            public eItemType ItemType_02 { get; set; }
            public int Probability_02 { get; set; }
            public int MaxCount_02 { get; set; }
            public eItemType ItemType_03 { get; set; }
            public int Probability_03 { get; set; }
            public int MaxCount_03 { get; set; }
            public eItemType ItemType_04 { get; set; }
            public int Probability_04 { get; set; }
            public int MaxCount_04 { get; set; }
            public eItemType ItemType_05 { get; set; }
            public int Probability_05 { get; set; }
            public int MaxCount_05 { get; set; }
            public eItemType ItemType_06 { get; set; }
            public int Probability_06 { get; set; }
            public int MaxCount_06 { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt16();
                ItemType_01 = (eItemType)Enum.Parse(typeof(eItemType), reader.ReadString());
                Probability_01 = reader.ReadInt32();
                MinCount_01 = reader.ReadInt32();
                MaxCount_01 = reader.ReadInt32();
                ItemType_02 = (eItemType)Enum.Parse(typeof(eItemType), reader.ReadString());
                Probability_02 = reader.ReadInt32();
                MaxCount_02 = reader.ReadInt32();
                ItemType_03 = (eItemType)Enum.Parse(typeof(eItemType), reader.ReadString());
                Probability_03 = reader.ReadInt32();
                MaxCount_03 = reader.ReadInt32();
                ItemType_04 = (eItemType)Enum.Parse(typeof(eItemType), reader.ReadString());
                Probability_04 = reader.ReadInt32();
                MaxCount_04 = reader.ReadInt32();
                ItemType_05 = (eItemType)Enum.Parse(typeof(eItemType), reader.ReadString());
                Probability_05 = reader.ReadInt32();
                MaxCount_05 = reader.ReadInt32();
                ItemType_06 = (eItemType)Enum.Parse(typeof(eItemType), reader.ReadString());
                Probability_06 = reader.ReadInt32();
                MaxCount_06 = reader.ReadInt32();
            }
        }

        private tb_FreeGift(tb_FreeGift_internal from)
        {
            this.ID = from.ID;
            this.ItemType_01 = from.ItemType_01;
            this.Probability_01 = from.Probability_01;
            this.MinCount_01 = from.MinCount_01;
            this.MaxCount_01 = from.MaxCount_01;
            this.ItemType_02 = from.ItemType_02;
            this.Probability_02 = from.Probability_02;
            this.MaxCount_02 = from.MaxCount_02;
            this.ItemType_03 = from.ItemType_03;
            this.Probability_03 = from.Probability_03;
            this.MaxCount_03 = from.MaxCount_03;
            this.ItemType_04 = from.ItemType_04;
            this.Probability_04 = from.Probability_04;
            this.MaxCount_04 = from.MaxCount_04;
            this.ItemType_05 = from.ItemType_05;
            this.Probability_05 = from.Probability_05;
            this.MaxCount_05 = from.MaxCount_05;
            this.ItemType_06 = from.ItemType_06;
            this.Probability_06 = from.Probability_06;
            this.MaxCount_06 = from.MaxCount_06;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_FreeGift_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_FreeGift_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_FreeGift info = new tb_FreeGift(one);
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
                tb_FreeGift_internal data = new tb_FreeGift_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_FreeGift info = new tb_FreeGift(data);
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

        public static tb_FreeGift Clone(tb_FreeGift from)
        {
            return new tb_FreeGift(from);
        }
    }
}
