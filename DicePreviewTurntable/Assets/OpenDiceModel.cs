using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using Dummiesman;
using System.IO;

[RequireComponent(typeof(Button))]
public class OpenDiceModel : MonoBehaviour, IPointerDownHandler {
    public RawImage preview;
    public Renderer m_Renderer;
    GameObject loadedObject;
    public GameObject parentObject;
    private string[] objPath;
    public Material diceMat;
    private GameObject dice;

    public void OnPointerDown(PointerEventData eventData) { }

    void Start() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "obj", false);
        if (paths.Length > 0) {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
        objPath = paths;
    }

    private IEnumerator OutputRoutine(string url) {
        var loader = new WWW(url);
        yield return loader;
        loadedObject = new OBJLoader().Load(objPath[0]);
        loadedObject.transform.SetParent(parentObject.transform, false);
        
        dice = parentObject.gameObject.transform.GetChild(0).GetChild(0).gameObject;
        dice.GetComponent<Renderer>().material = diceMat;

    }
}