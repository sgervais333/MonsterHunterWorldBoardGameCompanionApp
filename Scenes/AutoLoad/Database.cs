using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using Godot.Collections;
using MonsterHunterWorldBoardGameCompanionApp.Scripts.Data;
using Material = MonsterHunterWorldBoardGameCompanionApp.Scripts.Data.Material;

namespace MonsterHunterWorldBoardGameCompanionApp.Scenes.AutoLoad
{
    public class Database : Node
    {
        public List<Material> Materials;
        public List<Armor> Armors;
        public List<Weapon> Weapons;

        private const string DataFile = "user://data.json";
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

        public bool CheckIfDataExist() => new File().FileExists(DataFile);

        public void RequestData()
        {
            if (CheckIfDataExist())
            {
                File file = new File();
                file.Open(DataFile, File.ModeFlags.Read);
                JSONParseResult jsonResult = JSON.Parse(file.GetAsText());
                LoadJson(jsonResult);
                file.Close();
            }
            else
            {
                _httpRequest.Connect("request_completed", this, nameof(HttpRequestCompleted));
                Error testConnection = _httpRequest.Request("https://raw.githubusercontent.com/sgervais333/MonsterHunterWorldBoardGameCompanionApp/main/Data/Data.json");
                if (testConnection != Error.Ok)
                {
                    GD.PushError("An error occurred in the HTTP request.");
                }
            }
        }

        public void SaveData(byte[] body)
        {
            File file = new File();
            file.Open(DataFile, File.ModeFlags.WriteRead);
            file.StoreBuffer(body);
            file.Close();
        }

        public void HttpRequestCompleted(int result, int responseCode, string[] headers, byte[] body)
        {
            SaveData(body);
            LoadJson(JSON.Parse(Encoding.UTF8.GetString(body)));
        }

        public void LoadJson(JSONParseResult jsonResult)
        {
            if (!(jsonResult.Result is Dictionary response)) return;
            Materials = (from Dictionary item in (Array)response["materials"] select new Material(item)).ToList();
            Armors = (from Dictionary item in (Array)response["armors"] select new Armor(item)).ToList();
            Weapons = (from Dictionary item in (Array)response["weapons"] select new Weapon(item)).ToList();
        }
    }
}