//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        28/03/17
// Date last edited:    16/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
// A script used to control a temporary attack collider object spawned by a character in the game.
public class AttackScript : MonoBehaviour
{
    // The amount of time in seconds for which the attack object exists before self-destructing.
    public float Duration = 0.2f;
    // The amount of time for which the attack freezes the game character that it is a child of in place after being spawned.
    public float FreezeParentDuration = 0.2f;
    // The distance moved by the camera when it is shaking throughout the duration of the attack object being active.
    public float ScreenShakeMagnitude = 0.025f;

    // Initialises the attack object utilising the player character which spawned it.
    public void InitialiseUsingPlayer(Transform attackingCharacter)
    {
        // Initialises the attack object if it has not already been initialised.
        if (!isActive)
        {
            this.transform.SetParent(attackingCharacter);
            // Rotates the attack object to face in the same direction as the heading of player which spawned it.
            attackingCharacter.GetComponent<PlayerScript>().RotateChildObjectToFacePlayerHeading(this.transform);

            // Freezes the parent player character in place for the specified duration.
            if (FreezeParentDuration > 0.0f)
                attackingCharacter.GetComponent<PlayerScript>().Freeze(FreezeParentDuration);

            // Shakes the camera.
            Camera.main.GetComponent<MainCameraScript>().Shake(Duration, ScreenShakeMagnitude);

            isActive = true;
        }
        // Else, throws an exception as two player characters are trying to initialise the same attack object.
        else
            throw new System.Exception("Player " + attackingCharacter.name + " is attempting to initialise an attack object which has already been initialised.");
    }

    // Whether the attack has been initialied and is now able to damage entities and despawn after the specified duration.
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
                Destroy(this.gameObject);
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
}