/////////////////////////////////////////
// Export To ABSW_QuestDialogueData.xlsm
// Last Update : 2019-03-20:10:33:29
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_QuestDialogue
    {
        public int Family { get; protected set; }
        public string Dialogue_Key_01 { get; protected set; }
        public string Dialogue_Key_02 { get; protected set; }
        public string Dialogue_Key_03 { get; protected set; }
        public string Dialogue_Key_04 { get; protected set; }
        public string Dialogue_Key_05 { get; protected set; }
        public string Dialogue_Key_06 { get; protected set; }
        public string Dialogue_Key_07 { get; protected set; }
        public string Dialogue_Key_08 { get; protected set; }
        public string Dialogue_Key_09 { get; protected set; }
        public string Dialogue_Key_10 { get; protected set; }


        public static Dictionary<int, tb_QuestDialogue> map = new Dictionary<int, tb_QuestDialogue>();
        public static List<tb_QuestDialogue> list = new List<tb_QuestDialogue>();
        public static tb_QuestDialogue first = null;

        protected tb_QuestDialogue() {}
        public tb_QuestDialogue(tb_QuestDialogue from)
        {
            this.Family = from.Family;
            this.Dialogue_Key_01 = from.Dialogue_Key_01;
            this.Dialogue_Key_02 = from.Dialogue_Key_02;
            this.Dialogue_Key_03 = from.Dialogue_Key_03;
            this.Dialogue_Key_04 = from.Dialogue_Key_04;
            this.Dialogue_Key_05 = from.Dialogue_Key_05;
            this.Dialogue_Key_06 = from.Dialogue_Key_06;
            this.Dialogue_Key_07 = from.Dialogue_Key_07;
            this.Dialogue_Key_08 = from.Dialogue_Key_08;
            this.Dialogue_Key_09 = from.Dialogue_Key_09;
            this.Dialogue_Key_10 = from.Dialogue_Key_10;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_QuestDialogue_internal
        {
            public int Family { get; set; }
            public string Dialogue_Key_01 { get; set; }
            public string Dialogue_Key_02 { get; set; }
            public string Dialogue_Key_03 { get; set; }
            public string Dialogue_Key_04 { get; set; }
            public string Dialogue_Key_05 { get; set; }
            public string Dialogue_Key_06 { get; set; }
            public string Dialogue_Key_07 { get; set; }
            public string Dialogue_Key_08 { get; set; }
            public string Dialogue_Key_09 { get; set; }
            public string Dialogue_Key_10 { get; set; }

            public void Read(BinaryReader reader)
            {
                Family = reader.ReadInt32();
                Dialogue_Key_01 = reader.ReadString();
                Dialogue_Key_02 = reader.ReadString();
                Dialogue_Key_03 = reader.ReadString();
                Dialogue_Key_04 = reader.ReadString();
                Dialogue_Key_05 = reader.ReadString();
                Dialogue_Key_06 = reader.ReadString();
                Dialogue_Key_07 = reader.ReadString();
                Dialogue_Key_08 = reader.ReadString();
                Dialogue_Key_09 = reader.ReadString();
                Dialogue_Key_10 = reader.ReadString();
            }
        }

        private tb_QuestDialogue(tb_QuestDialogue_internal from)
        {
            this.Family = from.Family;
            this.Dialogue_Key_01 = from.Dialogue_Key_01;
            this.Dialogue_Key_02 = from.Dialogue_Key_02;
            this.Dialogue_Key_03 = from.Dialogue_Key_03;
            this.Dialogue_Key_04 = from.Dialogue_Key_04;
            this.Dialogue_Key_05 = from.Dialogue_Key_05;
            this.Dialogue_Key_06 = from.Dialogue_Key_06;
            this.Dialogue_Key_07 = from.Dialogue_Key_07;
            this.Dialogue_Key_08 = from.Dialogue_Key_08;
            this.Dialogue_Key_09 = from.Dialogue_Key_09;
            this.Dialogue_Key_10 = from.Dialogue_Key_10;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_QuestDialogue_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_QuestDialogue_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_QuestDialogue info = new tb_QuestDialogue(one);
                list.Add(info);
                map.Add(info.Family, info);
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
                tb_QuestDialogue_internal data = new tb_QuestDialogue_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_QuestDialogue info = new tb_QuestDialogue(data);
                    list.Add(info);
                    map.Add(info.Family, info);
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

        public static tb_QuestDialogue Clone(tb_QuestDialogue from)
        {
            return new tb_QuestDialogue(from);
        }
    }
}
