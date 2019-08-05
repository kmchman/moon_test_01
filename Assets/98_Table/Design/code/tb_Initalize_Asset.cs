/////////////////////////////////////////
// Export To ABSW_InitializeData.xlsm
// Last Update : 2019-07-12:18:44:33
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Initalize_Asset
    {
        public short ID { get; protected set; }
        public int Life { get; protected set; }
        public int Cash { get; protected set; }


        public static Dictionary<short, tb_Initalize_Asset> map = new Dictionary<short, tb_Initalize_Asset>();
        public static List<tb_Initalize_Asset> list = new List<tb_Initalize_Asset>();
        public static tb_Initalize_Asset first = null;

        protected tb_Initalize_Asset() {}
        public tb_Initalize_Asset(tb_Initalize_Asset from)
        {
            this.ID = from.ID;
            this.Life = from.Life;
            this.Cash = from.Cash;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Initalize_Asset_internal
        {
            public short ID { get; set; }
            public int Life { get; set; }
            public int Cash { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt16();
                Life = reader.ReadInt32();
                Cash = reader.ReadInt32();
            }
        }

        private tb_Initalize_Asset(tb_Initalize_Asset_internal from)
        {
            this.ID = from.ID;
            this.Life = from.Life;
            this.Cash = from.Cash;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Initalize_Asset_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Initalize_Asset_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Initalize_Asset info = new tb_Initalize_Asset(one);
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
                tb_Initalize_Asset_internal data = new tb_Initalize_Asset_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Initalize_Asset info = new tb_Initalize_Asset(data);
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

        public static tb_Initalize_Asset Clone(tb_Initalize_Asset from)
        {
            return new tb_Initalize_Asset(from);
        }
    }
}
