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
        GenerateItemsList(_commonItemsListContainer, _commonMaterials, _itemTemplate, player => player.CommonItems, 0, idPlayer);
        GenerateItemsList(_otherItemsListContainer, _otherMaterials, _itemTemplate, player => player.OtherItems, 1, idPlayer);
    }

    private void GenerateItemsList(Control listContainer, IReadOnlyDictionary<int, Material> materials, Control itemTemplate, Func<Player, Dictionary<int, int>> getPlayerItems, int materialOrOtherOrParts, int idPlayer)
    {
        foreach (Node child in listContainer.GetChildren())
        {
            child.QueueFree();
        }
        foreach (Material material in materials.Values)
        {
            Player player = this.GetCampaignData().Players[idPlayer];
            int commonItemQty = 0;
            

            if (getPlayerItems(player).ContainsKey(material.Id)) commonItemQty = getPlayerItems(player)[material.Id];
            if (itemTemplate.Duplicate(0) is Control item)
            {
                item.Name = $"{material.Name}";
                item.Visible = true;
                item.GetNode<Label>("Name").Text = material.Name;
                Texture texture = ResourceLoader.Load($"res://Ressources/Items/{(!string.IsNullOrEmpty(material.Image) ? material.Image : "Unknown")}.png") as Texture;
                if (material.Image != string.Empty) item.GetNode<TextureRect>("Icon").Texture = texture;
                Label qtyLabel = item.GetNode<Label>("MarginContainer/Number");
                qtyLabel.Text = commonItemQty.ToString();

                Button plusButton = item.GetNode<Button>("MarginContainer/PlusButton");
                plusButton.Connect("pressed", this, nameof(_on_ItemPlus_pressed), new Array(qtyLabel, material.Id, 1, materialOrOtherOrParts));
                Button minusButton = item.GetNode<Button>("MarginContainer/MinusButton");
                minusButton.Connect("pressed", this, nameof(_on_ItemPlus_pressed), new Array(qtyLabel, material.Id, -1, materialOrOtherOrParts));
                listContainer.AddChild(item);
                listContainer.Owner = Owner;
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

    public void _on_ItemPlus_pressed(Label qtyLabel, int id, int qtyToAdd, int materialOrOtherOrParts)
    {
        Material material = null;
        Dictionary<int, int> items = null;
        Player player = this.GetCampaignData().Players[_optionButtonPlayer.Selected];
        switch (materialOrOtherOrParts)
        {
            case 0: 
                material = _commonMaterials[id];
                items = player.CommonItems;
                break;
            case 1: 
                material = _otherMaterials[id];
                items = player.OtherItems;
                break;
            case 2: 
                material = _monsterPartsMaterials[id];
                items = new Dictionary<int, int>();//items = player.Monster;
                break;
            default: return;
        }
        if (items.ContainsKey(material.Id))
        {
            items[material.Id] = items[material.Id] + qtyToAdd >= 0 ? items[material.Id] + qtyToAdd : 0;
            qtyLabel.Text = items[material.Id].ToString();
        }
        else
        {
            items.Add(material.Id, qtyToAdd > 0 ? 1 : 0);
            qtyLabel.Text = 1.ToString();
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
