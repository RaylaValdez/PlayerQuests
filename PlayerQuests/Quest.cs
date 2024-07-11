using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Dalamud.Interface.Textures;

namespace PlayerQuests
{
    public class Quest : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Reward { get; set; }
        public string QuestType { get; set; } = null!;
        public float QuestPositionX = 0f;
        public float QuestPositionY = 0f;
        public float QuestPositionZ = 0f;
        public Vector3 QuestPosition => new(QuestPositionX, QuestPositionY, QuestPositionZ);
        public string QuestZone { get; set; } = null!;
        public string DatacenterName { get; set; } = null!;
        public string WorldName { get; set; } = null!;
        public Int64 TimePosted { get; set; }
        public Int64 ExpireTime { get; set; }
        public bool Accepted { get; set; }
        public bool Completed { get; set; }
        public string QuestAuthor { get; set; } = null!;

        public static string SerializeQuest(Quest quest)
        {
            var jsonSerializeOptions = new JsonSerializerOptions { WriteIndented = true };
            var options = jsonSerializeOptions;
            var jsonString = JsonSerializer.Serialize(quest, options);

            return jsonString;
        }


        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    
}
