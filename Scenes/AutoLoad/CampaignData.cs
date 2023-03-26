using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using MonsterHunterWorldBoardGameCompanionApp.Scripts.Data;
using Array = Godot.Collections.Array;

public class CampaignData : Node
{
    private const string CampaignFolder = "user://campaign";

    public string CampaignName;
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

    public void SaveCampaign(string campaignName)
    {
        CampaignName = campaignName;
        CreateDirectoryIfNotExist();

        File file = new File();
        file.Open($"{CampaignFolder}/{campaignName}", File.ModeFlags.WriteRead);

        foreach (Player player in Players)
        {
            file.StoreLine(JSON.Print(player.AsDict()));
        }
        file.Close();
    }

}
