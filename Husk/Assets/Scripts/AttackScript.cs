//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        28/03/17
// Date last edited:    22/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
// A script used to control a temporary attack collider object spawned by a character in the game to damage other characters.
public class AttackScript : MonoBehaviour
{
    // Enumerated values representing the different groups of characters which the attack object can damage.
    public enum CharacterDamageGroup
    {
        Player,
        Enemy
    }

    // The amount of time in seconds for which the attack object exists before self-destructing.
    public float ExistDuration;
    // The amount of time for which the character that spawned the attack is frozen in place after doing so.
    public float FreezeAttackerDuration;
    // The magnitude of the camera shake which exists throughout the lifetime of the attack object.
    public float ScreenShakeMagnitude;

    // Initialises the attack object utilising the character which spawned it - must be called instantly after the attack object has been spawned, otherwise an exception will be thrown.
    public void InitialiseUsingCharacter(Transform attackingCharacter, CharacterDamageGroup characterDamageGroup)
    {
        // Ensures that the attack-spawning object is a valid game character and that the attack object hasn't already been initialised.
        if (!attackingCharacter.GetComponent<CharacterScript>())
            throw new System.ArgumentException("The attack object must be initialised using an object which has a derivative of CharacterScript attached as a component.");
        if (isInitialised)
            throw new System.InvalidOperationException("The attack object has already been initialised.");

        this.transform.SetParent(attackingCharacter);
        // Rotates the attack object to face in the same direction as the heading of the character which spawned it.
        attackingCharacter.GetComponent<CharacterScript>().RotateChildObjectToFaceCharacterHeading(this.transform);
        // Freezes the parent character in place for the specified duration.
        if (FreezeAttackerDuration > 0.0f)
            attackingCharacter.GetComponent<CharacterScript>().Freeze(FreezeAttackerDuration);

        // Sets the group of characters which the attack will damage.
        this.damageGroup = characterDamageGroup;

        // Shakes the camera.
        if (Camera.main.GetComponent<MainCameraScript>().IsShaking)
            Camera.main.GetComponent<MainCameraScript>().StopShaking();
        Camera.main.GetComponent<MainCameraScript>().Shake(ExistDuration, ScreenShakeMagnitude);

        isInitialised = true;
    }


    // The group of characters which the attack object will damage.
    private CharacterDamageGroup damageGroup;
    // Whether the attack has been initialised and is now able to damage entities and despawn after the specified duration.
    private bool isInitialised;
    // The amount of time in seconds for which the attack object has existed since being spawned.
    private float destructionTimer;

    // Called when the script is loaded.
    private void Awake()
    {
        // Sets the collider of the attack to be a trigger so that it will detect when other colliders enter it without physically colliding with them.
        GetComponent<Collider2D>().isTrigger = true;
        // Sets the collider rigidbody to be kinematic so that the rigidbody doesn't physically interact with the scene - this collider must have a rigidbody attached to prevent it from being classified as static.
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    // Called when the script is initialised.
    private void Start()
    {
        isInitialised = false;
        destructionTimer = 0.0f;
    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        // Ensures that the attack object has been properly initialised.
        if (isInitialised == false)
            throw new System.InvalidOperationException("The attack object must be initialised using the InitialiseUsingCharacter() method directly after being spawned.");

        destructionTimer += Time.deltaTime;
        if (destructionTimer >= ExistDuration)
            Destroy(this.gameObject);
    }

    // Called when another object enters the trigger collider of the attack object.
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        // If the other collider belongs to a character, attempts to damage it.
        if (isInitialised && otherCollider.GetComponent<CharacterScript>() != null)
        {
            // Ensures that the attack object will only damage the correct group of characters.
            if (damageGroup == CharacterDamageGroup.Player && otherCollider.GetComponent<PlayerCharacterScript>() != null ||
                damageGroup == CharacterDamageGroup.Enemy && otherCollider.GetComponent<EnemyCharacterScript>() != null)
                otherCollider.GetComponent<CharacterScript>().Damage();
        }
    }
}