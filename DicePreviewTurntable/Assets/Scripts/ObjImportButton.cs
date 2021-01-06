/*
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;

[RequireComponent(typeof(Button))] 
public class ObjImportButton : FastObjImporter, IPointerDownHandler 
{
	
    private Mesh myMesh;
    public GameObject turntable;

    public void OnPointerDown(PointerEventData eventData) { }

    void Start() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "obj", false);
        print(paths);
        if (paths.Length > 0) {
            StartCoroutine(OutputRoutine(paths));
        }
    }

    private IEnumerator OutputRoutine(string paths) {
        myMesh = FastObjImporter.Instance.ImportFile(paths);
        myMesh.transform.parent = turntable.transform;
    }
}
*/

