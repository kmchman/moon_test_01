/////////////////////////////////////////
// Export To ABSW_StoneData.xlsm
// Last Update : 2019-04-29:14:42:20
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Stone_Profile
    {
        public short ID { get; protected set; }
        public eProfileType ProfileType { get; protected set; }
        public int Unlock_Fish { get; protected set; }
        public int Unlock_CostumeSet { get; protected set; }


        public static Dictionary<short, tb_Stone_Profile> map = new Dictionary<short, tb_Stone_Profile>();
        public static List<tb_Stone_Profile> list = new List<tb_Stone_Profile>();
        public static tb_Stone_Profile first = null;

        protected tb_Stone_Profile() {}
        public tb_Stone_Profile(tb_Stone_Profile from)
        {
            this.ID = from.ID;
            this.ProfileType = from.ProfileType;
            this.Unlock_Fish = from.Unlock_Fish;
            this.Unlock_CostumeSet = from.Unlock_CostumeSet;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Stone_Profile_internal
        {
            public short ID { get; set; }
            public eProfileType ProfileType { get; set; }
            public int Unlock_Fish { get; set; }
            public int Unlock_CostumeSet { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadInt16();
                ProfileType = (eProfileType)Enum.Parse(typeof(eProfileType), reader.ReadString());
                Unlock_Fish = reader.ReadInt32();
                Unlock_CostumeSet = reader.ReadInt32();
            }
        }

        private tb_Stone_Profile(tb_Stone_Profile_internal from)
        {
            this.ID = from.ID;
            this.ProfileType = from.ProfileType;
            this.Unlock_Fish = from.Unlock_Fish;
            this.Unlock_CostumeSet = from.Unlock_CostumeSet;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Stone_Profile_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Stone_Profile_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Stone_Profile info = new tb_Stone_Profile(one);
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
                tb_Stone_Profile_internal data = new tb_Stone_Profile_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Stone_Profile info = new tb_Stone_Profile(data);
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

        public static tb_Stone_Profile Clone(tb_Stone_Profile from)
        {
            return new tb_Stone_Profile(from);
        }
    }
}
