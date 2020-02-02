using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public RoBootCondition item;
    public Hand hand;
    public Body body;
    public Leg leg;
    public Collider2D boxCollider;

    private void Awake() {
        item = new RoBootCondition(hand, body, leg);
        UpdateItem();
    }

    private void OnTriggerStay2D(Collider2D other) {
        //Debug.Log("Staying" + other.gameObject.name);
    }

    public void ExchangeItem(RoBootCondition temp)
    {   
        item.hand = item.hand == Hand.None ? item.hand : temp.hand;
        temp.body = item.body == Body.None ? item.body : temp.body;
        temp.leg = item.leg == Leg.None ? item.leg : temp.leg;
        UpdateItem();
    }

    private void UpdateItem()
    {
        switch (item.hand)
        {
            case Hand.None:
                break;
            case Hand.Socket:
                break;
            case Hand.Drill:
                break;
            case Hand.Goal:
                break;
        }

        switch (item.body)
        {
            case Body.None:
                break;
            case Body.Light:
                break;
            case Body.Hanger:
                break;
            case Body.Goal:
                break;
        }

        switch (item.leg)
        {
            case Leg.None:
                break;
            case Leg.Magnet:
                break;
            case Leg.Spring:
                break;
            case Leg.Goal:
                break;
        }
    }
}
