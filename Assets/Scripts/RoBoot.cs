using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoBoot : MonoBehaviour
{
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_JumpForce = 400f;
    [SerializeField] private LayerMask m_GroundLayer;
    [SerializeField] private RobotCondition m_RobotCondtion;
    [SerializeField] private bool m_AirControl = true;
    [SerializeField] private bool m_UseGravity = true;

    private Transform m_GroundCheck;
    const float m_GroundRadius = 0.2f;
    private Transform m_CellingCheck;
    const float m_CellingRadius = 0.01f;
    private Animator m_Anim;
    private Rigidbody2D m_Rigidbody;
    private bool m_FacingRight = true;
    private bool m_Grounded;

    private void Awake() 
    {
        m_GroundCheck = transform.Find("GroundCheck");
        m_CellingCheck = transform.Find("CellingCheck");
        m_Anim = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() 
    {
        m_Grounded = false;
        if (m_UseGravity)
        {
            m_Rigidbody.AddForce(Physics2D.gravity * m_Rigidbody.mass * m_Rigidbody.mass);
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, m_GroundRadius, m_GroundLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
            }
        }
    }

    public void Move(float move, bool jump)
    {
        if (m_Grounded || m_AirControl)
        {
            m_Rigidbody.velocity = new Vector2(move * m_Speed, m_Rigidbody.velocity.y);

            if (move > 0 && !m_FacingRight)
            {
                Flip();
            }
            else if (move < 0 && m_FacingRight)
            {
                Flip();
            }
        }

        if (m_Grounded && jump)
        {
            m_Grounded = false;
            m_Rigidbody.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}

public enum RobotCondition{ Core, Leg, Full};
