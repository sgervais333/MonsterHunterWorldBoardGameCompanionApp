using Godot;
using MonsterHunterWorldBoardGameCompanionApp.Scenes.AutoLoad;

namespace MonsterHunterWorldBoardGameCompanionApp.Scripts.Extensions
{
    public static class NodeExtension
    {
        public static Database SaveGameData(this Node node) => node.GetNode<Database>($"/root/{nameof(Database)}");
    }
}
