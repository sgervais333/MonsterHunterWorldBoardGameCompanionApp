using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Array = Godot.Collections.Array;

namespace MonsterHunterWorldBoardGameCompanionApp.Scripts.Data
{
    public interface IDictionable
    {
        Godot.Collections.Dictionary<string, object> ToDict();
    }

    public static class EnumerableExtension
    {
        public static Array ToGodotArray(this IEnumerable<IDictionable> list)
        {
            return new Array(list.Select(s => s.ToDict()));
        }
    }
}
