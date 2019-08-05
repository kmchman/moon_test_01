/////////////////////////////////////////
// Export To ABSW_BuildingData.xlsm
// Last Update : 2019-07-19:14:46:25
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Building_House
    {
        public short ID { get; protected set; }
        public int Add_Max_Population { get; protected set; }


        public static Dictionary<short, tb_Building_House> map = new Dictionary<short, tb_Building_House>();
        public static List<tb_Building_House> list = new List<tb_Building_House>();
        public static tb_Building_House first = null;

        protected tb_Building_House() {}
        public tb_Building_House(tb_Building_House from)
        {
            this.ID = from.ID;
            this.Add_Max_Population = from.Add_Max_Population;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Building_House_internal
        {
            public short ID { get; set; }
            public int Add_Max_Population { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt16();
                Add_Max_Population = reader.ReadInt32();
            }
        }

        private tb_Building_House(tb_Building_House_internal from)
        {
            this.ID = from.ID;
            this.Add_Max_Population = from.Add_Max_Population;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Building_House_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Building_House_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Building_House info = new tb_Building_House(one);
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
                tb_Building_House_internal data = new tb_Building_House_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Building_House info = new tb_Building_House(data);
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

        public static tb_Building_House Clone(tb_Building_House from)
        {
            return new tb_Building_House(from);
        }
    }
}
