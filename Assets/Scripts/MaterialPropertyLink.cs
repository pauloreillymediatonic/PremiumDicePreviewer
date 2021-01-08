using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialPropertyLink : MonoBehaviour
{

    public GameObject diceCore;
    public Renderer[] renderers;
    private List<Material> materials = new List<Material>();
    public string floatName;
    public float value;
    public string toggleName;
    private float toggleValue = 1;

    public Slider slider;
    public Toggle toggle;

    private int signChange = 1;

    // Update is called once per frame
    void Update()
    {

    }


    public void SetMaterialToggle(float val)
    {
        renderers = diceCore.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            if (!materials.Contains(renderer.sharedMaterial))
                materials.Add(renderer.sharedMaterial);
        }

        foreach (Material material in materials)
        {
            material.SetFloat(floatName, val);
            if (val > 0)
            {
                material.SetFloat(toggleName, 1);
            }
            else
            {
                material.SetFloat(toggleName, 0);
            }
        }

        print(toggleValue);
    }
}
