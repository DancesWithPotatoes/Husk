//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        28/03/17
// Date last edited:    02/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
// A script used to control an individual attack object spawned by a game character.
public class AttackScript : MonoBehaviour
{
    // Whether or not the attack freezes the game character that it is a child of in place whilst it is active.
    public bool FreezesParent = true;
    // The amount of time in seconds for which the attack object exists before self-destructing.
    public float Duration = 0.2f;
    // The distance moved by the camera when it is shaking throughout the duration of the attack object being active.
    public float ScreenShakeMagnitude = 0.05f;

    // Initialises the attack object using the player character which spawned it.
    public void Initialise(Transform attackingCharacter)
    {
        this.transform.SetParent(attackingCharacter);
        // Rotates the attack object to face in the same direction as the heading of the character which spawned it.
        LocallyRotateAttackToAlignWithVector2(attackingCharacter.GetComponent<PlayerScript>().Heading);

        if (FreezesParent)
            attackingCharacter.GetComponent<PlayerScript>().IsFrozen = true;

        StartCoroutine(Camera.main.GetComponent<MainCameraScript>().Shake(Duration, ScreenShakeMagnitude));

        isActive = true;
    }    

    // Whether the attack has been initialied and is now active.
    private bool isActive = false;
    // The amount of time in seconds which the attack object has existed since being spawned.
    private float destructionTimer = 0.0f;

    // Called when the script is loaded.
    private void Awake()
    {
        // Sets the collider of the attack to be a trigger so that it will detect when other colliders enter it without physically colliding with them.
        GetComponent<Collider2D>().isTrigger = true;
        // Sets the collider rigidbody to be kinematic so that the rigidbody doesn't physically interact with the scene - this collider must have a rigidbody attached to prevent it from being classified as static.
        GetComponent<Rigidbody2D>().isKinematic = true;
    }
    
    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        if (isActive)
        {
            destructionTimer += Time.deltaTime;

            if (destructionTimer >= Duration)
            {
                if (FreezesParent)
                    this.GetComponentInParent<PlayerScript>().IsFrozen = false;

                Destroy(this.gameObject);
            }
        }
    }

    // Called when another object enters the trigger collider of the attack object.
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (isActive)
        {
            if (otherCollider.tag == "Enemy")
                otherCollider.GetComponent<EnemyScript>().Damage();
        }
    }

    // Changes the rotation of the attack object so that it aligns with the specified 2D direction vector in local space.
    private void LocallyRotateAttackToAlignWithVector2(Vector2 direction)
    {
        direction.Normalize();
        // The angle in degrees to rotate the attack object in local space so that it is facing in the same direction as the vector.
        float attackAngle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;

        this.transform.localRotation = Quaternion.AngleAxis(attackAngle, Vector3.forward);
    }
}