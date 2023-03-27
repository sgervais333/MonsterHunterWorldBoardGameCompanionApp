using Godot;
using MonsterHunterWorldBoardGameCompanionApp.Scripts.Data;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;

public class CampaignData : Node, IDictionable
{
    public const string CampaignFolder = "user://campaign";

    public bool Loaded = false;
    public string CampaignName;
    public int NumberOfPotions;
    public List<Player> Players;

    public override void _Ready()
    {
        Players = new List<Player>();
    }

    public void CreateDirectoryIfNotExist()
    {
        Directory dir = new Directory();
        if (dir.DirExists(CampaignFolder)) return;
        dir.Open("user://");
        dir.MakeDir("campaign");

    }

    public void LoadCampaign(Dictionary dict)
    {
        CampaignName = dict.Contains("CampaignName") ? (string)dict["CampaignName"] : null;
        NumberOfPotions = (int)(dict.Contains("NumberOfPotions") ? (float)dict["NumberOfPotions"] : 0f);
        Players = (from Dictionary item in (Array)dict["Players"] select new Player(item)).ToList();
        Loaded = true;
    }

    public void SaveCampaign(string campaignName = null)
    {
        if (CampaignName != null || campaignName != null)
        {
            string campaignToSave = campaignName ?? CampaignName;
            CampaignName = campaignToSave;
            CreateDirectoryIfNotExist();

            File file = new File();
            file.Open($"{CampaignFolder}/{campaignToSave}", File.ModeFlags.WriteRead);
            file.StoreLine(JSON.Print(ToDict()));
            file.Close();
            Loaded = true;
        }
        else
        {
            GD.PrintErr("MustHaveACampaignLoaded");
        }
    }

    public Godot.Collections.Dictionary<string, object> ToDict() =>
        new Godot.Collections.Dictionary<string, object>
        {
            {nameof(CampaignName), CampaignName},
            {nameof(NumberOfPotions), NumberOfPotions},
            {nameof(Players), Players.ToGodotArray()},
        };


}
