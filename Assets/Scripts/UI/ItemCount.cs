using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCount : MonoBehaviour
{
    public Image panelImage;
    public Color panelColour = Color.white;
    public TextMeshProUGUI counterText;
    public Basket basket;
    public Fruit.Fruits fruitSelection;

    // Start is called before the first frame update
    void Start()
    {
        panelImage.color = panelColour;

    }

    // Update is called once per frame
    void Update()
    {   
        switch (fruitSelection)
        {
            case Fruit.Fruits.Apple:
                counterText.text = basket.appleCount.ToString();
                break;
            case Fruit.Fruits.Banana:
                counterText.text = basket.bananaCount.ToString();
                break;
            case Fruit.Fruits.Grapes:
                counterText.text = basket.grapesCount.ToString();
                break;
            case Fruit.Fruits.Mango:
                counterText.text = basket.mangoCount.ToString();
                break;
            case Fruit.Fruits.Passionfruit:
                counterText.text = basket.passionfruitCount.ToString();
                break;
        }
    }

    private void OnValidate()
    {
        panelImage.color = panelColour;
    }
}
