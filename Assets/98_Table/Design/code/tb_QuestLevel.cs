/////////////////////////////////////////
// Export To ABSW_QuestLevelData.xlsm
// Last Update : 2019-07-19:14:38:26
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_QuestLevel
    {
        public short ID { get; protected set; }
        public int QuestType01_MaxNum { get; protected set; }
        public int QuestType01_Condition_Max { get; protected set; }
        public int QuestType01_Condition_Ratio { get; protected set; }
        public int QuesType01_Cooltime { get; protected set; }
        public int QuestType02_SlotBox_Min { get; protected set; }
        public int QuestType02_SlotBox_Max { get; protected set; }
        public int QuestType02_Condition_Min { get; protected set; }
        public int QuestType02_Condition_Max { get; protected set; }
        public int QuestType02_Condition_Ratio { get; protected set; }
        public int QuesType02_Cooltime_Min { get; protected set; }
        public int QuesType02_Cooltime_Max { get; protected set; }


        public static Dictionary<short, tb_QuestLevel> map = new Dictionary<short, tb_QuestLevel>();
        public static List<tb_QuestLevel> list = new List<tb_QuestLevel>();
        public static tb_QuestLevel first = null;

        protected tb_QuestLevel() {}
        public tb_QuestLevel(tb_QuestLevel from)
        {
            this.ID = from.ID;
            this.QuestType01_MaxNum = from.QuestType01_MaxNum;
            this.QuestType01_Condition_Max = from.QuestType01_Condition_Max;
            this.QuestType01_Condition_Ratio = from.QuestType01_Condition_Ratio;
            this.QuesType01_Cooltime = from.QuesType01_Cooltime;
            this.QuestType02_SlotBox_Min = from.QuestType02_SlotBox_Min;
            this.QuestType02_SlotBox_Max = from.QuestType02_SlotBox_Max;
            this.QuestType02_Condition_Min = from.QuestType02_Condition_Min;
            this.QuestType02_Condition_Max = from.QuestType02_Condition_Max;
            this.QuestType02_Condition_Ratio = from.QuestType02_Condition_Ratio;
            this.QuesType02_Cooltime_Min = from.QuesType02_Cooltime_Min;
            this.QuesType02_Cooltime_Max = from.QuesType02_Cooltime_Max;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_QuestLevel_internal
        {
            public short ID { get; set; }
            public int QuestType01_MaxNum { get; set; }
            public int QuestType01_Condition_Max { get; set; }
            public int QuestType01_Condition_Ratio { get; set; }
            public int QuesType01_Cooltime { get; set; }
            public int QuestType02_SlotBox_Min { get; set; }
            public int QuestType02_SlotBox_Max { get; set; }
            public int QuestType02_Condition_Min { get; set; }
            public int QuestType02_Condition_Max { get; set; }
            public int QuestType02_Condition_Ratio { get; set; }
            public int QuesType02_Cooltime_Min { get; set; }
            public int QuesType02_Cooltime_Max { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt16();
                QuestType01_MaxNum = reader.ReadInt32();
                QuestType01_Condition_Max = reader.ReadInt32();
                QuestType01_Condition_Ratio = reader.ReadInt32();
                QuesType01_Cooltime = reader.ReadInt32();
                QuestType02_SlotBox_Min = reader.ReadInt32();
                QuestType02_SlotBox_Max = reader.ReadInt32();
                QuestType02_Condition_Min = reader.ReadInt32();
                QuestType02_Condition_Max = reader.ReadInt32();
                QuestType02_Condition_Ratio = reader.ReadInt32();
                QuesType02_Cooltime_Min = reader.ReadInt32();
                QuesType02_Cooltime_Max = reader.ReadInt32();
            }
        }

        private tb_QuestLevel(tb_QuestLevel_internal from)
        {
            this.ID = from.ID;
            this.QuestType01_MaxNum = from.QuestType01_MaxNum;
            this.QuestType01_Condition_Max = from.QuestType01_Condition_Max;
            this.QuestType01_Condition_Ratio = from.QuestType01_Condition_Ratio;
            this.QuesType01_Cooltime = from.QuesType01_Cooltime;
            this.QuestType02_SlotBox_Min = from.QuestType02_SlotBox_Min;
            this.QuestType02_SlotBox_Max = from.QuestType02_SlotBox_Max;
            this.QuestType02_Condition_Min = from.QuestType02_Condition_Min;
            this.QuestType02_Condition_Max = from.QuestType02_Condition_Max;
            this.QuestType02_Condition_Ratio = from.QuestType02_Condition_Ratio;
            this.QuesType02_Cooltime_Min = from.QuesType02_Cooltime_Min;
            this.QuesType02_Cooltime_Max = from.QuesType02_Cooltime_Max;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_QuestLevel_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_QuestLevel_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_QuestLevel info = new tb_QuestLevel(one);
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
                tb_QuestLevel_internal data = new tb_QuestLevel_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_QuestLevel info = new tb_QuestLevel(data);
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

        public static tb_QuestLevel Clone(tb_QuestLevel from)
        {
            return new tb_QuestLevel(from);
        }
    }
}
