using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Experimental.Rendering.LWRP;

public class RoBoot : MonoBehaviour
{
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_JumpForce = 1000f;
    [SerializeField] private float m_DropRate = 3f;
    [SerializeField] private LayerMask m_GroundLayer;
    [SerializeField] private bool m_AirControl = true;
    [SerializeField] private bool m_UseGravity = true;
    public bool m_Leg = false;
    public bool m_Body = false;
    public Light2D playerLight;

    private Transform m_GroundCheck;
    const float m_GroundRadius = .02f;
    private Transform m_CellingCheck;
    const float m_CellingRadius = 0.01f;
    private Rigidbody2D m_Rigidbody;
    private GameObject m_BodyPart;
    private GameObject m_LegPart;
    private bool m_FacingRight = true;
    private bool m_Grounded;
    private bool m_Jumpable;

    private void Awake() 
    {
        m_GroundCheck = transform.Find("GroundCheck");
        m_CellingCheck = transform.Find("CellingCheck");
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_BodyPart = GameObject.Find("Body");
        m_LegPart = GameObject.Find("Leg");
        m_BodyPart.SetActive(false);
        m_LegPart.SetActive(false);
    }

    void FixedUpdate() 
    {
        PlayerLight();
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
        if (m_Leg)
        {
    
            GameManager.instance.condition.leg = Leg.Goal;
            if (m_Body)
            {
                GameManager.instance.condition.body = Body.Goal;
            }
        }
        else
        {
           
            GameManager.instance.condition.body = Body.None;
            GameManager.instance.condition.leg = Leg.None;
        }
      
        UpdateCollider();
       

       
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

    private void UpdateCollider()
    {
        
        if (GameManager.instance.condition.leg != Leg.None)
        {
            if (GameManager.instance.condition.body != Body.None)
            {
                // Only Leg, No Body
                m_BodyPart.SetActive(false);
                m_LegPart.SetActive(true);
                m_LegPart.transform.position = new Vector3(0f, -1f, 0f);
                m_GroundCheck.position = new Vector3(0f, -1.47f, 0f);
                GetComponent<Transform>().position += new Vector3(0,1,0);
            }
            else
            {
                // Both
                m_BodyPart.SetActive(true);
                m_BodyPart.transform.position = new Vector3(0f, -1f, 0f);
                m_LegPart.SetActive(true);
                m_LegPart.transform.position = new Vector3(0f, -2.0f, 0f);
                m_GroundCheck.position = new Vector3(0f, -2.47f, 0f);
                GetComponent<Transform>().position += new Vector3(0,2,0);
            }
        }
        else
        {
            m_BodyPart.SetActive(false);
            m_LegPart.SetActive(false);
        }
    }
    private void PlayerLight()
    {
        Debug.Log("light"+playerLight.intensity);
        if (GameManager.instance.condition.body == Body.Light)
        {
            playerLight.intensity = 1F;
        }
        else
        {
            playerLight.intensity = 0F;
        }
        Debug.Log("light" + playerLight.intensity);
    }

}


