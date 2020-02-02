using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (RoBoot))]
public class RoBootController : MonoBehaviour
{
    private RoBoot m_RoBoot;
    private bool m_Jump = false;
    private bool m_Pickable = false;
    private Item m_TargetItem;
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
        if (Input.GetKeyDown(KeyCode.F) && m_Pickable && m_TargetItem != null)
        {
            RoBootCondition temp = new RoBootCondition();
            temp.Update(m_TargetItem.item);
            m_TargetItem.ExchangeItem(GameManager.instance.condition);
            m_RoBoot.PickItem(temp);
        }

        // Debug Code
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_RoBoot.UpdateCollider(new RoBootCondition(){body = Body.None, leg = Leg.None});
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            m_RoBoot.UpdateCollider(new RoBootCondition(){body = Body.None, leg = Leg.Goal});
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_RoBoot.UpdateCollider(new RoBootCondition(){body = Body.Goal, leg = Leg.Goal});
        }


    }

    private void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal");
        m_RoBoot.Move(move, m_Jump);
        m_Jump = false;
    }

    private void OnTriggerStay2D(Collider2D other) {
        m_TargetItem = other.gameObject.GetComponent<Item>();
        if (m_TargetItem != null)
        {
            m_Pickable = true;
        }
    }
}
