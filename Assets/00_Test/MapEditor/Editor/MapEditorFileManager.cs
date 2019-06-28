using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MapEditor
{
    public class MapEditorFileManager : EditorWindow
    {
        [MenuItem("Test/OpenFile")]
        public static void OpenFile()
        {
            string path = EditorUtility.OpenFilePanel("Json 파일 불러오기", "", "json");
            if(path.Length != 0)
            {

            }
        }
    }
}