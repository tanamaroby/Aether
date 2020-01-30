using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class SpawnProjectile : MonoBehaviour
{
    public static bool didProjectileHit = false;

    public GameObject firePoint;
    public List<GameObject> vfx = new List<GameObject>();
    public Camera currentCamera;
    public AudioSource shootSound;
    public AudioSource impactSound;

    private GameObject effectToSpawn;
    private float timeToFire = 0;

    void Start()
    {
        AetherInput.GetPlayerActions().Fire.performed += SpawnVFX;
        effectToSpawn = vfx[0];
    }

    void Update()
    {
        if (didProjectileHit)
        {
            impactSound.Play();
            didProjectileHit = false;
        } 
    }

    public void SpawnVFX(InputAction.CallbackContext ctx)
    {
        ButtonControl button = ctx.control as ButtonControl;
        if (!button.wasPressedThisFrame)
        {
            return;
        }
        
        GameObject vfx;
        if (firePoint != null && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / effectToSpawn.GetComponent<MoveProjectile>().firerate;
            vfx = Instantiate(effectToSpawn, firePoint.transform.position, Quaternion.identity);
            vfx.transform.LookAt(currentCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, currentCamera.farClipPlane)));            
            shootSound.Play();
        } else
        {
            Debug.Log("No Fire Point");
        }
    }

    public void playImpactSound()
    {
        impactSound.Play();
    }
}
