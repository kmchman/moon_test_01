/////////////////////////////////////////
// Export To ABSW_BuildingData.xlsm
// Last Update : 2019-07-19:14:46:25
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Building_Farmland
    {
        public short ID { get; protected set; }
        public int QueSlot_Default { get; protected set; }
        public int QueSlot_Max { get; protected set; }
        public string QueSlot_Cost { get; protected set; }
        public int MultiQueSlot { get; protected set; }
        public int HoldSlot_Default { get; protected set; }
        public int HoldSlot_Max { get; protected set; }


        public static Dictionary<short, tb_Building_Farmland> map = new Dictionary<short, tb_Building_Farmland>();
        public static List<tb_Building_Farmland> list = new List<tb_Building_Farmland>();
        public static tb_Building_Farmland first = null;

        protected tb_Building_Farmland() {}
        public tb_Building_Farmland(tb_Building_Farmland from)
        {
            this.ID = from.ID;
            this.QueSlot_Default = from.QueSlot_Default;
            this.QueSlot_Max = from.QueSlot_Max;
            this.QueSlot_Cost = from.QueSlot_Cost;
            this.MultiQueSlot = from.MultiQueSlot;
            this.HoldSlot_Default = from.HoldSlot_Default;
            this.HoldSlot_Max = from.HoldSlot_Max;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Building_Farmland_internal
        {
            public short ID { get; set; }
            public int QueSlot_Default { get; set; }
            public int QueSlot_Max { get; set; }
            public string QueSlot_Cost { get; set; }
            public int MultiQueSlot { get; set; }
            public int HoldSlot_Default { get; set; }
            public int HoldSlot_Max { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt16();
                QueSlot_Default = reader.ReadInt32();
                QueSlot_Max = reader.ReadInt32();
                QueSlot_Cost = reader.ReadString();
                MultiQueSlot = reader.ReadInt32();
                HoldSlot_Default = reader.ReadInt32();
                HoldSlot_Max = reader.ReadInt32();
            }
        }

        private tb_Building_Farmland(tb_Building_Farmland_internal from)
        {
            this.ID = from.ID;
            this.QueSlot_Default = from.QueSlot_Default;
            this.QueSlot_Max = from.QueSlot_Max;
            this.QueSlot_Cost = from.QueSlot_Cost;
            this.MultiQueSlot = from.MultiQueSlot;
            this.HoldSlot_Default = from.HoldSlot_Default;
            this.HoldSlot_Max = from.HoldSlot_Max;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Building_Farmland_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Building_Farmland_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Building_Farmland info = new tb_Building_Farmland(one);
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
                tb_Building_Farmland_internal data = new tb_Building_Farmland_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Building_Farmland info = new tb_Building_Farmland(data);
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

        public static tb_Building_Farmland Clone(tb_Building_Farmland from)
        {
            return new tb_Building_Farmland(from);
        }
    }
}
