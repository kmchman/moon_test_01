/////////////////////////////////////////
// Export To ABSW_StoneData.xlsm
// Last Update : 2019-04-29:14:42:20
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Stone_Costume
    {
        public short ID { get; protected set; }
        public string Local_String { get; protected set; }
        public short CostumeSet { get; protected set; }
        public string CostumePart { get; protected set; }
        public short Unlock_Stone { get; protected set; }
        public string CostumeType { get; protected set; }
        public short Input_ItemID { get; protected set; }
        public int Input_Count { get; protected set; }
        public string PurchaseDate_Start { get; protected set; }
        public string PurchaseDate_End { get; protected set; }
        public string DressDate_End { get; protected set; }


        public static Dictionary<short, tb_Stone_Costume> map = new Dictionary<short, tb_Stone_Costume>();
        public static List<tb_Stone_Costume> list = new List<tb_Stone_Costume>();
        public static tb_Stone_Costume first = null;

        protected tb_Stone_Costume() {}
        public tb_Stone_Costume(tb_Stone_Costume from)
        {
            this.ID = from.ID;
            this.Local_String = from.Local_String;
            this.CostumeSet = from.CostumeSet;
            this.CostumePart = from.CostumePart;
            this.Unlock_Stone = from.Unlock_Stone;
            this.CostumeType = from.CostumeType;
            this.Input_ItemID = from.Input_ItemID;
            this.Input_Count = from.Input_Count;
            this.PurchaseDate_Start = from.PurchaseDate_Start;
            this.PurchaseDate_End = from.PurchaseDate_End;
            this.DressDate_End = from.DressDate_End;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Stone_Costume_internal
        {
            public short ID { get; set; }
            public string Local_String { get; set; }
            public short CostumeSet { get; set; }
            public string CostumePart { get; set; }
            public short Unlock_Stone { get; set; }
            public string CostumeType { get; set; }
            public short Input_ItemID { get; set; }
            public int Input_Count { get; set; }
            public string PurchaseDate_Start { get; set; }
            public string PurchaseDate_End { get; set; }
            public string DressDate_End { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt16();
                Local_String = reader.ReadString();
                CostumeSet = reader.ReadInt16();
                CostumePart = reader.ReadString();
                Unlock_Stone = reader.ReadInt16();
                CostumeType = reader.ReadString();
                Input_ItemID = reader.ReadInt16();
                Input_Count = reader.ReadInt32();
                PurchaseDate_Start = reader.ReadString();
                PurchaseDate_End = reader.ReadString();
                DressDate_End = reader.ReadString();
            }
        }

        private tb_Stone_Costume(tb_Stone_Costume_internal from)
        {
            this.ID = from.ID;
            this.Local_String = from.Local_String;
            this.CostumeSet = from.CostumeSet;
            this.CostumePart = from.CostumePart;
            this.Unlock_Stone = from.Unlock_Stone;
            this.CostumeType = from.CostumeType;
            this.Input_ItemID = from.Input_ItemID;
            this.Input_Count = from.Input_Count;
            this.PurchaseDate_Start = from.PurchaseDate_Start;
            this.PurchaseDate_End = from.PurchaseDate_End;
            this.DressDate_End = from.DressDate_End;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Stone_Costume_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Stone_Costume_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Stone_Costume info = new tb_Stone_Costume(one);
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
                tb_Stone_Costume_internal data = new tb_Stone_Costume_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Stone_Costume info = new tb_Stone_Costume(data);
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

        public static tb_Stone_Costume Clone(tb_Stone_Costume from)
        {
            return new tb_Stone_Costume(from);
        }
    }
}
