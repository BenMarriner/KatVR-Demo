using UnityEngine;
using Valve.VR.InteractionSystem;

public class Fruit : MonoBehaviour
{
    private Interactable interactable;

    public enum Fruits { Apple, Banana, Grapes, Mango, Passionfruit }
    

    public Fruits fruitType;
    [HideInInspector] public bool isBeingHeldInBasket;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.onDetachedFromHand += Interactable_onDetachedFromHand;

        var bodyCollider = GameObject.Find("BodyCollider").GetComponent<Collider>();
        Physics.IgnoreCollision(GetComponent<Collider>(), bodyCollider);
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Place fruit in basket
    private void Interactable_onDetachedFromHand(Hand hand)
    {
        var basket = hand.otherHand.currentAttachedObject;
        if (isBeingHeldInBasket)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            hand.currentAttachedObject.transform.SetParent(basket.transform);
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }

        Debug.Log("Detaching from hand");
    }
}
