using UnityEditor;
using UnityEngine;
namespace SG.Build {
    [System.Serializable]
    [CreateAssetMenu(menuName = "BuildSetting/Config")]
    public class BuildConfig : ScriptableObject {
        public string Name;
        public string BundleId;
        public BuildTarget BuildTarget;
        public BuildTargetGroup BuildTargetGroup;
        public string Version;
        public string BuildNumber;
        public int IntBuildNumber {
            get {
                int result = 0;
                int.TryParse(BuildNumber, out result);
                return result;
            }
        }
        public string[] Defines;
        public Texture2D ICON;
        public string[] Scenes;
        public EditorBuildSettingsScene[] BuildSettingScenes {
            get {
                if(Scenes == null)
                    return null;
                else {
                    EditorBuildSettingsScene[] tempscenes = new EditorBuildSettingsScene[Scenes.Length];
                    for(int i = 0; i < Scenes.Length; i++) {
                        tempscenes[i] = new EditorBuildSettingsScene(Scenes[i].Trim(' '), true);
                    }
                    return tempscenes;
                }
            }
        }

        //iOS Setting
        public string AppleDeveloperTeamID;
        public string TargetOSVersionString;
        public string CameraUsageDescription;
        public string LocationUsageDescription;
        public string MicrophoneUsageDescription;

        //Android Setting
        public bool WritePermissionInternal;
        public string KeyStorePass;
        public string KeyAliasPass;
    }
}