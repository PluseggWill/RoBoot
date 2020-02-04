using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public RoBootCondition item;
    public float radian = 0f;
    public float perRadian = 0.1f;
    public float radius = 0.005f;
    public Hand hand;
    public Body body;
    public Leg leg;
    public Collider2D boxCollider;
    public SpriteRenderer itemSprite;
    public bool isGoal = false;

    private void Awake() {
        if (isGoal)
        {
            item = new RoBootCondition(Hand.None, Body.None, Leg.None);
        }
        else
        {
            item = new RoBootCondition(hand, body, leg);
        }
        UpdateItem();
    }

    private void OnTriggerStay2D(Collider2D other) {
        //Debug.Log("Staying" + other.gameObject.name);
    }

    private void FixedUpdate() {
        radian += perRadian;
        itemSprite.transform.position += new Vector3(0, Mathf.Cos(radian) * radius, 0);
    }

    public void ExchangeItem(RoBootCondition temp)
    {   
        if (isGoal)
        {
            if (hand == Hand.Goal)
            {
                item.hand = temp.hand == Hand.Goal ? Hand.Goal : temp.hand;
            }

            else if (body == Body.Goal)
            {
                item.body = temp.body == Body.Goal ? Body.Goal : temp.body;
            }

            else if (leg == Leg.Goal)
            {
                item.leg = temp.leg == Leg.Goal ? Leg.Goal : temp.leg;
            }
        }
        else
        {
            item.hand = item.hand == Hand.None ? item.hand : temp.hand;
            item.body = item.body == Body.None ? item.body : temp.body;
            item.leg = item.leg == Leg.None ? item.leg : temp.leg;
        }
        UpdateItem();
        //Debug.Log("Item Exchanged");
    }

    private void UpdateItem()
    {
        NormalUpdate();
    }

    private void NormalUpdate()
    {
        string tempPath = "";
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
            default:
                break;
        }
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
            default:
                break;
        }
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
            default:
                break;
        }
        //Debug.Log("The Path is: " + tempPath);
        itemSprite.sprite = Resources.Load<Sprite>(tempPath);
    }
}
