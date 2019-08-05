/////////////////////////////////////////
// Export To ABSW_BuildingData.xlsm
// Last Update : 2019-07-19:14:46:25
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Building_Base
    {
        public short ID { get; protected set; }
        public string Name { get; protected set; }
        public string Local_String { get; protected set; }
        public int Open_Level { get; protected set; }
        public int SizeX { get; protected set; }
        public int SizeY { get; protected set; }
        public eBuildingType BuildingType { get; protected set; }


        public static Dictionary<short, tb_Building_Base> map = new Dictionary<short, tb_Building_Base>();
        public static List<tb_Building_Base> list = new List<tb_Building_Base>();
        public static tb_Building_Base first = null;

        protected tb_Building_Base() {}
        public tb_Building_Base(tb_Building_Base from)
        {
            this.ID = from.ID;
            this.Name = from.Name;
            this.Local_String = from.Local_String;
            this.Open_Level = from.Open_Level;
            this.SizeX = from.SizeX;
            this.SizeY = from.SizeY;
            this.BuildingType = from.BuildingType;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Building_Base_internal
        {
            public short ID { get; set; }
            public string Name { get; set; }
            public string Local_String { get; set; }
            public int Open_Level { get; set; }
            public int SizeX { get; set; }
            public int SizeY { get; set; }
            public eBuildingType BuildingType { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt16();
                Name = reader.ReadString();
                Local_String = reader.ReadString();
                Open_Level = reader.ReadInt32();
                SizeX = reader.ReadInt32();
                SizeY = reader.ReadInt32();
                BuildingType = (eBuildingType)Enum.Parse(typeof(eBuildingType), reader.ReadString());
            }
        }

        private tb_Building_Base(tb_Building_Base_internal from)
        {
            this.ID = from.ID;
            this.Name = from.Name;
            this.Local_String = from.Local_String;
            this.Open_Level = from.Open_Level;
            this.SizeX = from.SizeX;
            this.SizeY = from.SizeY;
            this.BuildingType = from.BuildingType;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Building_Base_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Building_Base_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Building_Base info = new tb_Building_Base(one);
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
                tb_Building_Base_internal data = new tb_Building_Base_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Building_Base info = new tb_Building_Base(data);
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

        public static tb_Building_Base Clone(tb_Building_Base from)
        {
            return new tb_Building_Base(from);
        }
    }
}
