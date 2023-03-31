using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Godot;
using Godot.Collections;
using MonsterHunterWorldBoardGameCompanionApp.Scripts;
using MonsterHunterWorldBoardGameCompanionApp.Scripts.Extensions;
using Directory = Godot.Directory;
using File = Godot.File;

public class LoadCampaign : Control
{
    private ItemList _itemList;
    private Button _loadButton;
    private Button _deleteButton;
    private string _selectedItem;
    private int _selectedIndex;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _itemList = GetNode<ItemList>("MarginContainer/VBoxContainer/Control/ItemList");
        _loadButton = GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer/LoadButton");
        _deleteButton = GetNode<Button>("MarginContainer/VBoxContainer/DeleteContainer/DeleteButton");
        foreach (string file in Utils.FilesInDirectory(CampaignData.CampaignFolder))
        {
            _itemList.AddItem(file);
        }
        
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public void LoadCampaignFile(string fileName)
    {
        File file = new File();
        file.Open($"{CampaignData.CampaignFolder}/{fileName}", File.ModeFlags.Read);
        JSONParseResult jsonResult = JSON.Parse(file.GetLine());
        if (!(jsonResult.Result is Dictionary response)) return;
        this.GetCampaignData().LoadCampaign(response);
        file.Close();
    }

    public void DeleteCampaignFile(string fileName)
    {
        Directory dir = new Directory();
        dir.Remove($"{CampaignData.CampaignFolder}/{fileName}");
    }

    public void _on_LoadButton_pressed()
    {
        if (_selectedItem == string.Empty) return;
        LoadCampaignFile(_selectedItem);
        this.ChangeScene("res://Screens/MainCampaign.tscn", removeLastBreadcrumbScenesPath: true);
    }

    public void _on_DeleteButton_pressed()
    {
        DeleteCampaignFile(_selectedItem);
        _itemList.RemoveItem(_selectedIndex);
        _loadButton.Disabled = true;
        _deleteButton.Disabled = true;
    }

    public void _on_ItemList_item_selected(int index)
    {
        _selectedIndex = index;
        _selectedItem = (string)_itemList.Items[index*3];
        _loadButton.Disabled = false;
        _deleteButton.Disabled = false;
    }
}
