using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (RoBoot))]
public class RoBootController : MonoBehaviour
{
    public bool IsDebugMode;
    private RoBoot m_RoBoot;
    private bool m_Jump = false;
    private Item m_TargetItem;
    private DoorTrigger m_DoorTrigger;
    private void Awake()
    {
        m_RoBoot = GetComponent<RoBoot>();
    }

    private void Update()
    {
        // Jump
        if (!m_Jump)
        {
            m_Jump = Input.GetButtonDown("Jump");
        }

        // Pick Item
        if (Input.GetKeyDown(KeyCode.F) && m_TargetItem != null)
        {
            RoBootCondition temp = new RoBootCondition(m_TargetItem.hand, m_TargetItem.body, m_TargetItem.leg);
            if (!m_TargetItem.isGoal)
            {
                temp.Update(m_TargetItem.item);
            }
            m_TargetItem.ExchangeItem(GameManager.instance.condition);
            
            m_RoBoot.PickItem(temp,m_TargetItem.isGoal);
            
        }

        if (Input.GetKeyDown(KeyCode.F) && m_DoorTrigger != null && GameManager.instance.condition.hand == Hand.Plug)
        {
            m_DoorTrigger.isTriggered = true;
        }

        // Debug Code
        if (IsDebugMode)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                m_RoBoot.UpdateRoBoot(new RoBootCondition(){body = Body.None, leg = Leg.None});
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                m_RoBoot.UpdateRoBoot(new RoBootCondition(){body = Body.None, leg = Leg.Goal});
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                m_RoBoot.UpdateRoBoot(new RoBootCondition(){body = Body.Goal, leg = Leg.Goal});
            }
        }
    }

    private void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal");
        m_RoBoot.Move(move, m_Jump);
        m_Jump = false;
    }

    private void OnTriggerStay2D(Collider2D other) {
        // Picking Item
        // Debug.Log("the Name is: " + other.name);

        m_TargetItem = other.gameObject.GetComponent<Item>();

        // Door Trigger
        m_DoorTrigger = other.gameObject.GetComponent<DoorTrigger>();

    }
}
