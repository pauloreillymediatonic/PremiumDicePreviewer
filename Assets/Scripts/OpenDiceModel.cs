using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using System.IO;
using TriLibCore.General;
using TriLibCore.Extensions;
using TriLibCore;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OpenDiceModel : MonoBehaviour, IPointerDownHandler
{
    public RawImage preview;
    public Renderer m_Renderer;
    GameObject loadedObject;
    public GameObject parentObject;
    private string[] objPath;
    public Material diceMat;
    private GameObject dice;

    Button loaderButton;

    public bool importNormals = false;

    public void OnPointerDown(PointerEventData eventData) { }

    void Start()
    {
        loaderButton = GetComponent<Button>();
        loaderButton.onClick.AddListener(LoadModel);
    }

    private void OnClick()
    {
        /*var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "obj", false);
        if (paths.Length > 0) {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
        objPath = paths;*/
    }

    /// <summary>
    /// Creates the AssetLoaderOptions instance and displays the Model file-picker.
    /// </summary>
    /// <remarks>
    /// You can create the AssetLoaderOptions by right clicking on the Assets Explorer and selecting "TriLib->Create->AssetLoaderOptions->Pre-Built AssetLoaderOptions".
    /// </remarks>
    public void LoadModel()
    {
        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        assetLoaderOptions.ImportMaterials = false;
        assetLoaderOptions.ImportNormals = importNormals;
        var assetLoaderFilePicker = AssetLoaderFilePicker.Create();
        assetLoaderFilePicker.LoadModelFromFilePickerAsync("Select a Model file", OnLoad, OnMaterialsLoad, OnProgress, OnBeginLoad, OnError, null, assetLoaderOptions);
    }

    /// <summary>
    /// Called when the the Model begins to load.
    /// </summary>
    /// <param name="filesSelected">Indicates if any file has been selected.</param>
    private void OnBeginLoad(bool filesSelected)
    {
        loaderButton.interactable = false;
    }

    /// <summary>
    /// Called when any error occurs.
    /// </summary>
    /// <param name="obj">The contextualized error, containing the original exception and the context passed to the method where the error was thrown.</param>
    private void OnError(IContextualizedError obj)
    {
        Debug.LogError($"An error ocurred while loading your Model: {obj.GetInnerException()}");
    }

    /// <summary>
    /// Called when the Model loading progress changes.
    /// </summary>
    /// <param name="assetLoaderContext">The context used to load the Model.</param>
    /// <param name="progress">The loading progress.</param>
    private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
    {
        Debug.Log($"Loading Model. Progress: {progress:P}");
    }

    /// <summary>
    /// Called when the Model (including Textures and Materials) has been fully loaded, or after any error occurs.
    /// </summary>
    /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
    /// <param name="assetLoaderContext">The context used to load the Model.</param>
    private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
    {
        if (assetLoaderContext.RootGameObject != null)
        {
            Debug.Log("Materials loaded. Model fully loaded.");
        }
        else
        {
            Debug.Log("Model could not be loaded.");
        }
    }

    /// <summary>
    /// Called when the Model Meshes and hierarchy are loaded.
    /// </summary>
    /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
    /// <param name="assetLoaderContext">The context used to load the Model.</param>
    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        if (loadedObject != null)
        {
            Destroy(loadedObject);
        }
        loadedObject = assetLoaderContext.RootGameObject;
        if (loadedObject != null)
        {
            Debug.Log("Model loaded. Loading materials.");
        }
        else
        {
            Debug.Log("Model materials could not be loaded.");
        }

        //Camera.main.FitToBounds(loadedObject, 5);

        //Vector3 newPosition = loadedObject.GetComponentsInChildren<> parentObject.transform.position

        MeshRenderer[] renderers = loadedObject.GetComponentsInChildren<MeshRenderer>();

        //Calculate center of the model

        Bounds biggestBounds = new Bounds();

        foreach (MeshRenderer renderer in renderers)
        {
            if(renderer.GetComponent<MeshFilter>().sharedMesh.bounds.size.magnitude > biggestBounds.size.magnitude)
            {
                biggestBounds = renderer.GetComponent<MeshFilter>().sharedMesh.bounds;
            }

            renderer.sharedMaterial = new Material(Shader.Find("Yux/PremiumDice"));
            renderer.material = new Material(Shader.Find("Yux/PremiumDice"));
        }

        Vector3 newPosition =  parentObject.transform.position - biggestBounds.center;

        loadedObject.transform.position = newPosition;

        loadedObject.transform.SetParent(parentObject.transform, false);

        loaderButton.interactable = true;
    }

    /*private IEnumerator OutputRoutine(string url) {
            var loader = new WWW(url);
            yield return loader;
            loadedObject = new OBJLoader().Load(objPath[0]);
            loadedObject.transform.SetParent(parentObject.transform, false);

            dice = parentObject.gameObject.transform.GetChild(0).GetChild(0).gameObject;
            dice.GetComponent<Renderer>().material = diceMat;

        }*/
}