/////////////////////////////////////////
// Export To ABSW_ItemData.xlsm
// Last Update : 2019-07-18:14:18:59
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Item
    {
        public short ID { get; protected set; }
        public string Name { get; protected set; }
        public string Local_String { get; protected set; }
        public eItemType ItemType { get; protected set; }
        public eStorageType StorageType { get; protected set; }
        public eStorageFilterType StorageFilterType { get; protected set; }
        public int Cash_Price { get; protected set; }
        public int Refund_Price { get; protected set; }
        public short BuildingID { get; protected set; }
        public int Unlock_Level { get; protected set; }
        public int Product_Time { get; protected set; }
        public int Product_Count { get; protected set; }
        public string Input_ItemIDs { get; protected set; }
        public string Input_Counts { get; protected set; }
        public int Acquire_XP { get; protected set; }
        public int Reward_Value { get; protected set; }
        public int Quest_Condition01_MaxNum { get; protected set; }
        public int Quest_Condition02_MaxNum { get; protected set; }
        public int Quest_Condition03_MaxNum { get; protected set; }
        public int Quest_Condition04_MaxNum { get; protected set; }
        public int Quest_Condition05_MaxNum { get; protected set; }
        public int Quest_Condition06_MaxNum { get; protected set; }


        public static Dictionary<short, tb_Item> map = new Dictionary<short, tb_Item>();
        public static List<tb_Item> list = new List<tb_Item>();
        public static tb_Item first = null;

        protected tb_Item() {}
        public tb_Item(tb_Item from)
        {
            this.ID = from.ID;
            this.Name = from.Name;
            this.Local_String = from.Local_String;
            this.ItemType = from.ItemType;
            this.StorageType = from.StorageType;
            this.StorageFilterType = from.StorageFilterType;
            this.Cash_Price = from.Cash_Price;
            this.Refund_Price = from.Refund_Price;
            this.BuildingID = from.BuildingID;
            this.Unlock_Level = from.Unlock_Level;
            this.Product_Time = from.Product_Time;
            this.Product_Count = from.Product_Count;
            this.Input_ItemIDs = from.Input_ItemIDs;
            this.Input_Counts = from.Input_Counts;
            this.Acquire_XP = from.Acquire_XP;
            this.Reward_Value = from.Reward_Value;
            this.Quest_Condition01_MaxNum = from.Quest_Condition01_MaxNum;
            this.Quest_Condition02_MaxNum = from.Quest_Condition02_MaxNum;
            this.Quest_Condition03_MaxNum = from.Quest_Condition03_MaxNum;
            this.Quest_Condition04_MaxNum = from.Quest_Condition04_MaxNum;
            this.Quest_Condition05_MaxNum = from.Quest_Condition05_MaxNum;
            this.Quest_Condition06_MaxNum = from.Quest_Condition06_MaxNum;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Item_internal
        {
            public short ID { get; set; }
            public string Name { get; set; }
            public string Local_String { get; set; }
            public eItemType ItemType { get; set; }
            public eStorageType StorageType { get; set; }
            public eStorageFilterType StorageFilterType { get; set; }
            public int Cash_Price { get; set; }
            public int Refund_Price { get; set; }
            public short BuildingID { get; set; }
            public int Unlock_Level { get; set; }
            public int Product_Time { get; set; }
            public int Product_Count { get; set; }
            public string Input_ItemIDs { get; set; }
            public string Input_Counts { get; set; }
            public int Acquire_XP { get; set; }
            public int Reward_Value { get; set; }
            public int Quest_Condition01_MaxNum { get; set; }
            public int Quest_Condition02_MaxNum { get; set; }
            public int Quest_Condition03_MaxNum { get; set; }
            public int Quest_Condition04_MaxNum { get; set; }
            public int Quest_Condition05_MaxNum { get; set; }
            public int Quest_Condition06_MaxNum { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt16();
                Name = reader.ReadString();
                Local_String = reader.ReadString();
                ItemType = (eItemType)Enum.Parse(typeof(eItemType), reader.ReadString());
                StorageType = (eStorageType)Enum.Parse(typeof(eStorageType), reader.ReadString());
                StorageFilterType = (eStorageFilterType)Enum.Parse(typeof(eStorageFilterType), reader.ReadString());
                Cash_Price = reader.ReadInt32();
                Refund_Price = reader.ReadInt32();
                BuildingID = reader.ReadInt16();
                Unlock_Level = reader.ReadInt32();
                Product_Time = reader.ReadInt32();
                Product_Count = reader.ReadInt32();
                Input_ItemIDs = reader.ReadString();
                Input_Counts = reader.ReadString();
                Acquire_XP = reader.ReadInt32();
                Reward_Value = reader.ReadInt32();
                Quest_Condition01_MaxNum = reader.ReadInt32();
                Quest_Condition02_MaxNum = reader.ReadInt32();
                Quest_Condition03_MaxNum = reader.ReadInt32();
                Quest_Condition04_MaxNum = reader.ReadInt32();
                Quest_Condition05_MaxNum = reader.ReadInt32();
                Quest_Condition06_MaxNum = reader.ReadInt32();
            }
        }

        private tb_Item(tb_Item_internal from)
        {
            this.ID = from.ID;
            this.Name = from.Name;
            this.Local_String = from.Local_String;
            this.ItemType = from.ItemType;
            this.StorageType = from.StorageType;
            this.StorageFilterType = from.StorageFilterType;
            this.Cash_Price = from.Cash_Price;
            this.Refund_Price = from.Refund_Price;
            this.BuildingID = from.BuildingID;
            this.Unlock_Level = from.Unlock_Level;
            this.Product_Time = from.Product_Time;
            this.Product_Count = from.Product_Count;
            this.Input_ItemIDs = from.Input_ItemIDs;
            this.Input_Counts = from.Input_Counts;
            this.Acquire_XP = from.Acquire_XP;
            this.Reward_Value = from.Reward_Value;
            this.Quest_Condition01_MaxNum = from.Quest_Condition01_MaxNum;
            this.Quest_Condition02_MaxNum = from.Quest_Condition02_MaxNum;
            this.Quest_Condition03_MaxNum = from.Quest_Condition03_MaxNum;
            this.Quest_Condition04_MaxNum = from.Quest_Condition04_MaxNum;
            this.Quest_Condition05_MaxNum = from.Quest_Condition05_MaxNum;
            this.Quest_Condition06_MaxNum = from.Quest_Condition06_MaxNum;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Item_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Item_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Item info = new tb_Item(one);
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
                tb_Item_internal data = new tb_Item_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Item info = new tb_Item(data);
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

        public static tb_Item Clone(tb_Item from)
        {
            return new tb_Item(from);
        }
    }
}
