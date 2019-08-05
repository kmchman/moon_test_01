/////////////////////////////////////////
// Export To ABSW_FishData.xlsm
// Last Update : 2019-07-15:12:27:45
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Fish
    {
        public int ID { get; protected set; }
        public string Local_String { get; protected set; }
        public eFishType FishType { get; protected set; }
        public eFishSizeType Fish_Size_Type { get; protected set; }
        public int Unlock_Level { get; protected set; }
        public int Family { get; protected set; }
        public int Create_Reward_ItemIDs { get; protected set; }
        public int Create_Reward_Counts { get; protected set; }
        public byte Breed_Able { get; protected set; }
        public int InputLife_Count { get; protected set; }
        public short Breed_Rate_Up_ItemID { get; protected set; }
        public int Breed_Rate_Up_Count { get; protected set; }
        public int Breed_Rate { get; protected set; }
        public int BreedDelivery_Time { get; protected set; }
        public string Event_Start_Date { get; protected set; }
        public string Event_End_Date { get; protected set; }
        public int Rank { get; protected set; }
        public int Acquire_Xp { get; protected set; }
        public int UI_Scale { get; protected set; }
        public int PosZ { get; protected set; }
        public int RotX { get; protected set; }
        public int RotY { get; protected set; }
        public int RotZ { get; protected set; }


        public static Dictionary<int, tb_Fish> map = new Dictionary<int, tb_Fish>();
        public static List<tb_Fish> list = new List<tb_Fish>();
        public static tb_Fish first = null;

        protected tb_Fish() {}
        public tb_Fish(tb_Fish from)
        {
            this.ID = from.ID;
            this.Local_String = from.Local_String;
            this.FishType = from.FishType;
            this.Fish_Size_Type = from.Fish_Size_Type;
            this.Unlock_Level = from.Unlock_Level;
            this.Family = from.Family;
            this.Create_Reward_ItemIDs = from.Create_Reward_ItemIDs;
            this.Create_Reward_Counts = from.Create_Reward_Counts;
            this.Breed_Able = from.Breed_Able;
            this.InputLife_Count = from.InputLife_Count;
            this.Breed_Rate_Up_ItemID = from.Breed_Rate_Up_ItemID;
            this.Breed_Rate_Up_Count = from.Breed_Rate_Up_Count;
            this.Breed_Rate = from.Breed_Rate;
            this.BreedDelivery_Time = from.BreedDelivery_Time;
            this.Event_Start_Date = from.Event_Start_Date;
            this.Event_End_Date = from.Event_End_Date;
            this.Rank = from.Rank;
            this.Acquire_Xp = from.Acquire_Xp;
            this.UI_Scale = from.UI_Scale;
            this.PosZ = from.PosZ;
            this.RotX = from.RotX;
            this.RotY = from.RotY;
            this.RotZ = from.RotZ;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Fish_internal
        {
            public int ID { get; set; }
            public string Local_String { get; set; }
            public eFishType FishType { get; set; }
            public eFishSizeType Fish_Size_Type { get; set; }
            public int Unlock_Level { get; set; }
            public int Family { get; set; }
            public int Create_Reward_ItemIDs { get; set; }
            public int Create_Reward_Counts { get; set; }
            public byte Breed_Able { get; set; }
            public int InputLife_Count { get; set; }
            public short Breed_Rate_Up_ItemID { get; set; }
            public int Breed_Rate_Up_Count { get; set; }
            public int Breed_Rate { get; set; }
            public int BreedDelivery_Time { get; set; }
            public string Event_Start_Date { get; set; }
            public string Event_End_Date { get; set; }
            public int Rank { get; set; }
            public int Acquire_Xp { get; set; }
            public int UI_Scale { get; set; }
            public int PosZ { get; set; }
            public int RotX { get; set; }
            public int RotY { get; set; }
            public int RotZ { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt32();
                Local_String = reader.ReadString();
                FishType = (eFishType)Enum.Parse(typeof(eFishType), reader.ReadString());
                Fish_Size_Type = (eFishSizeType)Enum.Parse(typeof(eFishSizeType), reader.ReadString());
                Unlock_Level = reader.ReadInt32();
                Family = reader.ReadInt32();
                Create_Reward_ItemIDs = reader.ReadInt32();
                Create_Reward_Counts = reader.ReadInt32();
                Breed_Able = reader.ReadByte();
                InputLife_Count = reader.ReadInt32();
                Breed_Rate_Up_ItemID = reader.ReadInt16();
                Breed_Rate_Up_Count = reader.ReadInt32();
                Breed_Rate = reader.ReadInt32();
                BreedDelivery_Time = reader.ReadInt32();
                Event_Start_Date = reader.ReadString();
                Event_End_Date = reader.ReadString();
                Rank = reader.ReadInt32();
                Acquire_Xp = reader.ReadInt32();
                UI_Scale = reader.ReadInt32();
                PosZ = reader.ReadInt32();
                RotX = reader.ReadInt32();
                RotY = reader.ReadInt32();
                RotZ = reader.ReadInt32();
            }
        }

        private tb_Fish(tb_Fish_internal from)
        {
            this.ID = from.ID;
            this.Local_String = from.Local_String;
            this.FishType = from.FishType;
            this.Fish_Size_Type = from.Fish_Size_Type;
            this.Unlock_Level = from.Unlock_Level;
            this.Family = from.Family;
            this.Create_Reward_ItemIDs = from.Create_Reward_ItemIDs;
            this.Create_Reward_Counts = from.Create_Reward_Counts;
            this.Breed_Able = from.Breed_Able;
            this.InputLife_Count = from.InputLife_Count;
            this.Breed_Rate_Up_ItemID = from.Breed_Rate_Up_ItemID;
            this.Breed_Rate_Up_Count = from.Breed_Rate_Up_Count;
            this.Breed_Rate = from.Breed_Rate;
            this.BreedDelivery_Time = from.BreedDelivery_Time;
            this.Event_Start_Date = from.Event_Start_Date;
            this.Event_End_Date = from.Event_End_Date;
            this.Rank = from.Rank;
            this.Acquire_Xp = from.Acquire_Xp;
            this.UI_Scale = from.UI_Scale;
            this.PosZ = from.PosZ;
            this.RotX = from.RotX;
            this.RotY = from.RotY;
            this.RotZ = from.RotZ;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Fish_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Fish_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Fish info = new tb_Fish(one);
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
                tb_Fish_internal data = new tb_Fish_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Fish info = new tb_Fish(data);
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

        public static tb_Fish Clone(tb_Fish from)
        {
            return new tb_Fish(from);
        }
    }
}
