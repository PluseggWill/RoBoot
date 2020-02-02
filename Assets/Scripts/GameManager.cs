using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public RoBootCondition condition;



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(transform.gameObject);
        }
        UpdateCondition(new RoBootCondition(){hand = Hand.None, body = Body.None, leg = Leg.None});
    }

    public void UpdateCondition(RoBootCondition update)
    {
        condition = update;
    }
}

public enum Hand {None, Drill, Plug, Goal};
public enum Body {None, Light, Hanger, Goal};
public enum Leg {None, Spring, Magnet, Goal};
public class RoBootCondition
{
    public Hand hand;
    public Body body;
    public Leg leg;

    public RoBootCondition()
    {
        hand = Hand.None;
        body = Body.None;
        leg = Leg.None;
    }

    public RoBootCondition(Hand tempHand, Body tempBody, Leg tempLeg)
    {
        hand = tempHand;
        body = tempBody;
        leg = tempLeg;
    }
    public bool IsEqual(RoBootCondition temp)
    {
        if (hand == temp.hand && body == temp.body && leg == temp.leg)
            return true;
        return false;
    }

    public void Update(RoBootCondition temp)
    {
        hand = temp.hand;
        body = temp.body;
        leg = temp.leg;
    }
}
