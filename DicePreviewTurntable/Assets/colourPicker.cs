using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class colourPicker : MonoBehaviour {

    public GameObject ColorPickedPrefab;
    private ColorPickerTriangle CP;
    private bool isPaint = false;
    private GameObject go;
    public Image img;
    public Renderer rend;
    private Material material;
    public GameObject diceCore;

    void Start()
    {
        
    }

    void Update()
    {
        if (isPaint)
        {
            img.color = CP.TheColor;
            material.SetColor("_RimColor", img.color);
        }
    }

    public void Click()
    {
        if (isPaint)
        {
            StopPaint();
        }
        else
        {
            StartPaint();
        }
    }

    private void StartPaint()
    {
        rend = diceCore.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshRenderer>();
        material = rend.sharedMaterial;
        ColorPickedPrefab.SetActive(true);
        CP = ColorPickedPrefab.GetComponent<ColorPickerTriangle>();
        CP.SetNewColor(img.color);
        
        
        isPaint = true;
    }

    private void StopPaint()
    {
        ColorPickedPrefab.SetActive(false);
        isPaint = false;
        print ("Current rim light colour is " +img.color);
    }
}
