/////////////////////////////////////////
// Export To ABSW_BuildingData.xlsm
// Last Update : 2019-07-19:14:46:25
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Building_Price
    {
        public int ID { get; protected set; }
        public short BuildingID { get; protected set; }
        public string Name { get; protected set; }
        public int Have_Count { get; protected set; }
        public short Level { get; protected set; }
        public int Population { get; protected set; }
        public string Input_ItemIDs { get; protected set; }
        public string Input_Counts { get; protected set; }
        public string Active_Input_ItemIDs { get; protected set; }
        public string Active_Input_Counts { get; protected set; }
        public int Acquire_XP { get; protected set; }
        public int Build_Time { get; protected set; }


        public static Dictionary<int, tb_Building_Price> map = new Dictionary<int, tb_Building_Price>();
        public static List<tb_Building_Price> list = new List<tb_Building_Price>();
        public static tb_Building_Price first = null;

        protected tb_Building_Price() {}
        public tb_Building_Price(tb_Building_Price from)
        {
            this.ID = from.ID;
            this.BuildingID = from.BuildingID;
            this.Name = from.Name;
            this.Have_Count = from.Have_Count;
            this.Level = from.Level;
            this.Population = from.Population;
            this.Input_ItemIDs = from.Input_ItemIDs;
            this.Input_Counts = from.Input_Counts;
            this.Active_Input_ItemIDs = from.Active_Input_ItemIDs;
            this.Active_Input_Counts = from.Active_Input_Counts;
            this.Acquire_XP = from.Acquire_XP;
            this.Build_Time = from.Build_Time;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Building_Price_internal
        {
            public int ID { get; set; }
            public short BuildingID { get; set; }
            public string Name { get; set; }
            public int Have_Count { get; set; }
            public short Level { get; set; }
            public int Population { get; set; }
            public string Input_ItemIDs { get; set; }
            public string Input_Counts { get; set; }
            public string Active_Input_ItemIDs { get; set; }
            public string Active_Input_Counts { get; set; }
            public int Acquire_XP { get; set; }
            public int Build_Time { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt32();
                BuildingID = reader.ReadInt16();
                Name = reader.ReadString();
                Have_Count = reader.ReadInt32();
                Level = reader.ReadInt16();
                Population = reader.ReadInt32();
                Input_ItemIDs = reader.ReadString();
                Input_Counts = reader.ReadString();
                Active_Input_ItemIDs = reader.ReadString();
                Active_Input_Counts = reader.ReadString();
                Acquire_XP = reader.ReadInt32();
                Build_Time = reader.ReadInt32();
            }
        }

        private tb_Building_Price(tb_Building_Price_internal from)
        {
            this.ID = from.ID;
            this.BuildingID = from.BuildingID;
            this.Name = from.Name;
            this.Have_Count = from.Have_Count;
            this.Level = from.Level;
            this.Population = from.Population;
            this.Input_ItemIDs = from.Input_ItemIDs;
            this.Input_Counts = from.Input_Counts;
            this.Active_Input_ItemIDs = from.Active_Input_ItemIDs;
            this.Active_Input_Counts = from.Active_Input_Counts;
            this.Acquire_XP = from.Acquire_XP;
            this.Build_Time = from.Build_Time;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Building_Price_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Building_Price_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Building_Price info = new tb_Building_Price(one);
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
                tb_Building_Price_internal data = new tb_Building_Price_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Building_Price info = new tb_Building_Price(data);
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

        public static tb_Building_Price Clone(tb_Building_Price from)
        {
            return new tb_Building_Price(from);
        }
    }
}
