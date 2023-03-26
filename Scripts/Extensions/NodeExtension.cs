using Godot;
using MonsterHunterWorldBoardGameCompanionApp.Scenes.AutoLoad;

namespace MonsterHunterWorldBoardGameCompanionApp.Scripts.Extensions
{
    public static class NodeExtension
    {
        public static Database DataBase(this Node node) => node.GetNode<Database>($"/root/{nameof(Database)}");
        public static CampaignData GetCampaignData(this Node node) => node.GetNode<CampaignData>($"/root/{nameof(CampaignData)}");
        public static TopBar GetTopBar(this Node node) => node.GetNode<TopBar>($"/root/{nameof(TopBar)}");
        public static void ChangeScene(this Node node, string target, bool addBreadcrumbScenesPath = true, bool removeLastBreadcrumbScenesPath = false, bool transition = false)
        {
            node.GetNode<SceneTransition>($"/root/{nameof(SceneTransition)}").ChangeScene(target, transition);
            if (removeLastBreadcrumbScenesPath) node.GetTopBar().RemoveLastScene();
            if (addBreadcrumbScenesPath) node.GetTopBar().AddScene(target);
        }
    }


}
