/////////////////////////////////////////
// Export To ABSW_BreedData.xlsm
// Last Update : 2019-07-15:20:46:52
/////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    public class tb_Breed_SocialReward
    {
        public byte ID { get; protected set; }
        public string Local_String { get; protected set; }
        public int Unlock_level { get; protected set; }
        public int Reward_Rate { get; protected set; }
        public int RewardDouble_Rate { get; protected set; }


        public static Dictionary<byte, tb_Breed_SocialReward> map = new Dictionary<byte, tb_Breed_SocialReward>();
        public static List<tb_Breed_SocialReward> list = new List<tb_Breed_SocialReward>();
        public static tb_Breed_SocialReward first = null;

        protected tb_Breed_SocialReward() {}
        public tb_Breed_SocialReward(tb_Breed_SocialReward from)
        {
            this.ID = from.ID;
            this.Local_String = from.Local_String;
            this.Unlock_level = from.Unlock_level;
            this.Reward_Rate = from.Reward_Rate;
            this.RewardDouble_Rate = from.RewardDouble_Rate;
        }


        /////////////////////////////////////////////////////////////////////////
        // for loading
        [Serializable]
        class tb_Breed_SocialReward_internal
        {
            public byte ID { get; set; }
            public string Local_String { get; set; }
            public int Unlock_level { get; set; }
            public int Reward_Rate { get; set; }
            public int RewardDouble_Rate { get; set; }

            public void Read(BinaryReader reader)
            {
                ID = reader.ReadByte();
                Local_String = reader.ReadString();
                Unlock_level = reader.ReadInt32();
                Reward_Rate = reader.ReadInt32();
                RewardDouble_Rate = reader.ReadInt32();
            }
        }

        private tb_Breed_SocialReward(tb_Breed_SocialReward_internal from)
        {
            this.ID = from.ID;
            this.Local_String = from.Local_String;
            this.Unlock_level = from.Unlock_level;
            this.Reward_Rate = from.Reward_Rate;
            this.RewardDouble_Rate = from.RewardDouble_Rate;

        }
        // for loading
        /////////////////////////////////////////////////////////////////////////


        public static void Load(string json)
        {
            Clear();

            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            List<tb_Breed_SocialReward_internal> data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tb_Breed_SocialReward_internal>>(json, settings);

            foreach (var one in data)
            {
                tb_Breed_SocialReward info = new tb_Breed_SocialReward(one);
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
                tb_Breed_SocialReward_internal data = new tb_Breed_SocialReward_internal();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    data.Read(reader);

                    tb_Breed_SocialReward info = new tb_Breed_SocialReward(data);
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

        public static tb_Breed_SocialReward Clone(tb_Breed_SocialReward from)
        {
            return new tb_Breed_SocialReward(from);
        }
    }
}
