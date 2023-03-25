using Godot;
using Godot.Collections;
using MonsterHunterWorldBoardGameCompanionApp.Scripts.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using File = Godot.File;
using Material = MonsterHunterWorldBoardGameCompanionApp.Scripts.Data.Material;

namespace MonsterHunterWorldBoardGameCompanionApp.Screens
{
    public class Splash : Node2D
    {
        private HTTPRequest _httpRequest;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _httpRequest = GetNode<HTTPRequest>(nameof(HTTPRequest));
            RequestData();
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {

        }

        public void RequestData()
        {
            _httpRequest.Connect("request_completed", this, nameof(HttpRequestCompleted));
            Error testConnection = _httpRequest.Request("https://raw.githubusercontent.com/sgervais333/MonsterHunterWorldBoardGameCompanionApp/main/Data/Data.json");
            if (testConnection != Error.Ok) {
                GD.PushError("An error occurred in the HTTP request.");
            }
        }

        public void SaveData(byte[] body)
        {
            File file = new File();
            file.Open($"user://data.json", File.ModeFlags.WriteRead);
            file.StoreBuffer(body);
            file.Close();
        }

        public void HttpRequestCompleted(int result, int responseCode, string[] headers, byte[] body)
        {
            SaveData(body);
            LoadJson(body);
        }

        public void LoadJson(byte[] body)
        {
            JSONParseResult jsonResult = JSON.Parse(Encoding.UTF8.GetString(body));
            if (!(jsonResult.Result is Dictionary response)) return;
            List<Material> materials = (from Dictionary item in (Array)response["materials"] select new Material(item)).ToList();
            List<Armor> armors = (from Dictionary item in (Array)response["armors"] select new Armor(item)).ToList();
            List<Weapon> weapons = (from Dictionary item in (Array)response["weapons"] select new Weapon(item)).ToList();
        }
    }
}
