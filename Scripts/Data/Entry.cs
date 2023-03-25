using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot.Collections;

namespace MonsterHunterWorldBoardGameCompanionApp.Scripts.Data
{
    public  class Entry
    {
        public int Id;
        public string Name;

        public Entry(Dictionary dict)
        {
            Id = (int)(dict.Contains("id") ? (float)dict["id"] : 0f);
            Name = dict.Contains("name") ? (string)dict["name"] : null;
        }
    }
}
