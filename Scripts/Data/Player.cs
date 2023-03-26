namespace MonsterHunterWorldBoardGameCompanionApp.Scripts.Data
{
    public class Player
    {
        public int Number;
        public string Name;
        public string HunterName;
        public string PalicoName;

        public Player(int number, string name, string hunterName, string palicoName)
        {
            Number = number;
            Name = name;
            HunterName = hunterName;
            PalicoName = palicoName;
        }

        public Godot.Collections.Dictionary<string, object> AsDict() =>
            new Godot.Collections.Dictionary<string, object>()
            {
                { "Number", Number },
                { "Name", Name },
                { "HunterName", HunterName },
                { "PalicoName", PalicoName },
            };
    }
}
