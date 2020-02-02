using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public RoBootCondition condition;
    private Component[] audios;



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
        audios = GetComponentsInChildren<AudioSource>();
        UpdateCondition(new RoBootCondition(){hand = Hand.None, body = Body.None, leg = Leg.None});
        UpdateAudio();
    }

    public void UpdateCondition(RoBootCondition update)
    {
        condition = update;
    }

    public void UpdateAudio()
    {
        string handAudio = "Hand";
        string bodyAudio = "Body";
        string legAudio = "Leg";

        if (condition.hand != Hand.None)
        {
            handAudio += (int)condition.hand;
            Debug.Log("handAudio: " + handAudio);
        }
        if (condition.body != Body.None)
        {
            bodyAudio += (int)condition.body;
            Debug.Log("bodyAudio: " + bodyAudio);
        }
        if (condition.leg != Leg.None)
        {
            legAudio += (int)condition.leg;
            Debug.Log("legAudio: " + legAudio);
        }

        foreach (AudioSource tempAudio in audios)
        {
            if (tempAudio.name == handAudio)
            {
                tempAudio.mute = false;
                continue;
            }
            if (tempAudio.name == bodyAudio)
            {
                tempAudio.mute = false;
                continue;
            }
            if (tempAudio.name == legAudio)
            {
                tempAudio.mute = false;
                continue;
            }
            if (tempAudio.name == "Head")
            {
                tempAudio.mute = false;
                continue;
            }
            tempAudio.mute = true;
        }
    }
}

public enum Hand {None, Plug, Drill, Goal};
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
