using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Basket : MonoBehaviour
{
    private Interactable interactable;

    [SerializeField] private bool isHeld;
    public Collider basketCollider;
    public Collider fruitTrigger;
    public int appleCount = 0, bananaCount = 0, grapesCount = 0, mangoCount = 0, passionfruitCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable.attachedToHand)
        {
            isHeld = interactable.attachedToHand.ObjectIsAttached(gameObject);
        }
        else isHeld = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (!(other.gameObject.layer == LayerMask.NameToLayer("Fruit"))) return;

        if (!IsHoldingBasketAndFruit(other.gameObject, out var fruitComponent)) return;

        Physics.IgnoreCollision(other, basketCollider, true);
        fruitComponent.isBeingHeldInBasket = true;
    }

    private void OnTriggerStay(Collider other)
    {
        //if (!(other.gameObject.layer == LayerMask.NameToLayer("Fruit"))) return;

        if (!IsHoldingBasketAndFruit(other.gameObject, out var fruitComponent)) return;

        fruitComponent.isBeingHeldInBasket = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //if (!(other.gameObject.layer == LayerMask.NameToLayer("Fruit"))) return;

        IsHoldingBasketAndFruit(other.gameObject, out var fruitComponent);
        if (!fruitComponent) return;

        Physics.IgnoreCollision(other, basketCollider, false);
        fruitComponent.isBeingHeldInBasket = false;
    }

    private bool IsHoldingBasketAndFruit(GameObject obj, out Fruit outFruitComponent)
    {        
        outFruitComponent = null;

        if (!isHeld) return false; // Is the player holding the basket?
        if (!interactable.attachedToHand) return false; // Is the player holding an object?
        if (!interactable.attachedToHand.otherHand.ObjectIsAttached(obj)) return false; // Is it in their other hand?
        if (!obj.TryGetComponent<Fruit>(out var fruitComponent)) return false; // Is the object a fruit?

        outFruitComponent = fruitComponent;
        return true;
    }
}
