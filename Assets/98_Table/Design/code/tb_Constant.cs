/////////////////////////////////////////
// Export To ABSW_ConstantData.xlsm
// Last Update : 2019-07-24:17:10:13
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Constant
    {
        public eConstantType ConstantType { get; protected set; }
        public int Value { get; protected set; }
        public string name { get; protected set; }


        public static Dictionary<eConstantType, tb_Constant> map = new Dictionary<eConstantType, tb_Constant>();
        public static List<tb_Constant> list = new List<tb_Constant>();
        public static tb_Constant first = null;

        protected tb_Constant() {}
        public tb_Constant(tb_Constant from)
        {
            this.ConstantType = from.ConstantType;
            this.Value = from.Value;
            this.name = from.name;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Constant_internal
        {
            public eConstantType ConstantType { get; set; }
            public int Value { get; set; }
            public string name { get; set; }

            public void Read(BinaryReader reader)
            {
                ConstantType = (eConstantType)Enum.Parse(typeof(eConstantType), reader.ReadString());
                Value = reader.ReadInt32();
                name = reader.ReadString();
            }
        }

        private tb_Constant(tb_Constant_internal from)
        {
            this.ConstantType = from.ConstantType;
            this.Value = from.Value;
            this.name = from.name;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Constant_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Constant_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Constant info = new tb_Constant(one);
                list.Add(info);
                map.Add(info.ConstantType, info);
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
                tb_Constant_internal data = new tb_Constant_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Constant info = new tb_Constant(data);
                    list.Add(info);
                    map.Add(info.ConstantType, info);
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

        public static tb_Constant Clone(tb_Constant from)
        {
            return new tb_Constant(from);
        }
    }
}
