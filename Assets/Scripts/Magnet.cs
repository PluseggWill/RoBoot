using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    RoBoot m_RoBoot = null;
    
    private void OnTriggerStay2D(Collider2D other) {
        //Debug.Log("Trigger: " + other.name);
        //m_RoBoot = other.gameObject.GetComponentInParent<RoBoot>();
        //if (m_RoBoot != null)
        if (other.transform.name == "Head")
        {
            if (!m_RoBoot.m_Magnet)
            {
                m_RoBoot.m_Magnet = true;
                m_RoBoot.m_Transform.Rotate(new Vector3(0,0,180));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        //m_RoBoot = other.gameObject.GetComponentInParent<RoBoot>();
        //if (m_RoBoot != null)
        if (other.transform.name == "Head")
        {
            if (m_RoBoot.m_Magnet)
            {
                m_RoBoot.m_Magnet = false;
                m_RoBoot.m_Transform.Rotate(new Vector3(0,0,180));
            }
        }
    }
}
