using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed;
    Rigidbody rigidBody;

    [SerializeField] private bool delayDestroyOnStart;
    [SerializeField] private float delayDuration;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.velocity = transform.forward * speed;
        if (delayDestroyOnStart)
            Destroy(gameObject, delayDuration);
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
