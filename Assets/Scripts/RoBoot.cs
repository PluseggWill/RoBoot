﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoBoot : MonoBehaviour
{
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_JumpForce = 1000f;
    [SerializeField] private float m_DropRate = 3f;
    [SerializeField] private LayerMask m_GroundLayer;
    [SerializeField] private bool m_AirControl = true;
    //[SerializeField] private bool m_UseGravity = true;
    
    public GameObject tilemapGameObject;
    private Tilemap tilemap;
    public Transform m_Transform;
    private Transform m_GroundCheck;
    const float m_GroundRadius = .2f;
    private Rigidbody2D m_Rigidbody;
    private GameObject m_BodyPart;
    private GameObject m_Upper;
    private GameObject m_LegPart;
    private GameObject m_Lower;
    public GameObject m_Hand1;
    public GameObject m_Hand2;
    private bool m_FacingRight = true;
    public bool m_Magnet = false;
    public bool m_IsMag = false;
    private float m_MagCoe;
    [SerializeField]private bool m_Grounded;
    private float m_JumpableCoe = 0.6f;

    private void Awake() 
    {
        tilemap = tilemapGameObject.GetComponent<Tilemap>();
        m_GroundCheck = transform.Find("GroundCheck");
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Transform = GetComponent<Transform>();
        m_Upper = GameObject.Find("Upper");
        m_Lower = GameObject.Find("Lower");
        Debug.Log(m_Hand1.name);
        m_Upper.SetActive(false);
        m_Lower.SetActive(false);
        m_GroundLayer = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate() 
    {
        m_Grounded = false;
        m_MagCoe = m_Magnet ? -1.3f : 1;
        //Debug.Log("MagnetCoe:  " + m_MagCoe);

        if (m_Rigidbody.velocity.y * m_MagCoe < 0)
        {
            m_Rigidbody.AddForce(Physics2D.gravity * m_DropRate * m_MagCoe);
        }
        else
        {
            m_Rigidbody.AddForce(Physics2D.gravity * m_MagCoe);
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

            if (move * m_MagCoe > 0 && !m_FacingRight)
            {
                Flip();
            }
            else if (move * m_MagCoe < 0 && m_FacingRight)
            {
                Flip();
            }
            if (m_Grounded && (move * move) > 0.1f)
            {
                if (GameManager.instance.condition.leg == Leg.None)
                {
                    GameManager.instance.PlaySe(0);
                }
                else if (GameManager.instance.condition.leg == Leg.Goal || GameManager.instance.condition.leg == Leg.Magnet)
                {
                    GameManager.instance.PlaySe(3);
                }
            }
            else{
                GameManager.instance.StopSe(3);
                GameManager.instance.StopSe(0);
            }
        }

        if (m_Grounded && jump)
        {
            GameManager.instance.PlaySe(2);
            m_Grounded = false;
            m_Rigidbody.AddForce(new Vector2(0f, m_JumpForce * m_MagCoe * m_JumpableCoe));
        }
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void PickItem (RoBootCondition item)
    {
        // Called when picking up item
        RoBootCondition temp = new RoBootCondition();
        temp.Update(GameManager.instance.condition);
        
        temp.hand = item.hand == Hand.None ? temp.hand : item.hand;
        temp.body = item.body == Body.None ? temp.body : item.body;
        temp.leg = item.leg == Leg.None ? temp.leg : item.leg;


        UpdateRoBoot(temp);
    }

    public void UpdateRoBoot(RoBootCondition condition)
    {
        if (condition.leg == Leg.Spring)
        {
            m_JumpableCoe = 1.1f;
        }
        else
        {
            m_JumpableCoe = 0.6f;
        }

        if (condition.leg == Leg.Magnet)
        {
            m_IsMag = true;
        }
        else
        {
            m_IsMag = false;
        }
        UpdateCollider(condition);
        UpdateParts(condition);
        GameManager.instance.UpdateAudio();
    }

    public void UpdateParts(RoBootCondition condition)
    {
        // Load the sprite of body parts
        string bodyPath = "";
        string legPath = "";
        string handPath = "";

        if (condition.hand != Hand.None)
        {
            switch (condition.hand)
            {
                case Hand.Plug:
                    handPath = "Sprites/HandPlug";
                    break;
                case Hand.Drill:
                    handPath = "Sprites/HandDrill";
                    break;
                case Hand.Goal:
                    handPath = "Sprites/HandGoal";
                    break;
            }
        }
        if (condition.body != Body.None)
        {
            switch (condition.body)
            {
                case Body.Light:
                    bodyPath = "Sprites/BodyLight";
                    break;
                case Body.Hanger:
                    bodyPath = "Sprites/BodyHanger";
                    break;
                case Body.Goal:
                    bodyPath = "Sprites/BodyGoal";
                    break;
            }
        }
        if (condition.leg != Leg.None)
        {
            switch (condition.leg)
            {
                case Leg.Magnet:
                    legPath = "Sprites/LegMagnet";
                    break;
                case Leg.Spring:
                    legPath = "Sprites/LegSpring";
                    break;
                case Leg.Goal:
                    legPath = "Sprites/LegGoal";
                    break;
            }
        }
        //Debug.Log("The Path is: " + tempPath);
        if (m_BodyPart != null)
        {
            m_BodyPart.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>(bodyPath);
            //Debug.Log("The BodyPath is: " + bodyPath);
        }
        else
        {
            //Debug.Log("Body part is Null!");
        }

        if (m_LegPart != null)
        {
            m_LegPart.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>(legPath);
            //Debug.Log("The LegPath is: " + legPath);
        }
        else
        {
            //Debug.Log("Leg part is Null!");
        }
        
        if (m_Hand1 != null && m_Hand2 != null)
        {
            Debug.Log("The HandPath is: " + handPath);
            m_Hand1.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(handPath);
            m_Hand2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(handPath);
            //Debug.Log("The HandPath is: " + handPath);
        }
        else
        {
            Debug.Log("Hand part is Null!");
        }
    }

    public void UpdateCollider(RoBootCondition condition)
    {
        // Update different level of collider
        if (GameManager.instance.condition.IsEqual(condition))
        {
            //Debug.Log("Same");
            return;
        }
            
        
        GameManager.instance.condition.Update(condition);
        if (condition.leg == Leg.None && condition.body == Body.None)
        {
            m_LegPart = null;
            m_BodyPart = null;
            m_Upper.SetActive(false);
            m_Lower.SetActive(false);
            m_Hand1.SetActive(true);
            m_Hand2.SetActive(false);
            m_GroundCheck.position = m_Transform.position - new Vector3(0.0f, 0.47f, 0.0f);
        }
        else if (condition.leg != Leg.None && condition.body == Body.None)
        {
            //m_LegPart = GameObject.Find("Upper");
            m_LegPart = m_Upper;
            m_BodyPart = null;
            if (!m_Upper.activeInHierarchy)
            {
                m_Transform.position += new Vector3(0.0f, 1.0f, 0.0f);
            }
            m_Upper.SetActive(true);
            m_Lower.SetActive(false);
            m_Hand1.SetActive(false);
            m_Hand2.SetActive(true);
            m_GroundCheck.position = m_Transform.position - new Vector3(0.0f, 1.47f, 0.0f);
        }
        else if (condition.leg != Leg.None && condition.body != Body.None)
        {
            m_LegPart = m_Lower;
            m_BodyPart = m_Upper;
            if (!m_Upper.activeInHierarchy)
            {
                m_Transform.position += new Vector3(0.0f, 3.0f, 0.0f);
                if (!m_Lower.activeInHierarchy)
                {
                    //Debug.Log("fuck");
                    //m_Transform.position += new Vector3(0.0f, 5.0f, 0.0f);
                }
            }
            m_Upper.SetActive(true);
            m_Lower.SetActive(true);
            m_Hand1.SetActive(false);
            m_Hand2.SetActive(true);
            m_GroundCheck.position = m_Transform.position - new Vector3(0.0f, 2.47f, 0.0f);
        }
        else
        {
            m_BodyPart = m_Upper;
            m_LegPart = null;
            if (!m_Upper.activeInHierarchy)
            {
                m_Transform.position += new Vector3(0.0f, 1.0f, 0.0f);
            }
            m_Upper.SetActive(true);
            m_Lower.SetActive(false);
            m_Hand1.SetActive(false);
            m_Hand2.SetActive(true);
            m_GroundCheck.position = m_Transform.position - new Vector3(0.0f, 1.47f, 0.0f);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.instance.condition.hand.Equals("Drill")) //判断手是不是钻头
        {
            Debug.Log( tilemapGameObject == collision.gameObject);
            Vector3 hitPosition = Vector3.zero;
            if (tilemap != null && tilemapGameObject == collision.gameObject)
            {
                
                foreach (ContactPoint2D hit in collision.contacts)
                {
                    hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
                    hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
                    tilemap.SetTile(tilemap.WorldToCell(hitPosition), null);
                }
            }
        }
    }
}

    


