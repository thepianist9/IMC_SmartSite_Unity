using GLTFast;
using System;
using System.Collections;
using GLTFast.Export;

using UnityEngine;
using UnityEngine.Networking;
using File = System.IO.File;
using GLTFast.Logging;

public class GLTFLoader : MonoBehaviour
{
    [SerializeField] private string localglTF = "Duck.gltf";
    [SerializeField] private string remoteUrl = "https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Duck/glTF/Duck.gltf";


    [SerializeField] bool LoadLocal;
    private string filePath;
    // Start is called before the first frame update
    void Start()
    {
        filePath = Application.persistentDataPath + "/" + localglTF;
        Debug.Log(filePath);
        if (LoadLocal)
        {
            if (!File.Exists(filePath))
            {
                /*StartCoroutine(DownloadAndSave());*/
                DownloadAndExportToLocal();
            }
            else
            {
                LoadGltfBinaryFromMemory();
            }
        }
        else
        {
            LoadGltfBinaryFromHttps();
        }
    }


    private void LoadGltfBinaryFromHttps()
    {
        var gltf = gameObject.AddComponent<GLTFast.GltfAsset>();
        gltf.Url = remoteUrl;


    }

    async void LoadGltfBinaryFromMemory()
    {
        
        Byte[] byteData;
        //if file does not exist


        if (File.Exists(filePath))
        {
            Debug.Log("Loading file from" + filePath);
            byteData = File.ReadAllBytes(filePath);
            var gltfImp = new GltfImport();
            bool success = await gltfImp.LoadGltfBinary(
                byteData,
                // The URI of the original data is important for resolving relative URIs within the glTF
                new Uri(filePath)
                );
            if (success)
            {
                success = await gltfImp.InstantiateMainSceneAsync(transform);
                Debug.Log("Loaded");
            }
        }
        else
        {
            Debug.Log("File not existing");
            //LoadGltfBinaryFromMemory();
        }




    }
   

    async void DownloadAndExportToLocal()
    {

        LoadGltfBinaryFromHttps();
        transform.GetChild(0).gameObject.tag = "gltf";

        var logger = new CollectingLogger();

        // ExportSettings and GameObjectExportSettings allow you to configure the export
        // Check their respective source for details

        // ExportSettings provides generic export settings
        var exportSettings = new ExportSettings
        {
            Format = GltfFormat.Binary,
            FileConflictResolution = FileConflictResolution.Overwrite,
            // Export everything except cameras or animation
            ComponentMask = ~(ComponentType.Camera | ComponentType.Animation),
            // Boost light intensities
            LightIntensityFactor = 100f,
        };

        // GameObjectExportSettings provides settings specific to a GameObject/Component based hierarchy
        var gameObjectExportSettings = new GameObjectExportSettings
        {
            // Include inactive GameObjects in export
            OnlyActiveInHierarchy = false,
            // Also export disabled components
            DisabledComponents = true,
            // Only export GameObjects on certain layers
            LayerMask = LayerMask.GetMask("Default", "MyCustomLayer"),
        };

        // GameObjectExport lets you create glTFs from GameObject hierarchies
        var export = new GameObjectExport(exportSettings, gameObjectExportSettings, logger: logger);

        // Example of gathering GameObjects to be exported (recursively)
        var rootLevelNodes = GameObject.FindGameObjectsWithTag("gltf");

        // Add a scene
        export.AddScene(rootLevelNodes, "My new glTF scene");

        // Async glTF export
        var success = await export.SaveToFileAndDispose(filePath);

        if (!success)
        {
            Debug.LogError("Something went wrong exporting a glTF");
            // Log all exporter messages
            logger.LogAll();
        }
    }
}



