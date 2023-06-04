#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.ModernUIPack
{
    public class InitMUIP
    {
        [InitializeOnLoad]
        public class InitOnLoad
        {
            static InitOnLoad()
            {
                if (!EditorPrefs.HasKey("MUIPv5.Installed"))
                {
                    EditorPrefs.SetInt("MUIPv5.Installed", 1);
                    EditorUtility.DisplayDialog("Hello there!", "Thank you for purchasing Modern UI Pack." +
                        "\r\rTo use the UI Manager, go to Tools > Modern UI Pack > Show UI Manager." +
                        "\r\rIf you need help, feel free to contact us through our support channels or Discord.", "Got it!");
                }

                if (!EditorPrefs.HasKey("MUIP.HasCustomEditorData"))
                {
                    EditorPrefs.SetInt("MUIP.HasCustomEditorData", 1);

                    string mainPath = AssetDatabase.GetAssetPath(Resources.Load("MUIP Manager"));
                    mainPath = mainPath.Replace("Resources/MUIP Manager.asset", "").Trim();
                    string darkPath = mainPath + "Skins/MUI Skin Dark.guiskin";
                    string lightPath = mainPath + "Skins/MUI Skin Light.guiskin";

                    EditorPrefs.SetString("MUIP.CustomEditorDark", darkPath);
                    EditorPrefs.SetString("MUIP.CustomEditorLight", lightPath);
                }
            }
        }
    }
}
#endif