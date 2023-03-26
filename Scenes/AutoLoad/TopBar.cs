using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MonsterHunterWorldBoardGameCompanionApp.Scripts.Extensions;

public class TopBar : CanvasLayer
{
    public List<string> BreadcrumbScenesPath;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BreadcrumbScenesPath = new List<string>();
        BreadcrumbScenesPath.Add(GetTree().CurrentScene.Filename);
    }

    public void AddScene(string stringPath)
    {
        BreadcrumbScenesPath.Add(stringPath);
    }

    public void RemoveLastScene()
    {
        BreadcrumbScenesPath.RemoveAt(BreadcrumbScenesPath.Count - 1);
    }

    public void _on_Back_pressed()
    {
        if (BreadcrumbScenesPath.Count == 1)
        {
            GetTree().Quit();
            return;
        }
        RemoveLastScene();
        string target = BreadcrumbScenesPath.Last();
        this.ChangeScene(target, addBreadcrumbScenesPath: false, removeLastBreadcrumbScenesPath: false);
    }
}
