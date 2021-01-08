using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceSwapButton : MonoBehaviour
{
    public Text buttonText;

    bool isNormalDice = true;

    [Header("References")]
    public List<GameObject> premiumDiceParameters;
    public List<GameObject> normalDiceParameters;

    // Start is called before the first frame update
    void Start()
    {
        SetupSwapObjects(this.isNormalDice);
    }

    public void SwapDices()
    {
        this.isNormalDice = !this.isNormalDice;

        SetupSwapObjects(this.isNormalDice);
    }
    private void SetupSwapObjects(bool isNormalDice = true)
    {
        this.isNormalDice = isNormalDice;

        foreach (GameObject go in premiumDiceParameters)
        {
            go.SetActive(!this.isNormalDice);
        }

        foreach (GameObject go in normalDiceParameters)
        {
            go.SetActive(this.isNormalDice);
        }

        if(this.isNormalDice)
        {
            buttonText.text = "USE PREMIUM DICE";
        }
        else
        {
            buttonText.text = "USE NORMAL DICE";
        }
    }
}
