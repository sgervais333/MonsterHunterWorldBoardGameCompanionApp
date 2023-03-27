using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Godot;
using Godot.Collections;
using MonsterHunterWorldBoardGameCompanionApp.Scripts.Extensions;
using Directory = Godot.Directory;
using File = Godot.File;

public class LoadCampaign : Control
{
    private ItemList _itemList;
    private string _selectedItem;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _itemList = GetNode<ItemList>("MarginContainer/VBoxContainer/Control/ItemList");
        foreach (string file in ListFilesInDirectory())
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

    public Array<string> ListFilesInDirectory()
    {
        Array<string> files = new Array<string>();
        Directory dir = new Directory();
        if (!dir.DirExists(CampaignData.CampaignFolder)) return new Array<string>();
        dir.Open(CampaignData.CampaignFolder);

        dir.ListDirBegin();
        while (true)
        {
            string file = dir.GetNext();
            if (file == string.Empty)
            {
                break;
            }
            if (!file.BeginsWith("."))
            {
                files.Add(file);
            }
        }
        dir.ListDirEnd();
        return files;
    }

    public void _on_Button_pressed()
    {
        if (_selectedItem == string.Empty) return;
        LoadCampaignFile(_selectedItem);
        this.ChangeScene("res://Screens/MainCampaign.tscn", removeLastBreadcrumbScenesPath: true);
    }

    public void _on_ItemList_item_selected(int index)
    {
        _selectedItem = (string)_itemList.Items[index*3];
    }
}