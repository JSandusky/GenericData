using System;
using System.Collections.Generic;
using System.Text;
using DataUtils;
using System.Drawing;

namespace FTL {

    [Serializable]
    public class Player : DataObject {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime BanEnd { get; set; }
        public string BanNote { get; set; }
    }

    [Serializable]
    public class Match : DataObject {
        public DateTime MatchDate { get; set; }
        public Stage MatchStage { get; set; }
        public List<Player> Players { get;set; }
        public List<MatchData> DataItems { get; set; }
    }

    [Serializable]
    public class MatchData : DataObject {
        public Player Who { get; set; }
        public int Team { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int Score { get; set; }
        public Wep MostPopular { get; set; }
    }

    [Serializable]
    public class Wep : DataObject {
        public string ItemName { get; set; }
        public string ItemDesc { get; set; }
        public string ItemCost { get; set; }
        public ItemType ItemType { get; set; }
        public string Rarity { get; set; }
        public string ItemIcon { get; set; }
    }

    [Serializable]
    public class ItemType : DataObject {
        public string TypeName { get; set; }
        public string TypeDesc { get; set; }
        public string TypeIcon { get; set; }
    }

    [Serializable]
    public class Projectile : DataObject {
        public string ProjDesc { get; set; }
        public int Damage { get; set; }
        public string ProjType { get; set; }
    }

    [Serializable]
    public class Creep : DataObject {
        public string CreepName { get; set; }
        public string CreepDesc { get; set; }
        public int CreepHP { get; set; }
        public int CreepXP { get; set; }
    }

    [Serializable]
    public class TypedCreep : Creep {
        public int TypeLevel { get; set; }
    }

    [Serializable]
    public class Buff : DataObject {
        public string BuffName { get; set; } //Waller/Jailer/Mortar
        public string BuffItemName { get; set; } //of shocking
        public bool isPrefix { get; set; } //molten
        public Color colorValue { get; set; } //used to setup a tint on something buffed by this, 000000 is ignored
        public string ParticleEffect { get; set; } //used to setup a p-effect on something buffed by this
        public string TargetProperty { get; set; } //reflection
        public string AttachSkill { get; set; } //like D3 waller/jailer/mortar
        public float ModValue { get; set; }
        public bool isMultiplier { get; set; } //use * instead of +
        public int PointsValue { get; set; } //adds point value to whoever is buffed
    }

    [Serializable]
    public class Stage : DataObject {
        public string StageName { get; set; }
        public string ForTypes { get; set; }
        public string StageDesc { get; set; }
    }

    [Serializable]
    public class Skill : DataObject {
        public string SkillName { get; set; }
        public string SkillDescription { get; set; }
        public float Cooldown { get; set; }
        public int Tier { get; set; }
    }

    [Serializable]
    public class Cast : DataObject {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public int Tier { get; set; }
    }

    [Serializable]
    public class Summon : Cast {
        public Creep Spawns { get; set; }
    }

    [Serializable]
    public class Invocation : Cast {
    }

    [Serializable]
    public class Evocation : Cast {
    }
}
