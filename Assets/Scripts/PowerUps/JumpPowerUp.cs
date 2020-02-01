using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPowerUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider c) 
    {
        if (c.CompareTag("Player"))
        {
            PowerUpsManager manager = c.GetComponent<PowerUpsManager>();

            if (manager != null && !manager.GetDoubleJump())
            {
                manager.JumpHigher();
                Destroy(gameObject);
            }
        }
    }

}
