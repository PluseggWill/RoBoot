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
    public SpriteRenderer itemSprite;

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
        item.body = item.body == Body.None ? item.body : temp.body;
        item.leg = item.leg == Leg.None ? item.leg : temp.leg;
        UpdateItem();
    }

    private void UpdateItem()
    {
        string tempPath = "";
        if (item.hand != Hand.None)
        {
            switch (item.hand)
            {
                case Hand.Plug:
                    tempPath = "Sprites/HandPlug";
                    break;
                case Hand.Drill:
                    tempPath = "Sprites/HandDrill";
                    break;
                case Hand.Goal:
                    tempPath = "Sprites/HandGoal";
                    break;
            }
        }
        else if (item.body != Body.None)
        {
            switch (item.body)
            {
                case Body.Light:
                    tempPath = "Sprites/BodyLight";
                    break;
                case Body.Hanger:
                    tempPath = "Sprites/BodyHanger";
                    break;
                case Body.Goal:
                    tempPath = "Sprites/BodyGoal";
                    break;
            }
        }
        else if (item.leg != Leg.None)
        {
            switch (item.leg)
            {
                case Leg.Magnet:
                    tempPath = "Sprites/LegMagnet";
                    break;
                case Leg.Spring:
                    tempPath = "Sprites/LegSpring";
                    break;
                case Leg.Goal:
                    tempPath = "Sprites/LegGoal";
                    break;
            }
        }
        Debug.Log("The Path is: " + tempPath);
        itemSprite.sprite = Resources.Load<Sprite>(tempPath);
    }
}
