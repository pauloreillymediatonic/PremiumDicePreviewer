using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class colourPicker : MonoBehaviour
{

    public GameObject ColorPickedPrefab;
    private ColorPickerTriangle CP;
    private bool isPaint = false;
    private GameObject go;
    public Image img;
    private MeshRenderer[] renderers;
    private List<Material> materials = new List<Material>();
    public GameObject diceCore;
    public string matAtt;

    void Update()
    {
        if (isPaint)
        {
            img.color = CP.TheColor;
            foreach (Material material in materials)
            {
                material.SetColor(matAtt, img.color);
            }
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
        renderers = diceCore.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in renderers)
        {
            materials.Add(mr.sharedMaterial);
        }
        
        ColorPickedPrefab.SetActive(true);
        CP = ColorPickedPrefab.GetComponent<ColorPickerTriangle>();
        CP.SetNewColor(img.color);

        isPaint = true;
    }

    private void StopPaint()
    {
        ColorPickedPrefab.SetActive(false);
        isPaint = false;
        print("Current rim light colour is " + img.color);
    }
}
