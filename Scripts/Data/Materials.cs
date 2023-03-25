using Godot.Collections;

namespace MonsterHunterWorldBoardGameCompanionApp.Scripts.Data
{
    public class Material : Entry
    {
        public string Type;
        public string Image;

        public Material(Dictionary dict) : base(dict)
        {
            Type = dict.Contains("type") ? (string)dict["type"] : null;
            Image = dict.Contains("image") ? (string)dict["image"] : null;
        }
    }
}