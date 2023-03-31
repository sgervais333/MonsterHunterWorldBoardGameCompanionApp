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
    private Label _campaignDayTrackerLabel;
    private OptionButton _optionButtonPlayer;
    private Control _itemTemplate;
    private Control _commonItemsContainer;
    private Control _otherItemsContainer;
    private Control _materialItemsContainer;
    private GridContainer _commonItemsListContainer;
    private GridContainer _otherItemsListContainer;
    private GridContainer _materialItemsListContainer;
    private IReadOnlyDictionary<int, Material> _commonMaterials;
    private IReadOnlyDictionary<int, Material> _otherMaterials;
    private IReadOnlyDictionary<int, Material> _monsterPartsMaterials;

    public override void _Ready()
    {
        _campaignData = this.GetCampaignData();
        _numberOfPotionsLabel = GetNode<Label>("MarginContainer/VBoxContainer/Control/HBoxContainer/TextureRect/NumberOfPotions");
        _campaignDayTrackerLabel = GetNode<Label>("MarginContainer/VBoxContainer/Control/HBoxContainer/HBoxContainer/TextureRect/CampaignDayTracker");
        _optionButtonPlayer = GetNode<OptionButton>("MarginContainer/VBoxContainer/OptionButtonPlayer");
        //TODO: Devrait etre unique par container
        _itemTemplate = GetNode<Control>("MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/CommonItems/ScrollContainer/PanelContainer/Templates/ItemTemplate");
        _commonItemsListContainer = GetNode<GridContainer>("MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/CommonItems/ScrollContainer/PanelContainer/Common");
        _commonItemsContainer = GetNode<Control>("MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/CommonItems");
        _otherItemsListContainer = GetNode<GridContainer>("MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/OtherItems/ScrollContainer/PanelContainer/Other");
        _otherItemsContainer = GetNode<Control>("MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/OtherItems");
        _materialItemsListContainer = GetNode<GridContainer>("MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/MaterialItems/ScrollContainer/PanelContainer/Material");
        _materialItemsContainer = GetNode<Control>("MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/MaterialItems");

        _optionButtonPlayer.AddItem("All", -1);
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
        _campaignDayTrackerLabel.Text = _campaignData.CampaignDayTracker.ToString();
        _on_MainCampaign_resized();
    }

    private void SetInfoForPlayer(int idPlayer)
    {
        GenerateItemsList(_commonItemsListContainer, _commonMaterials, _itemTemplate, player => player.CommonItems, 0, idPlayer);
        GenerateItemsList(_otherItemsListContainer, _otherMaterials, _itemTemplate, player => player.OtherItems, 1, idPlayer);
        GenerateItemsList(_materialItemsListContainer, _monsterPartsMaterials, _itemTemplate, player => player.MaterialItems, 2, idPlayer);
    }

    private void GenerateItemsList(Control listContainer, IReadOnlyDictionary<int, Material> materials, Control itemTemplate, Func<Player, Dictionary<int, int>> getPlayerItems, int materialOrOtherOrParts, int idPlayer)
    {
        foreach (Node child in listContainer.GetChildren())
        {
            child.QueueFree();
        }
        foreach (Material material in materials.Values)
        {

            Player[] players = idPlayer == 0 ? this.GetCampaignData().Players.ToArray() : new []{ this.GetCampaignData().Players[idPlayer-1] };

            if (itemTemplate.Duplicate(0) is Control item)
            {
                item.Name = $"{material.Id}";
                item.Visible = true;
                item.GetNode<Label>("Name").Text = material.Name;
                if (material.Image != null && this.DataBase().IconTextures.ContainsKey(material.Image))
                {
                    item.GetNode<TextureRect>("Icon").Texture = this.DataBase().IconTextures[material.Image];
                }
                else if (material.Image != null && this.DataBase().IconTextures.ContainsKey("Unknown"))
                { 
                    item.GetNode<TextureRect>("Icon").Texture = this.DataBase().IconTextures["Unknown"];
                }

                Label qtyLabel = item.GetNode<Label>("MarginContainer/Number");
                List<int> qties = new List<int>();
                foreach (Player player in players)
                {
                    qties.Add(getPlayerItems(player).ContainsKey(material.Id)
                        ? getPlayerItems(player)[material.Id]
                        : 0);
                }

                qtyLabel.Text = string.Join("/", qties);

                Button plusButton = item.GetNode<Button>("MarginContainer/PlusButton");
                plusButton.Connect("pressed", this, nameof(_on_ItemPlus_pressed), new Array(qtyLabel, material.Id, 1, materialOrOtherOrParts));
                Button minusButton = item.GetNode<Button>("MarginContainer/MinusButton");
                minusButton.Connect("pressed", this, nameof(_on_ItemPlus_pressed), new Array(qtyLabel, material.Id, -1, materialOrOtherOrParts));
                listContainer.AddChild(item);
                listContainer.Owner = Owner;
            }
        }
    }

    private void UpdateQty(Control listContainer, Func<Player, Dictionary<int, int>> getPlayerItems, int idPlayer)
    {
        Player[] players = idPlayer == 0 ? this.GetCampaignData().Players.ToArray() : new[] { this.GetCampaignData().Players[idPlayer - 1] };
        foreach (Control control in listContainer.GetChildren())
        {
            Label qtyLabel = control.GetNode<Label>("MarginContainer/Number");
            List<int> qties = new List<int>();
            int materialId = int.Parse(control.Name);
            foreach (Player player in players)
            {
                qties.Add(getPlayerItems(player).ContainsKey(materialId)
                    ? getPlayerItems(player)[materialId]
                    : 0);
            }

            qtyLabel.Text = string.Join("/", qties);
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

    public void _on_CampaignDayTrackerMinus_pressed()
    {
        if (_campaignData.CampaignDayTracker == 0) return;
        _campaignData.CampaignDayTracker--;
        _campaignDayTrackerLabel.Text = _campaignData.CampaignDayTracker.ToString();
        UpdateCampaign();
    }

    public void _on_CampaignDayTrackerPlus_pressed()
    {
        _campaignData.CampaignDayTracker++;
        _campaignDayTrackerLabel.Text = _campaignData.CampaignDayTracker.ToString();
        UpdateCampaign();
    }

    public void _on_ItemPlus_pressed(Label qtyLabel, int id, int qtyToAdd, int materialOrOtherOrParts)
    {
        Player[] players = _optionButtonPlayer.Selected == 0 ? this.GetCampaignData().Players.ToArray() : new[] { this.GetCampaignData().Players[_optionButtonPlayer.Selected - 1] };
        List<int> qties = new List<int>();
        foreach (Player player in players)
        {
            Material material = null;
            Dictionary<int, int> items = null;
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
                    items = player.MaterialItems;
                    break;
                default: return;
            }
            if (items.ContainsKey(material.Id))
            {
                items[material.Id] = items[material.Id] + qtyToAdd >= 0 ? items[material.Id] + qtyToAdd : 0;
                qties.Add(items[material.Id]);
            }
            else
            {
                int q = qtyToAdd > 0 ? 1 : 0;
                items.Add(material.Id, q);
                qties.Add(q);
            }
        }
        qtyLabel.Text = string.Join("/", qties);
        UpdateCampaign();
    }

    public void _on_OptionButtonPlayer_item_selected(int index)
    {
        UpdateQty(_commonItemsListContainer, player => player.CommonItems, index);
        UpdateQty(_otherItemsListContainer, player => player.OtherItems, index);
        UpdateQty(_materialItemsListContainer, player => player.MaterialItems, index);
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

    public void _on_MaterialExpand_toggled(bool buttonPressed)
    {
        _materialItemsListContainer.Visible = buttonPressed;
        _materialItemsContainer.SizeFlagsVertical = 0;
        if (buttonPressed) _materialItemsContainer.SizeFlagsVertical += (int)SizeFlags.ExpandFill;
        if (!buttonPressed) _materialItemsContainer.SizeFlagsVertical -= (int)SizeFlags.ExpandFill;
    }

    public void _on_MainCampaign_resized()
    {
        Vector2 size = GetViewportRect().Size;
        if (_commonItemsListContainer == null || _otherItemsListContainer == null || _materialItemsListContainer == null) return;
        _commonItemsListContainer.Columns = (int)(size.x / 984);
        _otherItemsListContainer.Columns = (int)(size.x / 984);
        _materialItemsListContainer.Columns = (int)(size.x / 984);
    }

}
