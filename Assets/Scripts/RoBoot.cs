using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Experimental.Rendering.LWRP;

public class RoBoot : MonoBehaviour
{
    [SerializeField] private LayerMask m_GroundLayer;
    [SerializeField] private bool m_AirControl = true;
    //[SerializeField] private bool m_UseGravity = true;
    public GameObject playerLightGameObject;
    Light2D playerLight;
    public GameObject tilemapGameObject;
    private Tilemap tilemap;
    public Transform m_Transform;
    private Transform m_GroundCheck;
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
    public bool m_IsDrill = false;
    public bool m_IsLight = false;
    private float m_MagCoe;
    [SerializeField]private bool m_Grounded;

    #region PrivateParameters
    const float m_GroundRadius = .2f;
    private float m_JumpableCoe;
    private float[] jumpCoe = {0.6f, 1.2f};
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_JumpForce = 1000f;
    [SerializeField] private float m_DropRate = 3f;
    public GameObject[] JumpCheck;

    #endregion

    private void Awake() 
    {
        playerLight = playerLightGameObject.GetComponent<Light2D>();
        tilemap = tilemapGameObject.GetComponent<Tilemap>();
        m_GroundCheck = transform.Find("GroundCheck");
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Transform = GetComponent<Transform>();
        m_Upper = GameObject.Find("Upper");
        m_Lower = GameObject.Find("Lower");
        m_Upper.SetActive(false);
        m_Lower.SetActive(false);
        m_GroundLayer = LayerMask.GetMask("Ground");
        m_JumpableCoe = jumpCoe[0];
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

    private void Update()
    {
        LightIt();
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

    public void PickItem (RoBootCondition item, bool isGoal)
    {
        // Called when picking up item
        RoBootCondition temp = new RoBootCondition();
        temp.Update(GameManager.instance.condition);

        if (isGoal)
        {
            if (temp.hand == Hand.Goal && item.hand == Hand.Goal)
            {
                temp.hand = Hand.None;
            }
            else if (temp.body == Body.Goal && item.body == Body.Goal)
            {
                temp.body = Body.None;
            }
            else if (temp.leg == Leg.Goal && item.leg == Leg.Goal)
            {
                temp.leg = Leg.None;
            }
            UpdateRoBoot(temp);
            return;
        }

        if (temp.hand != Hand.None)
        {
            if (item.hand != Hand.None)
            {
                temp.hand = item.hand;
            }
        }
        else
        {
            temp.hand = item.hand;
        }
        
        if (temp.body != Body.None)
        {
            if (item.body != Body.None)
            {
                temp.body = item.body;
            }
        }
        else
        {
            temp.body = item.body;
        }

        if (temp.leg != Leg.None)
        {
            if (item.leg != Leg.None)
            {
                temp.leg = item.leg;
            }
        }
        else
        {
            temp.leg = item.leg;
        }

        //Debug.Log("Exchanged");
        UpdateRoBoot(temp);
    }

    public void UpdateRoBoot(RoBootCondition condition)
    {
        Debug.Log("The Hand Now Is: " + condition.hand);
        Debug.Log("The Body Now Is: " + condition.body);
        Debug.Log("The Leg Now Is: " + condition.leg);

        if (condition.leg == Leg.Spring)
        {
            m_JumpableCoe = jumpCoe[1];
        }
        else
        {
            m_JumpableCoe = jumpCoe[0];
        }

        if (condition.leg == Leg.Magnet)
        {
            m_IsMag = true;
        }
        else
        {
            m_IsMag = false;
        }
        if(condition.hand == Hand.Drill)
        {
            m_IsDrill = true;
        }
        else
        {
            m_IsDrill = false;
        }
        if (condition.body == Body.Light)
        {
            m_IsLight = true;
        }
        else
        {
            m_IsLight = false;
        }
        
        UpdateCollider(condition);
        UpdateParts(condition);
        GameManager.instance.UpdateCondition(condition);
        GameManager.instance.UpdateAudio();
    }

    public void UpdateParts(RoBootCondition condition)
    {
        // Load the sprite of body parts
        string bodyPath = "";
        string legPath = "";
        string handPath = "";

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
            default:
                break;
        }

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
            default:
                break;
        }

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
            default:
                break;
        }
        //Debug.Log("The Path is: " + tempPath);
        if (m_BodyPart != null)
        {
            m_BodyPart.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>(bodyPath);
            //Debug.Log("The BodyPath is: " + bodyPath);
        }

        if (m_LegPart != null)
        {
            m_LegPart.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>(legPath);
            //Debug.Log("The LegPath is: " + legPath);
        }

        
        if (m_Hand1 != null)
        {
            //Debug.Log("The HandPath is: " + handPath);
            m_Hand1.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(handPath);
        }

        if (m_Hand2 != null)
        {
            //Debug.Log("The HandPath is: " + handPath);
            m_Hand2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(handPath);
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
        
        if (condition.leg == Leg.None && condition.body == Body.None)
        {
            m_LegPart = null;
            m_BodyPart = null;
            m_Upper.SetActive(false);
            m_Lower.SetActive(false);
            m_Hand1.SetActive(true);
            m_Hand2.SetActive(false);
            m_GroundCheck.position = JumpCheck[0].transform.position;
        }
        else if (condition.leg != Leg.None && condition.body == Body.None)
        {
            // Only Leg
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
            m_GroundCheck.position = JumpCheck[1].transform.position;
        }
        else if (condition.leg != Leg.None && condition.body != Body.None)
        {
            // Both Leg and Body
            m_LegPart = m_Lower;
            m_BodyPart = m_Upper;
            if (!m_Upper.activeInHierarchy)
            {
                m_Transform.position += new Vector3(0.0f, 1.0f, 0.0f);
                if (!m_Lower.activeInHierarchy)
                {
                    //Debug.Log("fuck");
                    m_Transform.position += new Vector3(0.0f, 1.0f, 0.0f);
                }
            }
            else
            {
                m_Transform.position += new Vector3(0.0f, 1.0f, 0.0f);
            }
            m_Upper.SetActive(true);
            m_Lower.SetActive(true);
            m_Hand1.SetActive(false);
            m_Hand2.SetActive(true);
            m_GroundCheck.position = JumpCheck[2].transform.position;
        }
        else
        {
            // Only Body
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
            m_GroundCheck.position = JumpCheck[1].transform.position;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(m_IsDrill);
        if (m_IsDrill) //判断手是不是钻头
        {
            //Debug.Log( tilemapGameObject == collision.gameObject);
            Vector3 hitPosition = Vector3.zero;
            if (tilemap != null && tilemapGameObject == collision.gameObject)
            {
                
                foreach (ContactPoint2D hit in collision.contacts)
                {
                    GameManager.instance.PlaySe(1);
                    hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
                    hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
                    tilemap.SetTile(tilemap.WorldToCell(hitPosition), null);
                }
            }
        }
    }

    void LightIt()
    {
        if (m_IsLight)
        {
            playerLight.intensity = 2;
        }
        else
        {
            playerLight.intensity = 0;
        }
    }
}

    


