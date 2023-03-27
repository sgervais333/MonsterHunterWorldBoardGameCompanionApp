using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using MonsterHunterWorldBoardGameCompanionApp.Scripts.Data;
using MonsterHunterWorldBoardGameCompanionApp.Scripts.Extensions;
using System.Linq;
using Godot.Collections;
using MonsterHunterWorldBoardGameCompanionApp.Scenes.AutoLoad;
using Array = Godot.Collections.Array;
using Material = MonsterHunterWorldBoardGameCompanionApp.Scripts.Data.Material;

public class MainCampaign : Control
{
    private CampaignData _campaignData;
    private Label _numberOfPotionsLabel;
    private OptionButton _optionButtonPlayer;
    private Control _itemTemplate;
    private Control _itemsContainer;
    private IReadOnlyDictionary<int, Material> _commonMaterials;

    public override void _Ready()
    {
        _campaignData = this.GetCampaignData();
        _numberOfPotionsLabel = GetNode<Label>("MarginContainer/VBoxContainer/Control/HBoxContainer/TextureRect/NumberOfPotions");
        _optionButtonPlayer = GetNode<OptionButton>("MarginContainer/VBoxContainer/OptionButtonPlayer");
        _itemTemplate = GetNode<Control>("MarginContainer/VBoxContainer/ScrollContainer/PanelContainer/Templates/ItemTemplate");
        _itemsContainer = GetNode<Control>("MarginContainer/VBoxContainer/ScrollContainer/PanelContainer/Common");

        foreach (Player campaignDataPlayer in _campaignData.Players)
        {
            _optionButtonPlayer.AddItem(campaignDataPlayer.Name, campaignDataPlayer.Number);
        }

        SetInfoForPlayer(_optionButtonPlayer.Selected);

        _commonMaterials = new ReadOnlyDictionary<int, Material>(this.DataBase().Materials.Where(w => w.Type == "Common")
                .ToDictionary(k => k.Id));
        
        _numberOfPotionsLabel.Text = _campaignData.NumberOfPotions.ToString();
    }

    private void SetInfoForPlayer(int idPlayer)
    {
        foreach (Node child in _itemsContainer.GetChildren())
        {
            child.QueueFree();
        }
        foreach (Material material in this.DataBase().Materials.Where(w => w.Type == "Common"))
        {
            Player player = this.GetCampaignData().Players[idPlayer];//Player player1 = this.GetCampaignData().Players[_optionButtonPlayer.Selected];
            int commonItemQty = 0;
            if (player.CommonItems.ContainsKey(material.Id)) commonItemQty = player.CommonItems[material.Id];
            if (_itemTemplate.Duplicate(0) is Control item)
            {
                item.Name = $"{material.Name}";
                item.Visible = true;
                item.GetNode<Label>("Name").Text = material.Name;
                Texture texture = ResourceLoader.Load($"res://Ressources/Items/{material.Image}.png") as Texture;
                if (material.Image != string.Empty) item.GetNode<TextureRect>("Icon").Texture = texture;
                Label qtyLabel = item.GetNode<Label>("MarginContainer/Number");
                qtyLabel.Text = commonItemQty.ToString();

                Button plusButton = item.GetNode<Button>("MarginContainer/PlusButton");
                plusButton.Connect("pressed", this, nameof(_on_CommonItemPlus_pressed), new Array(qtyLabel, material.Id));
                Button minusButton = item.GetNode<Button>("MarginContainer/MinusButton");
                minusButton.Connect("pressed", this, nameof(_on_CommonItemMinus_pressed), new Array(qtyLabel, material.Id));
                _itemsContainer.AddChild(item);
                _itemsContainer.Owner = Owner;
            }
        }
    }

    public void _on_PotionMinus_pressed()
    {
        if (_campaignData.NumberOfPotions == 0) return;
        _campaignData.NumberOfPotions--;
        _numberOfPotionsLabel.Text = _campaignData.NumberOfPotions.ToString();
    }

    public void _on_PotionPlus_pressed()
    {
        if (_campaignData.NumberOfPotions == 3) return;
        _campaignData.NumberOfPotions++;
        _numberOfPotionsLabel.Text = _campaignData.NumberOfPotions.ToString();
    }

    public void _on_CommonItemPlus_pressed(Label qtyLabel, int id)
    {
        Material material = _commonMaterials[id];
        Player player = this.GetCampaignData().Players[_optionButtonPlayer.Selected];
        if (player.CommonItems.ContainsKey(material.Id))
        {
            player.CommonItems[material.Id]++;
            qtyLabel.Text = player.CommonItems[material.Id].ToString();
        }
        else
        {
            player.CommonItems.Add(material.Id, 1);
            qtyLabel.Text = 1.ToString();
        }
    }

    public void _on_CommonItemMinus_pressed(Label qtyLabel, int id)
    {
        Material material = _commonMaterials[id];
        Player player = this.GetCampaignData().Players[_optionButtonPlayer.Selected];
        if (player.CommonItems.ContainsKey(material.Id))
        {
            player.CommonItems[material.Id]--;
            qtyLabel.Text = player.CommonItems[material.Id].ToString();
        }
        else
        {
            player.CommonItems.Add(material.Id, 0);
            qtyLabel.Text = 0.ToString();
        }
    }

    public void _on_OptionButtonPlayer_item_selected(int index)
    {
        SetInfoForPlayer(index);
    }
}
