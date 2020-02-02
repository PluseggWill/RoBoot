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
            Destroy(transform.gameObject);

        condition.hand = Hand.None;
        condition.body = Body.None;
        condition.leg = Leg.None;
    }
}

public enum Hand {None, Drill, Socket, Goal};
public enum Body {None, Light, Hanger, Goal};
public enum Leg {None, Spring, Magnet, Goal};
public class RoBootCondition
{
    public Hand hand;
    public Body body;
    public Leg leg;
}
