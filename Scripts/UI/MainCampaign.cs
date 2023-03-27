using System;
using Godot;
using MonsterHunterWorldBoardGameCompanionApp.Scripts.Data;
using MonsterHunterWorldBoardGameCompanionApp.Scripts.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Array = Godot.Collections.Array;
using Material = MonsterHunterWorldBoardGameCompanionApp.Scripts.Data.Material;

public class MainCampaign : Control
{
    private CampaignData _campaignData;
    private Label _numberOfPotionsLabel;
    private OptionButton _optionButtonPlayer;
    private Control _itemTemplate;
    private Control _commonItemsContainer;
    private Control _otherItemsContainer;
    private Control _commonItemsListContainer;
    private Control _otherItemsListContainer;
    private IReadOnlyDictionary<int, Material> _commonMaterials;
    private IReadOnlyDictionary<int, Material> _otherMaterials;
    private IReadOnlyDictionary<int, Material> _monsterPartsMaterials;

    public override void _Ready()
    {
        _campaignData = this.GetCampaignData();
        _numberOfPotionsLabel = GetNode<Label>("MarginContainer/VBoxContainer/Control/HBoxContainer/TextureRect/NumberOfPotions");
        _optionButtonPlayer = GetNode<OptionButton>("MarginContainer/VBoxContainer/OptionButtonPlayer");
        //TODO: Devrait etre unique par container
        _itemTemplate = GetNode<Control>("MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/CommonItems/ScrollContainer/PanelContainer/Templates/ItemTemplate");
        _commonItemsContainer = GetNode<Control>("MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/CommonItems");
        _commonItemsListContainer = GetNode<Control>("MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/CommonItems/ScrollContainer/PanelContainer/Common");
        _otherItemsListContainer = GetNode<Control>("MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/OtherItems/ScrollContainer/PanelContainer/Other");
        _otherItemsContainer = GetNode<Control>("MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/OtherItems");

        foreach (Player campaignDataPlayer in _campaignData.Players)
        {
            _optionButtonPlayer.AddItem(campaignDataPlayer.Name, campaignDataPlayer.Number);
        }

        _commonMaterials = new ReadOnlyDictionary<int, Material>(this.DataBase().Materials.Where(w => w.Type == "Common")
                .ToDictionary(k => k.Id));
        _otherMaterials = new ReadOnlyDictionary<int, Material>(this.DataBase().Materials.Where(w => w.Type == "Other")
            .ToDictionary(k => k.Id));
        _monsterPartsMaterials = new ReadOnlyDictionary<int, Material>(this.DataBase().Materials.Where(w => w.Type == "MonsterParts")
            .ToDictionary(k => k.Id));
        SetInfoForPlayer(_optionButtonPlayer.Selected);

        _numberOfPotionsLabel.Text = _campaignData.NumberOfPotions.ToString();
    }

    private void SetInfoForPlayer(int idPlayer)
    {
        foreach (Node child in _commonItemsListContainer.GetChildren())
        {
            child.QueueFree();
        }
        foreach (Material material in _commonMaterials.Values)
        {
            Player player = this.GetCampaignData().Players[idPlayer];
            int commonItemQty = 0;
            if (player.CommonItems.ContainsKey(material.Id)) commonItemQty = player.CommonItems[material.Id];
            if (_itemTemplate.Duplicate(0) is Control item)
            {
                item.Name = $"{material.Name}";
                item.Visible = true;
                item.GetNode<Label>("Name").Text = material.Name;
                Texture texture = ResourceLoader.Load($"res://Ressources/Items/{(!string.IsNullOrEmpty(material.Image) ? material.Image : "Unknown")}.png") as Texture;
                if (material.Image != string.Empty) item.GetNode<TextureRect>("Icon").Texture = texture;
                Label qtyLabel = item.GetNode<Label>("MarginContainer/Number");
                qtyLabel.Text = commonItemQty.ToString();

                Button plusButton = item.GetNode<Button>("MarginContainer/PlusButton");
                plusButton.Connect("pressed", this, nameof(_on_CommonItemPlus_pressed), new Array(qtyLabel, material.Id));
                Button minusButton = item.GetNode<Button>("MarginContainer/MinusButton");
                minusButton.Connect("pressed", this, nameof(_on_CommonItemMinus_pressed), new Array(qtyLabel, material.Id));
                _commonItemsListContainer.AddChild(item);
                _commonItemsListContainer.Owner = Owner;
            }
        }
        //TODO: Centraliser
        foreach (Material material in _otherMaterials.Values)
        {
            Player player = this.GetCampaignData().Players[idPlayer];
            int otherItemQty = 0;
            if (player.OtherItems.ContainsKey(material.Id)) otherItemQty = player.OtherItems[material.Id];
            if (_itemTemplate.Duplicate(0) is Control item)
            {
                item.Name = $"{material.Name}";
                item.Visible = true;
                item.GetNode<Label>("Name").Text = material.Name;
                Texture texture = ResourceLoader.Load($"res://Ressources/Items/{(!string.IsNullOrEmpty(material.Image) ? material.Image : "Unknown")}.png") as Texture;
                if (material.Image != string.Empty) item.GetNode<TextureRect>("Icon").Texture = texture;
                Label qtyLabel = item.GetNode<Label>("MarginContainer/Number");
                qtyLabel.Text = otherItemQty.ToString();

                Button plusButton = item.GetNode<Button>("MarginContainer/PlusButton");
                plusButton.Connect("pressed", this, nameof(_on_OtherItemPlus_pressed), new Array(qtyLabel, material.Id));
                Button minusButton = item.GetNode<Button>("MarginContainer/MinusButton");
                minusButton.Connect("pressed", this, nameof(_on_OtherItemMinus_pressed), new Array(qtyLabel, material.Id));
                _otherItemsListContainer.AddChild(item);
                _otherItemsListContainer.Owner = Owner;
            }
        }
    }

