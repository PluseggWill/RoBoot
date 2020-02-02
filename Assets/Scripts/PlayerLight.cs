using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class PlayerLight : MonoBehaviour
{
    public Light2D playerLight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LightIt();
    }

    public void LightIt()
    {
        if (/*true||*/GameManager.instance.condition.body.Equals("Light"))//判断手是不是灯
        {
            playerLight.intensity = 0f;
        }
        else
        {
            playerLight.intensity = 1f;
        }
    }

}
