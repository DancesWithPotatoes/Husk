//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        28/03/17
// Date last edited:    19/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
// A script used to control a temporary attack collider object spawned by a character in the game.
public class AttackScript : MonoBehaviour
{
    // Enumerated values representing the different groups of characters which the attack object can effect.
    public enum CharacterDamageGroup
    {
        Player,
        Enemy
    }

    // The amount of time in seconds for which the attack object exists before self-destructing.
    public float Duration = 0.2f;
    // The amount of time for which the attack freezes the game character that created it of in place after being spawned.
    public float FreezeAttackerDuration = 0.2f;
    // The distance moved by the camera when it is shaking throughout the duration of the attack object being active.
    public float ScreenShakeMagnitude = 0.025f;

    // Initialises the attack object utilising the character which spawned it.
    public void InitialiseUsingCharacter(Transform attackingCharacter, CharacterDamageGroup characterDamageGroup)
    {
        // Initialises the attack object if it has not already been initialised.
        if (!isActive)
        {
            this.transform.SetParent(attackingCharacter);
            // Rotates the attack object to face in the same direction as the heading of the character which spawned it.
            attackingCharacter.GetComponent<CharacterScript>().RotateChildObjectToFaceCharacterHeading(this.transform);

            // Sets the group of characters which the attack will damage.
            this.damageGroup = characterDamageGroup;

            // Freezes the parent character in place for the specified duration.
            if (FreezeAttackerDuration > 0.0f)
                attackingCharacter.GetComponent<CharacterScript>().Freeze(FreezeAttackerDuration);

            // Shakes the camera.
            Camera.main.GetComponent<MainCameraScript>().Shake(Duration, ScreenShakeMagnitude);

            isActive = true;
        }
        // Else, throws an exception as the attack object has already been initialised.
        else
            throw new System.Exception(attackingCharacter.name + " is attempting to initialise an attack object which has already been initialised.");
    }


    // The group of characters which the attack object will damage.
    private CharacterDamageGroup damageGroup;
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
        // If the other collider belongs to a character, attempts to damage it.
        if (isActive && otherCollider.GetComponent<CharacterScript>() != null)
        {
            // Ensures that the attack object will only damage the correct group of characters.
            if (damageGroup == CharacterDamageGroup.Player && otherCollider.GetComponent<PlayerCharacterScript>() != null ||
                damageGroup == CharacterDamageGroup.Enemy && otherCollider.GetComponent<EnemyCharacterScript>() != null)
                otherCollider.GetComponent<CharacterScript>().Damage();
        }
    }
}