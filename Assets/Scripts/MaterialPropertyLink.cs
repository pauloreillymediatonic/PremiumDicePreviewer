using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialPropertyLink : MonoBehaviour
{

	public GameObject diceCore;
	public Renderer rend;
	private Material material;
	public string floatName;
	public float value;
	public string toggleName;
	private float toggleValue = 1;

	public Slider slider;
	public Toggle toggle;

	private int signChange = 1;
    // Start is called before the first frame update
    void Start()
    {
    	

    }

    // Update is called once per frame
    void Update()
    {
        //rend = diceCore.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshRenderer>();
        //material = rend.material;
        value = slider.value;
        if (material != null)
        {
            material.SetFloat(floatName, value);
        }
        
    }

 
    public void SetMaterialToggle()
    {
        rend = diceCore.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshRenderer>();
        material = rend.material;
    	signChange *= -1;
    	toggleValue += signChange;
    	material.SetFloat(toggleName, toggleValue);
    	print(toggleValue);
    }
}
