using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;

[RequireComponent(typeof(Button))]
public class LoadTextureButton : MonoBehaviour, IPointerDownHandler
{
    public bool isNormalTexture = false;
    public RawImage preview;
    public RawImage replacementImage;
    public GameObject core;
    private MeshRenderer[] renderers;
    private List<Material> materials = new List<Material>();
    private GameObject dice;

    public string textureName;


    public void OnPointerDown(PointerEventData eventData) { }

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "png", false);
        if (paths.Count > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0].Name).AbsoluteUri));
        }
    }

    private IEnumerator OutputRoutine(string url)
    {
        renderers = core.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            if (!materials.Contains(renderer.sharedMaterial))
                materials.Add(renderer.sharedMaterial);
        }

        //dice = core.gameObject.transform.GetChild(0).GetChild(0).gameObject;
        // mat = dice.GetComponent<Renderer>().material;
        var loader = new WWW(url);
        yield return loader;

        Texture2D newTexture = new Texture2D(loader.texture.width, loader.texture.height);
        if(isNormalTexture)
        {
            Color32[] pixels = loader.texture.GetPixels32();
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i].a = pixels[i].r;
                pixels[i].r = 0;
            }
            newTexture.SetPixels32(pixels);
            newTexture.Apply();
        }
        else
        {
            newTexture = loader.texture;
        }

        preview.texture = loader.texture;
        //replacementImage.texture = loader.texture;


        foreach (Material material in materials)
        {
            material.SetTexture(textureName, newTexture);
        }
    }
}