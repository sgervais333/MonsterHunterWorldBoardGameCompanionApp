using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterWorldBoardGameCompanionApp.Scripts.Extensions
{
    public static class GodotExtension
    {

        public static Dictionary<int, T> ToSystemDict<T>(this Godot.Collections.Dictionary godotDictionary) where T : struct
        {
            return new Dictionary<string, object>(
                    new Godot.Collections.Dictionary<string, object>(
                        godotDictionary))
                .ToDictionary(k => int.Parse(k.Key), v =>
                {
                    float f = (float)v.Value;
                    return (T)Convert.ChangeType(f, typeof(T));
                });
        }

    }
}
