using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.VersionControl;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Fruit : MonoBehaviour
{
    private Interactable interactable;

    public enum Fruits { Apple, Banana, Grapes, Mango, Passionfruit }

    [HideInInspector] public bool isBeingHeldInBasket;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.onDetachedFromHand += Interactable_onDetachedFromHand;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isBeingHeldInBasket);
    }

    // Place fruit in basket
    private void Interactable_onDetachedFromHand(Hand hand)
    {
        if (isBeingHeldInBasket)
        {
            var basket = hand.otherHand.currentAttachedObject;
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
