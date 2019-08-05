/////////////////////////////////////////
// Export To ABSW_MapData.xlsm
// Last Update : 2019-06-28:14:18:58
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Map_Area
    {
        public int ID { get; protected set; }
        public int Index_X { get; protected set; }
        public int Index_Y { get; protected set; }
        public int Position_X { get; protected set; }
        public int Position_Y { get; protected set; }
        public int Starting_Open { get; protected set; }
        public int Stone_Open { get; protected set; }


        public static Dictionary<int, tb_Map_Area> map = new Dictionary<int, tb_Map_Area>();
        public static List<tb_Map_Area> list = new List<tb_Map_Area>();
        public static tb_Map_Area first = null;

        protected tb_Map_Area() {}
        public tb_Map_Area(tb_Map_Area from)
        {
            this.ID = from.ID;
            this.Index_X = from.Index_X;
            this.Index_Y = from.Index_Y;
            this.Position_X = from.Position_X;
            this.Position_Y = from.Position_Y;
            this.Starting_Open = from.Starting_Open;
            this.Stone_Open = from.Stone_Open;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Map_Area_internal
        {
            public int ID { get; set; }
            public int Index_X { get; set; }
            public int Index_Y { get; set; }
            public int Position_X { get; set; }
            public int Position_Y { get; set; }
            public int Starting_Open { get; set; }
            public int Stone_Open { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt32();
                Index_X = reader.ReadInt32();
                Index_Y = reader.ReadInt32();
                Position_X = reader.ReadInt32();
                Position_Y = reader.ReadInt32();
                Starting_Open = reader.ReadInt32();
                Stone_Open = reader.ReadInt32();
            }
        }

        private tb_Map_Area(tb_Map_Area_internal from)
        {
            this.ID = from.ID;
            this.Index_X = from.Index_X;
            this.Index_Y = from.Index_Y;
            this.Position_X = from.Position_X;
            this.Position_Y = from.Position_Y;
            this.Starting_Open = from.Starting_Open;
            this.Stone_Open = from.Stone_Open;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Map_Area_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Map_Area_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Map_Area info = new tb_Map_Area(one);
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
                tb_Map_Area_internal data = new tb_Map_Area_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Map_Area info = new tb_Map_Area(data);
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

        public static tb_Map_Area Clone(tb_Map_Area from)
        {
            return new tb_Map_Area(from);
        }
    }
}
