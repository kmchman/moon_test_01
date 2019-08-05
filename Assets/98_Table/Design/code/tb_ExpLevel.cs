/////////////////////////////////////////
// Export To ABSW_ExpData.xlsm
// Last Update : 2019-07-18:15:04:14
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_ExpLevel
    {
        public short Level { get; protected set; }
        public int Xp { get; protected set; }
        public string GiftRatio { get; protected set; }
        public string Reward_Cash { get; protected set; }
        public string Reward_Life { get; protected set; }
        public string Reward_ItemIDs { get; protected set; }
        public string Reward_Counts { get; protected set; }


        public static Dictionary<short, tb_ExpLevel> map = new Dictionary<short, tb_ExpLevel>();
        public static List<tb_ExpLevel> list = new List<tb_ExpLevel>();
        public static tb_ExpLevel first = null;

        protected tb_ExpLevel() {}
        public tb_ExpLevel(tb_ExpLevel from)
        {
            this.Level = from.Level;
            this.Xp = from.Xp;
            this.GiftRatio = from.GiftRatio;
            this.Reward_Cash = from.Reward_Cash;
            this.Reward_Life = from.Reward_Life;
            this.Reward_ItemIDs = from.Reward_ItemIDs;
            this.Reward_Counts = from.Reward_Counts;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_ExpLevel_internal
        {
            public short Level { get; set; }
            public int Xp { get; set; }
            public string GiftRatio { get; set; }
            public string Reward_Cash { get; set; }
            public string Reward_Life { get; set; }
            public string Reward_ItemIDs { get; set; }
            public string Reward_Counts { get; set; }

            public void Read(BinaryReader reader)
            {
                Level = reader.ReadInt16();
                Xp = reader.ReadInt32();
                GiftRatio = reader.ReadString();
                Reward_Cash = reader.ReadString();
                Reward_Life = reader.ReadString();
                Reward_ItemIDs = reader.ReadString();
                Reward_Counts = reader.ReadString();
            }
        }

        private tb_ExpLevel(tb_ExpLevel_internal from)
        {
            this.Level = from.Level;
            this.Xp = from.Xp;
            this.GiftRatio = from.GiftRatio;
            this.Reward_Cash = from.Reward_Cash;
            this.Reward_Life = from.Reward_Life;
            this.Reward_ItemIDs = from.Reward_ItemIDs;
            this.Reward_Counts = from.Reward_Counts;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_ExpLevel_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_ExpLevel_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_ExpLevel info = new tb_ExpLevel(one);
                list.Add(info);
                map.Add(info.Level, info);
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
                tb_ExpLevel_internal data = new tb_ExpLevel_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_ExpLevel info = new tb_ExpLevel(data);
                    list.Add(info);
                    map.Add(info.Level, info);
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

        public static tb_ExpLevel Clone(tb_ExpLevel from)
        {
            return new tb_ExpLevel(from);
        }
    }
}
