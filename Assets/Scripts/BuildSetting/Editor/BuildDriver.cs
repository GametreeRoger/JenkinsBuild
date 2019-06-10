using UnityEngine;
using UnityEditor;

namespace SG.Build {
    public class BuildDriver  {
        const string BUILD_CONFIG_PATH = "Assets/BuildSetting/{0}.asset";

        public static void BuildFromCommandLine() {
            var cfgName = GetArg("-sku");
            BuildConfig cfg = AssetDatabase.LoadAssetAtPath<BuildConfig>(string.Format(BUILD_CONFIG_PATH, cfgName));
            if(cfg == null) {
                Debug.LogErrorFormat("{0} not found!", cfgName);
                return;
            }
            cfg.Name = cfgName;
            cfg.BuildNumber = GetArg("-buildNumber");
            cfg.Version = GetArg("-version");
            if(Configure(cfg)) {
                var output = GetArg("-output");
                var scenes = cfg.Scenes;
                BuildPipeline.BuildPlayer(scenes, output, cfg.BuildTarget, BuildOptions.None);
            }
        }

        public static BuildConfig GetConfigFile(string fileName) {
            BuildConfig cfg = AssetDatabase.LoadAssetAtPath<BuildConfig>(string.Format(BUILD_CONFIG_PATH, fileName));
            if(cfg == null) {
                Debug.LogErrorFormat("{0} not found!", fileName);
                return null;
            }
            cfg.Name = fileName;
            return cfg;
        }

        private static string GetArg(string name) {
            var args = System.Environment.GetCommandLineArgs();
            for(int i = 0; i < args.Length; i++) {
                if(args[i] == name && args.Length > i + 1) {
                    return args[i + 1];
                }
            }
            return null;
        }

        public static bool Configure(BuildConfig cfg) {
            if(EditorUserBuildSettings.SwitchActiveBuildTarget(cfg.BuildTargetGroup, cfg.BuildTarget)) {
                Debug.LogFormat("BuildDriver::Configure - Setting build to '{0}'.", cfg.Name);

                // Never show the Made with Unity logo
                PlayerSettings.SplashScreen.showUnityLogo = false;
                string Defines = "";
                if(cfg.Defines != null)
                    Defines = string.Join(";", cfg.Defines);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(cfg.BuildTargetGroup, Defines);
                // PlayerSettings.applicationIdentifier - only works in Editor, not batch mode ... booo! 
                // https://docs.unity3d.com/560/Documentation/ScriptReference/PlayerSettings-applicationIdentifier.html
                PlayerSettings.SetApplicationIdentifier(cfg.BuildTargetGroup, cfg.BundleId);
                // PlayerSettings.bundleVersion = cfg.BundleVersion;

                EditorBuildSettings.scenes = cfg.BuildSettingScenes;

                ConfigureIcons(cfg);
                ConfigurePlatform(cfg);
                PlayerSettings.SplashScreen.show = false;
                Debug.LogFormat("BuildDriver::Configure - Application Identifier: '{0}'.", PlayerSettings.GetApplicationIdentifier(cfg.BuildTargetGroup));
                Debug.LogFormat("BuildDriver::Configure - Bundle Version: '{0}'.", PlayerSettings.bundleVersion);

                return true;
            } else {
                Debug.LogErrorFormat("BuildDriver::Configure - Could not switch platform to '{0}' for build configuration '{1}'.", cfg.BuildTarget, cfg.Name);
                return false;
            }
        }

        private static void ConfigurePlatform(BuildConfig cfg) {
            switch(cfg.BuildTarget) {
                case BuildTarget.iOS: {
                        PlayerSettings.bundleVersion = cfg.Version;
                        PlayerSettings.iOS.buildNumber = cfg.BuildNumber;
                        PlayerSettings.iOS.appleDeveloperTeamID = cfg.AppleDeveloperTeamID;
                        PlayerSettings.iOS.targetOSVersionString = cfg.TargetOSVersionString;
                        PlayerSettings.iOS.cameraUsageDescription = cfg.CameraUsageDescription;
                        PlayerSettings.iOS.locationUsageDescription = cfg.LocationUsageDescription;
                        PlayerSettings.iOS.microphoneUsageDescription = cfg.MicrophoneUsageDescription;
                    }
                    break;
                case BuildTarget.Android: {
                        PlayerSettings.bundleVersion = cfg.Version;
                        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
                        PlayerSettings.Android.bundleVersionCode = cfg.IntBuildNumber;
                        if(cfg.WritePermissionInternal) {
                            PlayerSettings.Android.forceSDCardPermission = false;
                            PlayerSettings.Android.forceInternetPermission = true;
                        } else {
                            PlayerSettings.Android.forceSDCardPermission = true;
                            PlayerSettings.Android.forceInternetPermission = false;
                        }
                        PlayerSettings.Android.keystorePass = cfg.KeyStorePass;
                        PlayerSettings.Android.keyaliasPass = cfg.KeyAliasPass;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void ConfigureIcons(BuildConfig cfg) {
            int[] sizes = PlayerSettings.GetIconSizesForTargetGroup(cfg.BuildTargetGroup);
            Texture2D[] icons = new Texture2D[sizes.Length];

            Texture2D allIconTemplate = cfg.ICON;
            if(allIconTemplate != null) {
                for(int i = 0; i < sizes.Length; i++) {
                    icons[i] = allIconTemplate;
                }
            } else {
                Debug.LogWarning("BuildDriver::ConfigureIcons - ICON not found.");
            }
            PlayerSettings.SetIconsForTargetGroup(cfg.BuildTargetGroup, icons);
        }
    }
}