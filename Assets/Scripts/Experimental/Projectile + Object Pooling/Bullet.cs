using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    
    private GameObject owner;
    private IDamager damager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init(float speed, GameObject owner, IDamager damager)
    {
        rb.velocity = transform.forward * speed * Time.fixedDeltaTime;
        
        this.owner = owner;
        this.damager = damager;
    }

    private void OnTriggerStay(Collider other)
    {
        IHumanoid hum;

        // Debug.Log("----------------------------------");
        if (other.attachedRigidbody != null)
        {
            // Debug.Log("rigid body exists");
            // Debug.Log(other.attachedRigidbody.gameObject + " | " + owner);
            if (other.attachedRigidbody.gameObject.layer != gameObject.layer && other.attachedRigidbody.gameObject != owner)
            {
                // Debug.Log("rigidbody isnt on same layer and hit gameobject isnt owner");
                if ((hum = other.attachedRigidbody.gameObject.GetComponent<IHumanoid>()) != null)
                {
                    // Debug.Log("it has a humanoid");
                    hum.TakeDamage(damager);
                }
                
                gameObject.SetActive(false);
            }
        }
        else if(other.gameObject.layer != gameObject.layer && other.gameObject != owner)
        {
            // Debug.Log("rigid body does not exist");
            // Debug.Log("poof #2");
            if ((hum = other.gameObject.GetComponent<IHumanoid>()) != null)
            {
                // Debug.Log("it has a humanoid");
                hum.TakeDamage(damager);
            }
            
            gameObject.SetActive(false);
        }
    }
}