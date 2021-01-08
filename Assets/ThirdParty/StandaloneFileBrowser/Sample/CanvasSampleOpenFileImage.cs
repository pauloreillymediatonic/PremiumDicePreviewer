using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;

[RequireComponent(typeof(Button))]
public class CanvasSampleOpenFileImage : MonoBehaviour, IPointerDownHandler {
    public RawImage preview;
    public RawImage replacementImage;
    public GameObject core;
    private Renderer rend;
    private Material mat;
    private GameObject dice;

    public string textureName;


    public void OnPointerDown(PointerEventData eventData) { }

    void Start() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "png", false);
        if (paths.Length > 0) {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }

    private IEnumerator OutputRoutine(string url) {
        dice = core.gameObject.transform.GetChild(0).GetChild(0).gameObject;
        mat = dice.GetComponent<Renderer>().material;
        var loader = new WWW(url);
        yield return loader;
        preview.texture = loader.texture;
        replacementImage.texture = loader.texture;
        mat.SetTexture(textureName, loader.texture);
    }
}

