using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using MonsterHunterWorldBoardGameCompanionApp.Scripts.Data;
using MonsterHunterWorldBoardGameCompanionApp.Scripts.Extensions;
using Array = Godot.Collections.Array;

public class CreateCampaign : Control
{
    private const string PlayerPath = "MarginContainer/VBoxContainer/";

    private Array<PanelContainer> _playersContainer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _playersContainer = new Array<PanelContainer>
        {
            GetNode<PanelContainer>("MarginContainer/ScrollContainer/VBoxContainer/Players/Player1")
        };
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    public void AddPlayer()
    {
        if (_playersContainer.First().Duplicate(0) is PanelContainer panelContainer)
        {
            _playersContainer.Add(panelContainer);
            panelContainer.Name = $"{_playersContainer.Count}";
            Button buttonDelete = panelContainer.GetNode<Button>($"{PlayerPath}HSplitContainer/Delete");
            Label playerLabel = panelContainer.GetNode<Label>($"{PlayerPath}HSplitContainer/Player");
            panelContainer.GetNode<LineEdit>($"{PlayerPath}/PlayerName/LineEdit").Text = string.Empty;
            panelContainer.GetNode<LineEdit>($"{PlayerPath}/HunterName/LineEdit").Text = string.Empty;
            panelContainer.GetNode<LineEdit>($"{PlayerPath}/PalicoName/LineEdit").Text = string.Empty;
            buttonDelete.Disabled = false;
            playerLabel.Text = $"Player {_playersContainer.Count}";
            _playersContainer[0].GetParent().AddChild(panelContainer);
            buttonDelete.Connect("pressed", this, nameof(_on_RemovePlayer_pressed), new Array(panelContainer));
            panelContainer.Owner = Owner;
        }
    }

    private void RemovePlayer(PanelContainer playerContainer)
    {
        playerContainer.QueueFree();
        _playersContainer.Remove(playerContainer);
    }

    public void _on_RemovePlayer_pressed(PanelContainer playerContainer)
    {
        RemovePlayer(playerContainer);
    }

    public void _on_AddPlayer_pressed()
    {
        AddPlayer();
    }

    public List<Player> GetPlayers()
    {
        List<Player> players = new List<Player>();
        int i = 1;
        foreach (PanelContainer playerContainer in _playersContainer)
        {
            string name = playerContainer.GetNode<LineEdit>($"{PlayerPath}/PlayerName/LineEdit").Text;
            string hunterName = playerContainer.GetNode<LineEdit>($"{PlayerPath}/HunterName/LineEdit").Text;
            string palicoName = playerContainer.GetNode<LineEdit>($"{PlayerPath}/PalicoName/LineEdit").Text;
            players.Add(new Player(i, name, hunterName, palicoName));
            i++;
        }

        return players;
    }

    public void _on_CreateCampaign_pressed()
    {
        string campaignName = GetNode<LineEdit>("MarginContainer/ScrollContainer/VBoxContainer/HBoxContainer/Campaign/LineEdit").Text;

        CampaignData campaignData = this.GetCampaignData();
        campaignData.Players = GetPlayers();

        campaignData.SaveCampaign(campaignName);
    }
}
