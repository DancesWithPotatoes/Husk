//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        28/03/17
// Date last edited:    28/03/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
// A script used to control an individual attack object spawned by a game character.
public class AttackScript : MonoBehaviour
{
    // The amount of time in seconds for which the attack object exists before self-destructing.
    public float Duration = 0.5f;

    // The amount of time in seconds which the attack object has existed since being spawned.
    private float destructionTimer = 0.0f;

    // Called when the script is loaded.
    private void Awake()
    {
        // Sets the collider of the attack to be a trigger so that it will detect when other colliders enter it without physically colliding with them.
        GetComponent<PolygonCollider2D>().isTrigger = true;
        // Sets the collider rigidbody to be kinematic so that the rigidbody doesn't physically interact with the scene - this collider must have a rigidbody attached to prevent it from being classified as static.
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    // Called when the script is initialised.
    private void Start()
    {

    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        destructionTimer += Time.deltaTime;

        if (destructionTimer >= Duration)
            Destroy(this.gameObject);
    }

    // Called when another object enters the trigger collider of the attack object.
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.tag == "Enemy")
            otherCollider.GetComponent<EnemyScript>().Damage();
    }
}