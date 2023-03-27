using System.Collections.Generic;

namespace MonsterHunterWorldBoardGameCompanionApp.Scripts.Data
{
    public class Player : IDictionable
    {
        public int Number;
        public string Name;
        public string HunterName;
        public string PalicoName;
        public Dictionary<int, int> CommonItems;

        public Player(int number, string name, string hunterName, string palicoName)
        {
            CommonItems = new Dictionary<int, int>();
            Number = number;
            Name = name;
            HunterName = hunterName;
            PalicoName = palicoName;
        }

        public Player(Godot.Collections.Dictionary dict)
        {
            CommonItems = new Dictionary<int, int>();
            Number = (int)(dict.Contains("Number") ? (float)dict["Number"] : 0f);
            Name = dict.Contains("Name") ? (string)dict["Name"] : null;
            HunterName = dict.Contains("HunterName") ? (string)dict["HunterName"] : null;
            PalicoName = dict.Contains("PalicoName") ? (string)dict["PalicoName"] : null;
        }

        public Godot.Collections.Dictionary<string, object> ToDict() =>
            new Godot.Collections.Dictionary<string, object>()
            {
                { nameof(Number), Number },
                { nameof(Name), Name },
                { nameof(HunterName), HunterName },
                { nameof(PalicoName), PalicoName },
                { nameof(CommonItems), new Godot.Collections.Dictionary(CommonItems) }
            };
    }
}
