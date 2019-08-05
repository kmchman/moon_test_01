/////////////////////////////////////////
// Export To ABSW_BreedData.xlsm
// Last Update : 2019-07-15:20:46:52
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Breed_SocialSlot
    {
        public byte ID { get; protected set; }
        public int Purchase_Able { get; protected set; }
        public int InputCash_Count { get; protected set; }


        public static Dictionary<byte, tb_Breed_SocialSlot> map = new Dictionary<byte, tb_Breed_SocialSlot>();
        public static List<tb_Breed_SocialSlot> list = new List<tb_Breed_SocialSlot>();
        public static tb_Breed_SocialSlot first = null;

        protected tb_Breed_SocialSlot() {}
        public tb_Breed_SocialSlot(tb_Breed_SocialSlot from)
        {
            this.ID = from.ID;
            this.Purchase_Able = from.Purchase_Able;
            this.InputCash_Count = from.InputCash_Count;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Breed_SocialSlot_internal
        {
            public byte ID { get; set; }
            public int Purchase_Able { get; set; }
            public int InputCash_Count { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadByte();
                Purchase_Able = reader.ReadInt32();
                InputCash_Count = reader.ReadInt32();
            }
        }

        private tb_Breed_SocialSlot(tb_Breed_SocialSlot_internal from)
        {
            this.ID = from.ID;
            this.Purchase_Able = from.Purchase_Able;
            this.InputCash_Count = from.InputCash_Count;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Breed_SocialSlot_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Breed_SocialSlot_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Breed_SocialSlot info = new tb_Breed_SocialSlot(one);
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
                tb_Breed_SocialSlot_internal data = new tb_Breed_SocialSlot_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Breed_SocialSlot info = new tb_Breed_SocialSlot(data);
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

        public static tb_Breed_SocialSlot Clone(tb_Breed_SocialSlot from)
        {
            return new tb_Breed_SocialSlot(from);
        }
    }
}
