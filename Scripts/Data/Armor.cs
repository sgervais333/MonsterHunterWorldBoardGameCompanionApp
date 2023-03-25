using Godot.Collections;

namespace MonsterHunterWorldBoardGameCompanionApp.Scripts.Data
{
    public class Armor : Entry
    {
        public string Type;
        public int Rarity;
        public bool Starter;

        public Armor(Dictionary dict) : base(dict)
        {
            Type = dict.Contains("type") ? (string)dict["type"] : null;
            Rarity = (int)(dict.Contains("rarity") ? (float)dict["rarity"] : 0f);
            Starter = dict.Contains("starter") && (bool)dict["starter"];
        }
    }
}