using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.David.Scripts
{
    public class BuildApp : MonoBehaviour
    {

        const string MENU = "XRSpatioTemporalAuthoring/Build/";

        // Line
        // space by increment of 10 to add an empty line
        //[MenuItem("[ DEVA ]/--------------------------", false, 110)]
        //[MenuItem("[ DEVA ]/---------------------------", false, 210)]

        [MenuItem(MENU + "Reset Minor Version Number", false, 10)]
        public static void Reset_Minor()
        {
            string[] ver = PlayerSettings.bundleVersion.Split(".");
            newVer = $"{ver[0]}.{ver[1]}.0";

            PlayerSettings.bundleVersion = newVer;
        }

        [MenuItem(MENU + "Build Android APK #&c", false, 100)]
        public static void Build_Android_APK()
        {
            EditorUserBuildSettings.buildAppBundle = false;
            Android_BuildNow();
        }

        [MenuItem(MENU + "Build Android PlayStore", false, 100)]
        public static void Build_Android_AAB()
        {
            EditorUserBuildSettings.buildAppBundle = true;
            Android_BuildNow();
        }

        [MenuItem(MENU + "Build iOS XCode", false, 200)]
        public static void Build_iOS_XCODE()
        {
            EditorUserBuildSettings.buildAppBundle = true;
            iOS_BuildNow();
        }

        static string baseName = "XRSpatio-Temporal";
        static string newVer;
        static int newCode;
        static string[] levels;

        private static void Init(bool incVersion)
        {
            string[] ver = PlayerSettings.bundleVersion.Split(".");
            if (incVersion) newVer = $"{ver[0]}.{ver[1]}.{int.Parse(ver[2]) + 1}";
            else newVer = $"{ver[0]}.{ver[1]}.{int.Parse(ver[2])}";

            //newCode = PlayerSettings.VersionCode ??

            string scenes = "Assets/Scenes/";
            //string scenesGPS = "Assets/03_ThirdParty/ARLocation/Samples/";
            levels = new string[] {
            scenes + "Project Scene.unity",
            //scenesGPS + "ARLocation 3D Text.unity"
        };
            Debug.Log("Tot levels: " + levels.Length);
            Debug.Log("DEVA Version: " + newVer);

            //PlayerSettings.productName = "";
            PlayerSettings.bundleVersion = newVer;
            PlayerSettings.keystorePass = "FraunhoferHHI10587";
            PlayerSettings.keyaliasPass = "FraunhoferHHI10587";
        }

        public static void Android_BuildNow()
        {
            string type = "";

            //
            // Remember last selected scene object
            //
/*            GameObject lastSelected = Selection.activeGameObject; // don't work
            lastSelected = SceneAsset.FindFirstObjectByType<Compair.CompAirManager>().gameObject;
*/
            if (EditorUserBuildSettings.buildAppBundle)
            {
                Init(false);
                //
                // Increment the bundle nr. for Play Store
                //
                PlayerSettings.Android.bundleVersionCode++;
                type = "aab";
            }
            else
            {
                Init(true);
                type = "apk";
            }

            int release = PlayerSettings.Android.bundleVersionCode;

            //PlayerSettings.SetIl2CppCodeGeneration

            //
            // Get filename/folder
            //
            string path = EditorUtility.SaveFilePanel("Choose Location for Android Built v" + newVer + " R" + release + " Type: " + type,
                "../Builds", baseName + "_build", type);

            if (!string.IsNullOrEmpty(path))
            {
                //
                // Build player
                //
                string finalPath = Path.Combine(Path.GetDirectoryName(path), baseName + "." + type);
                FileUtil.DeleteFileOrDirectory(finalPath); // delete last one (so no override window appears ;-)

                BuildPlayerOptions opt = new BuildPlayerOptions();
                opt.locationPathName = path;
                opt.target = BuildTarget.Android;
                opt.scenes = levels;
                opt.options = BuildOptions.None;

                BuildPipeline.BuildPlayer(opt);

                if (File.Exists(path)) FileUtil.MoveFileOrDirectory(path, finalPath);

                //string pathForSylR = "Dieser PC\\SylR\\Interner Speicher\\Download";
                //if (Directory.Exists(pathForSylR)) FileUtil.CopyFileOrDirectory(finalPath, Path.Combine(pathForSylR, baseName + "." + mode));
            }

/*            //
            // Restore selected scene object
            //
            UnityEditor.EditorGUIUtility.PingObject(lastSelected);
            UnityEditor.Selection.activeGameObject = lastSelected;*/
        }

        public static void iOS_BuildNow(bool incVersion = true)
        {
            Init(incVersion);

            // Get folder name.
            string path = EditorUtility.SaveFolderPanel("Choose Location for iOS Built v" + newVer + " Mode: IOS",
                "../IMC_CompAir_Unity-EXE", baseName);

            if (!string.IsNullOrEmpty(path))
            {
                //
                // Build player
                //
                BuildPlayerOptions opt = new BuildPlayerOptions();
                opt.locationPathName = path;
                opt.target = BuildTarget.iOS;
                opt.scenes = levels;
                opt.options = BuildOptions.None;

                BuildPipeline.BuildPlayer(opt);
            }
        }

    }
}

   

