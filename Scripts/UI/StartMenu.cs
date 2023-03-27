using Godot;
using MonsterHunterWorldBoardGameCompanionApp.Scripts.Extensions;
using System;

public class StartMenu : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (this.GetCampaignData().Loaded)
        {
            Node buttonsNode = GetNode("MarginContainer/VBoxContainer");
            Button continueButton = new Button
            {
                Name = "Continue Campaign",
                Text = "Continue Campaign",
            };
            buttonsNode.AddChild(continueButton);
            buttonsNode.MoveChild(continueButton, 1);
            continueButton.Connect("pressed", this, nameof(_on_Continue_Campaign_pressed));
            buttonsNode.Owner = Owner;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public void _on_New_Game_pressed()
    {
        this.ChangeScene("res://Screens/CreateCampaign.tscn");
    }

    public void _on_Load_Game_pressed()
    {
        this.ChangeScene("res://Screens/LoadCampaign.tscn");
    }

    public void _on_Continue_Campaign_pressed()
    {
        this.ChangeScene("res://Screens/MainCampaign.tscn");
    }
}