    private void UpdateCampaign()
    {
        this.GetCampaignData().SaveCampaign();
    }

    public void _on_PotionMinus_pressed()
    {
        if (_campaignData.NumberOfPotions == 0) return;
        _campaignData.NumberOfPotions--;
        _numberOfPotionsLabel.Text = _campaignData.NumberOfPotions.ToString();
        UpdateCampaign();
    }

    public void _on_PotionPlus_pressed()
    {
        if (_campaignData.NumberOfPotions == 3) return;
        _campaignData.NumberOfPotions++;
        _numberOfPotionsLabel.Text = _campaignData.NumberOfPotions.ToString();
        UpdateCampaign();
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
        UpdateCampaign();
    }

    public void _on_CommonItemMinus_pressed(Label qtyLabel, int id)
    {
        Material material = _commonMaterials[id];
        Player player = this.GetCampaignData().Players[_optionButtonPlayer.Selected];
        if (player.CommonItems.ContainsKey(material.Id))
        {
            if (player.CommonItems[material.Id] > 0)
            {
                player.CommonItems[material.Id]--;
                qtyLabel.Text = player.CommonItems[material.Id].ToString();
            }
        }
        else
        {
            player.CommonItems.Add(material.Id, 0);
            qtyLabel.Text = 0.ToString();
        }
        UpdateCampaign();
    }

    //Centraliser
    public void _on_OtherItemPlus_pressed(Label qtyLabel, int id)
    {
        Material material = _otherMaterials[id];
        Player player = this.GetCampaignData().Players[_optionButtonPlayer.Selected];
        if (player.OtherItems.ContainsKey(material.Id))
        {
            player.OtherItems[material.Id]++;
            qtyLabel.Text = player.OtherItems[material.Id].ToString();
        }
        else
        {
            player.OtherItems.Add(material.Id, 1);
            qtyLabel.Text = 1.ToString();
        }
        UpdateCampaign();
    }

    //Centraliser
    public void _on_OtherItemMinus_pressed(Label qtyLabel, int id)
    {
        Material material = _otherMaterials[id];
        Player player = this.GetCampaignData().Players[_optionButtonPlayer.Selected];
        if (player.OtherItems.ContainsKey(material.Id))
        {
            if (player.OtherItems[material.Id] > 0)
            {
                player.OtherItems[material.Id]--;
                qtyLabel.Text = player.OtherItems[material.Id].ToString();
            }
        }
        else
        {
            player.OtherItems.Add(material.Id, 0);
            qtyLabel.Text = 0.ToString();
        }
        UpdateCampaign();
    }

    public void _on_OptionButtonPlayer_item_selected(int index)
    {
        SetInfoForPlayer(index);
    }

    public void _on_CommonExpand_toggled(bool buttonPressed)
    {
        _commonItemsListContainer.Visible = buttonPressed;
        _commonItemsContainer.SizeFlagsVertical = 0;
        if (buttonPressed) _commonItemsContainer.SizeFlagsVertical += (int)SizeFlags.ExpandFill;
        if (!buttonPressed) _commonItemsContainer.SizeFlagsVertical -= (int)SizeFlags.ExpandFill;
    }

    public void _on_OtherExpand_toggled(bool buttonPressed)
    {
        _otherItemsListContainer.Visible = buttonPressed;
        _otherItemsContainer.SizeFlagsVertical = 0;
        if (buttonPressed) _otherItemsContainer.SizeFlagsVertical += (int)SizeFlags.ExpandFill;
        if (!buttonPressed) _otherItemsContainer.SizeFlagsVertical -= (int)SizeFlags.ExpandFill;
    }
    
}
