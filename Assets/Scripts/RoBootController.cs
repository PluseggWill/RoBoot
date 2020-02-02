using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (RoBoot))]
public class RoBootController : MonoBehaviour
{
    private RoBoot m_RoBoot;
    private bool m_Jump;
    private void Awake()
    {
        m_RoBoot = GetComponent<RoBoot>();
    }

    private void Update()
    {
        if (!m_Jump)
        {
            m_Jump = Input.GetButtonDown("Jump");
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
}
