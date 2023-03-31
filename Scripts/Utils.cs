using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using Godot;

namespace MonsterHunterWorldBoardGameCompanionApp.Scripts
{
    public static class Utils
    {
        public static void a()
        {

        }

        public static Array<string> FilesInDirectory(string directory)
        {
            Array<string> files = new Array<string>();
            Directory dir = new Directory();
            if (!dir.DirExists(directory)) return new Array<string>();
            dir.Open(directory);

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

        public static List<string> ListFilesInDirectory(string directory) => FilesInDirectory(directory).ToList();
    }
}
