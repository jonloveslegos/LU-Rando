using LURando.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LURando.Models
{
    public class Missions
    {
        [DbColumn(IsPrimary = true)]
        public Int32 id { get; set; }
        [DbColumn]
        public string defined_type { get; set; }
        [DbColumn]
        public string defined_subtype { get; set; }
        [DbColumn]
        public Int32 UISortOrder { get; set; }
        [DbColumn]
        public Int32 offer_objectID { get; set; }
        [DbColumn]
        public Int32 target_objectID { get; set; }
        [DbColumn]
        public Int64 reward_currency { get; set; }
        [DbColumn]
        public Int32 LegoScore { get; set; }
        [DbColumn]
        public Int64 reward_reputation { get; set; }
        [DbColumn]
        public Int64 isChoiceReward { get; set; }
        [DbColumn]
        public Int32 reward_item1 { get; set; }
        [DbColumn]
        public Int32 reward_item1_count { get; set; }
        [DbColumn]
        public Int32 reward_item2 { get; set; }
        [DbColumn]
        public Int32 reward_item2_count { get; set; }
        [DbColumn]
        public Int32 reward_item3 { get; set; }
        [DbColumn]
        public Int32 reward_item3_count { get; set; }
        [DbColumn]
        public Int32 reward_item4 { get; set; }
        [DbColumn]
        public Int32 reward_item4_count { get; set; }
        [DbColumn]
        public Int32 reward_emote { get; set; }
        [DbColumn]
        public Int32 reward_emote2 { get; set; }
        [DbColumn]
        public Int32 reward_emote3 { get; set; }
        [DbColumn]
        public Int32 reward_emote4 { get; set; }
        [DbColumn]
        public Int32 reward_maximagination { get; set; }
        [DbColumn]
        public Int32 reward_maxhealth { get; set; }
        [DbColumn]
        public Int32 reward_maxinventory { get; set; }
        [DbColumn]
        public Int32 reward_maxmodel { get; set; }
        [DbColumn]
        public Int32 reward_maxwidget { get; set; }
        [DbColumn]
        public Int64 reward_maxwallet { get; set; }
        [DbColumn]
        public Int64 repeatable { get; set; }
        [DbColumn]
        public Int64 reward_currency_repeatable { get; set; }
        [DbColumn]
        public Int32 reward_item1_repeatable { get; set; }
        [DbColumn]
        public Int32 reward_item1_repeat_count { get; set; }
        [DbColumn]
        public Int32 reward_item2_repeatable { get; set; }
        [DbColumn]
        public Int32 reward_item2_repeat_count { get; set; }
        [DbColumn]
        public Int32 reward_item3_repeatable { get; set; }
        [DbColumn]
        public Int32 reward_item3_repeat_count { get; set; }
        [DbColumn]
        public Int32 reward_item4_repeatable { get; set; }
        [DbColumn]
        public Int32 reward_item4_repeat_count { get; set; }
        [DbColumn]
        public Int32 time_limit { get; set; }
        [DbColumn]
        public Int64 isMission { get; set; }
        [DbColumn]
        public Int32 missionIconID { get; set; }
        [DbColumn]
        public string prereqMissionID { get; set; }
        [DbColumn]
        public Int64 localize { get; set; }
        [DbColumn]
        public Int64 inMOTD { get; set; }
        [DbColumn]
        public Int64 cooldownTime { get; set; }
        [DbColumn]
        public Int64 isRandom { get; set; }
        [DbColumn]
        public string randomPool { get; set; }
        [DbColumn]
        public Int32 UIPrereqID { get; set; }
        [DbColumn]
        public string gate_version { get; set; }
        [DbColumn]
        public string HUDStates { get; set; }
        [DbColumn]
        public Int32 locStatus { get; set; }
        [DbColumn]
        public Int32 reward_bankinventory { get; set; }
        public Missions()
        {
        }
        public Missions(Missions mission)
        {
            id = mission.id;
            defined_type = mission.defined_type;
            defined_subtype = mission.defined_subtype;
            UISortOrder = mission.UISortOrder;
            offer_objectID = mission.offer_objectID;
            target_objectID = mission.target_objectID;
            reward_currency = mission.reward_currency;
            LegoScore = mission.LegoScore;
            reward_reputation = mission.reward_reputation;
            isChoiceReward = mission.isChoiceReward;
            reward_item1 = mission.reward_item1;
            reward_item1_count = mission.reward_item1_count;
            reward_item2 = mission.reward_item2;
            reward_item2_count = mission.reward_item2_count;
            reward_item3 = mission.reward_item3;
            reward_item3_count = mission.reward_item3_count;
            reward_item4 = mission.reward_item4;
            reward_item4_count = mission.reward_item4_count;
            reward_emote = mission.reward_emote;
            reward_emote2 = mission.reward_emote2;
            reward_emote3 = mission.reward_emote3;
            reward_emote4 = mission.reward_emote4;
            reward_maximagination = mission.reward_maximagination;
            reward_maxhealth = mission.reward_maxhealth;
            reward_maxinventory = mission.reward_maxinventory;
            reward_maxmodel = mission.reward_maxmodel;
            reward_maxwidget = mission.reward_maxwidget;
            reward_maxwallet = mission.reward_maxwallet;
            repeatable = mission.repeatable;
            reward_currency_repeatable = mission.reward_currency_repeatable;
            reward_item1_repeatable = mission.reward_item1_repeatable;
            reward_item1_repeat_count = mission.reward_item1_repeat_count;
            reward_item2_repeatable = mission.reward_item2_repeatable;
            reward_item2_repeat_count = mission.reward_item2_repeat_count;
            reward_item3_repeatable = mission.reward_item3_repeatable;
            reward_item3_repeat_count = mission.reward_item3_repeat_count;
            reward_item4_repeatable = mission.reward_item4_repeatable;
            reward_item4_repeat_count = mission.reward_item4_repeat_count;
            time_limit = mission.time_limit;
            isMission = mission.isMission;
            missionIconID = mission.missionIconID;
            prereqMissionID = mission.prereqMissionID;
            localize = mission.localize;
            inMOTD = mission.inMOTD;
            cooldownTime = mission.cooldownTime;
            isRandom = mission.isRandom;
            randomPool = mission.randomPool;
            UIPrereqID = mission.UIPrereqID;
            gate_version = mission.gate_version;
            HUDStates = mission.HUDStates;
            locStatus = mission.locStatus;
            reward_bankinventory = mission.reward_bankinventory;
        }
    }
}
