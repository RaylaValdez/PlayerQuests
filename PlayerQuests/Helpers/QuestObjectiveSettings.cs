using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerQuests.Helpers
{
    internal sealed class QuestObjectiveSettings
    {
        public string Objective { get; set; } = null!;

        public QuestObjectiveSettings? Clone() => this.MemberwiseClone() as QuestObjectiveSettings;
    }
}
