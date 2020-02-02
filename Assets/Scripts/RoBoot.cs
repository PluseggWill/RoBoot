using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.Tilemaps;

public class RoBoot : MonoBehaviour
{
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_JumpForce = 1000f;
    [SerializeField] private float m_DropRate = 3f;
    [SerializeField] private LayerMask m_GroundLayer;
    [SerializeField] private bool m_AirControl = true;
    [SerializeField] private bool m_UseGravity = true;

    public GameObject tilemapGameObject;
    public Light2D playerLight;
    Tilemap tilemap;

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

    private void Start()
    {
        if (tilemapGameObject != null)
        {
            tilemap = tilemapGameObject.GetComponent<Tilemap>();
        }
    }
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

    public void PickItem (RoBootCondition item)
    {
        // Called when picking up item
        RoBootCondition temp = new RoBootCondition();
        temp.Update(GameManager.instance.condition);
        
        temp.hand = item.hand == Hand.None ? temp.hand : item.hand;
        temp.body = item.body == Body.None ? temp.body : item.body;
        temp.leg = item.leg == Leg.None ? temp.leg : item.leg;

        UpdateCollider(temp);
        UpdateParts(temp);
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
                    legPath = "Sprites/LegSpring_Walk";
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
            Debug.Log("The BodyPath is: " + bodyPath);
        }
        else
        {
            Debug.Log("Body part is Null!");
        }
        if (m_LegPart != null)
        {
            m_LegPart.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>(legPath);
            Debug.Log("The LegPath is: " + legPath);
        }
        else
        {
            Debug.Log("Leg part is Null!");
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
            m_GroundCheck.position = m_Transform.position - new Vector3(0.0f, 1.47f, 0.0f);
        }
        else if (condition.leg != Leg.None && condition.body != Body.None)
        {
            m_LegPart = m_Lower;
            m_BodyPart = m_Upper;
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
            m_BodyPart = m_Upper;
            m_LegPart = null;
            if (!m_Upper.activeInHierarchy)
            {
                m_Transform.position += new Vector3(0.0f, 1.0f, 0.0f);
            }
            m_Upper.SetActive(true);
            m_Lower.SetActive(false);
            m_GroundCheck.position = m_Transform.position - new Vector3(0.0f, 1.47f, 0.0f);
        }
    }

    public void PlayerLight()
    {
        if (true)//判断手是不是灯
        {
            playerLight.intensity = 0f;
        }
        else
        {
            playerLight.intensity = 1f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (false) //判断手是不是钻头
        {
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

    


