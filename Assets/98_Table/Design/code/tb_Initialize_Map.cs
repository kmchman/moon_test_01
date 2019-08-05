/////////////////////////////////////////
// Export To ABSW_InitializeData.xlsm
// Last Update : 2019-07-12:18:44:33
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Initialize_Map
    {
        public short ID { get; protected set; }
        public short BuildingID { get; protected set; }
        public short PosX { get; protected set; }
        public short PosY { get; protected set; }
        public short Rot { get; protected set; }


        public static Dictionary<short, tb_Initialize_Map> map = new Dictionary<short, tb_Initialize_Map>();
        public static List<tb_Initialize_Map> list = new List<tb_Initialize_Map>();
        public static tb_Initialize_Map first = null;

        protected tb_Initialize_Map() {}
        public tb_Initialize_Map(tb_Initialize_Map from)
        {
            this.ID = from.ID;
            this.BuildingID = from.BuildingID;
            this.PosX = from.PosX;
            this.PosY = from.PosY;
            this.Rot = from.Rot;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Initialize_Map_internal
        {
            public short ID { get; set; }
            public short BuildingID { get; set; }
            public short PosX { get; set; }
            public short PosY { get; set; }
            public short Rot { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt16();
                BuildingID = reader.ReadInt16();
                PosX = reader.ReadInt16();
                PosY = reader.ReadInt16();
                Rot = reader.ReadInt16();
            }
        }

        private tb_Initialize_Map(tb_Initialize_Map_internal from)
        {
            this.ID = from.ID;
            this.BuildingID = from.BuildingID;
            this.PosX = from.PosX;
            this.PosY = from.PosY;
            this.Rot = from.Rot;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Initialize_Map_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Initialize_Map_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Initialize_Map info = new tb_Initialize_Map(one);
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
                tb_Initialize_Map_internal data = new tb_Initialize_Map_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Initialize_Map info = new tb_Initialize_Map(data);
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

        public static tb_Initialize_Map Clone(tb_Initialize_Map from)
        {
            return new tb_Initialize_Map(from);
        }
    }
}
