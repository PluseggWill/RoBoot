using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoBoot : MonoBehaviour
{
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_JumpForce = 1000f;
    [SerializeField] private float m_DropRate = 3f;
    [SerializeField] private LayerMask m_GroundLayer;
    [SerializeField] private bool m_AirControl = true;
    [SerializeField] private bool m_UseGravity = true;

    private Transform m_Transform;
    private Transform m_GroundCheck;
    const float m_GroundRadius = .2f;
    private Rigidbody2D m_Rigidbody;
    private GameObject m_BodyPart;
    private GameObject m_Upper;
    private GameObject m_LegPart;
    private GameObject m_Lower;
    private bool m_FacingRight = true;
    [SerializeField]private bool m_Grounded;
    private bool m_Jumpable;

    private void Awake() 
    {
        m_GroundCheck = transform.Find("GroundCheck");
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Transform = GetComponent<Transform>();
        m_Upper = GameObject.Find("Upper");
        m_Lower = GameObject.Find("Lower");
        m_Upper.SetActive(false);
        m_Lower.SetActive(false);
        m_GroundLayer = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate() 
    {
        m_Grounded = false;

        if (m_UseGravity)
        {
            if (m_Rigidbody.velocity.y < 0)
            {
                m_Rigidbody.AddForce(Physics2D.gravity * m_DropRate);
            }
            else
            {
                m_Rigidbody.AddForce(Physics2D.gravity);
            }
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, m_GroundRadius, m_GroundLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
            }
        }

        // Test Code
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

    public void UpdateCollider(RoBootCondition condition)
    {
        if (GameManager.instance.condition.IsEqual(condition))
        {
            Debug.Log("Same");
            return;
        }
            
        
        GameManager.instance.condition.Update(condition);
        if (condition.leg == Leg.None && condition.body == Body.None)
        {
            m_Upper.SetActive(false);
            m_Lower.SetActive(false);
            m_GroundCheck.position = m_Transform.position - new Vector3(0.0f, 0.47f, 0.0f);
        }
        else if (condition.leg != Leg.None && condition.body == Body.None)
        {
            m_LegPart = GameObject.Find("Upper");
            if (!m_Upper.activeInHierarchy)
            {
                m_Transform.position += new Vector3(0.0f, 1.0f, 0.0f);
            }
            m_Upper.SetActive(true);
            m_Lower.SetActive(false);
            m_GroundCheck.position = m_Transform.position - new Vector3(0.0f, 1.47f, 0.0f);
        }
        else if (condition.leg != Leg.None && condition.body != Body.None)
        {
            m_LegPart = GameObject.Find("Lower");
            m_BodyPart = GameObject.Find("Upper");
            if (!m_Upper.activeInHierarchy)
            {
                m_Transform.position += new Vector3(0.0f, 1.0f, 0.0f);
                if (!m_Lower.activeInHierarchy)
                {
                    m_Transform.position += new Vector3(0.0f, 1.0f, 0.0f);
                }
            }
            m_Upper.SetActive(true);
            m_Lower.SetActive(true);
            m_GroundCheck.position = m_Transform.position - new Vector3(0.0f, 2.47f, 0.0f);
        }
        else
        {
            m_BodyPart = GameObject.Find("Upper");
            if (!m_Upper.activeInHierarchy)
            {
                m_Transform.position += new Vector3(0.0f, 1.0f, 0.0f);
            }
            m_Upper.SetActive(true);
            m_Lower.SetActive(false);
            m_GroundCheck.position = m_Transform.position - new Vector3(0.0f, 1.47f, 0.0f);
        }
    }
}


