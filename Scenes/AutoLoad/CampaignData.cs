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
        NumberOfPotions = (int)(dict.Contains("NumberOfPotions") ? (float)dict["NumberOfPotions"] : 0f);
        Players = (from Dictionary item in (Array)dict["Players"] select new Player(item)).ToList();
        Loaded = true;
    }

    public void SaveCampaign(string campaignName)
    {
        CampaignName = campaignName;
        CreateDirectoryIfNotExist();

        File file = new File();
        file.Open($"{CampaignFolder}/{campaignName}", File.ModeFlags.WriteRead);
        file.StoreLine(JSON.Print(ToDict()));
        file.Close();
        Loaded = true;
    }

    public Godot.Collections.Dictionary<string, object> ToDict() =>
        new Godot.Collections.Dictionary<string, object>
        {
            {nameof(NumberOfPotions), NumberOfPotions},
            {nameof(Players), Players.ToGodotArray()},
        };


}
