//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        01/05/17
// Date last edited:    02/05/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
// An ability object which allows a parent character to perform an attack.
public class AttackAbilityScript : AbilityScript
{
    // Enumerated values representing the different groups of characters which the attack object can damage.
    public enum CharacterDamageGroup
    {
        Player,
        Enemy
    }

    // The amount of time for which the character prepares to attack before the attack object hitbox becomes activated.
    public float WindUpDuration;
    // The amount of time for which the attack ability object persists after the attack hitbox has been deactivated.
    public float CooldownDuration;
    // The amount of time for which the character that spawned the attack ability object is frozen in place after doing so.
    public float FreezeParentDuration;
    // The magnitude of the camera shake which persists throughout the lifetime of the attack object.
    public float ScreenShakeMagnitude;


    // Called when the script is loaded.
    protected override void AwakeAddendum()
    {
        // Ensures that the windup and cooldown durations of the attack object are greater than or equal to zero.
        if (WindUpDuration < 0.0f)
            throw new System.Exception("The WindUpDuration value of the ability object must be greater than or equal to zero.");
        if (CooldownDuration < 0.0f)
            throw new System.Exception("The CooldownDuration value of the ability object must be greater than or equal to zero.");
        // Ensures that the windup and cooldown duration values fit within the overall lifetime duration of the attack object while also providing an active hitbox window.
        if ((WindUpDuration + CooldownDuration) >= Duration)
            throw new System.Exception("The combined values of the WindUpDuration and CooldownDuration must be less than the overall Duration value in order to allow a valid hitbox activation window.");
        
        // Sets the collider of the attack object to be a trigger so that it will detect when other colliders enter it without physically colliding with them.
        GetComponent<Collider2D>().isTrigger = true;
        // Sets the collider rigidbody to be kinematic so that the rigidbody doesn't physically interact with the scene - this collider must have a rigidbody attached to prevent it from being classified as static.
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    // Called after the InitialiseUsingCharacter() method has been executed.
    protected override void InitialiseUsingCharacterAddendum()
    {
        // The script used to handle the parent character which spawned the attack ability object.
        CharacterScript parentCharacterScript = parentCharacter.GetComponent<CharacterScript>();

        // Rotates the attack object to face in the same direction as the heading of the character which spawned it.
        parentCharacterScript.RotateChildObjectToFaceCharacterHeading(this.transform);

        // Freezes the parent character in place for the specified duration.
        if (FreezeParentDuration > 0.0f)
        {
            if (parentCharacterScript.IsFrozen)
                parentCharacterScript.Unfreeze();
            parentCharacterScript.Freeze(FreezeParentDuration);
        }

        // Shakes the camera for the lifetime of the attack object.
        if (ScreenShakeMagnitude > 0.0f)
        {
            // The script used to handle the main camera.
            MainCameraScript cameraScript = Camera.main.GetComponent<MainCameraScript>();
            if (cameraScript.IsShaking)
                cameraScript.StopShaking();
            cameraScript.Shake(Duration, ScreenShakeMagnitude);
        }

        // Decides which group of characters the attack object will damage based on the tag of it's parent.
        if (parentCharacter.tag == "Player")
            damageGroup = CharacterDamageGroup.Enemy;
        else if (parentCharacter.tag == "Enemy")
            damageGroup = CharacterDamageGroup.Player;

        // If the attack has a wind-up, disables the hitbox until this period is complete.
        if (WindUpDuration > 0.0f)
            GetComponent<Collider2D>().enabled = false;
    }

    // Called each frame and used to update gameplay logic.
    protected override void UpdateAddendum() 
    {
        if (!GetComponent<Collider2D>().enabled && existTime >= WindUpDuration)
        {
            GetComponent<Collider2D>().enabled = true;

            if (parentCharacter.GetComponent<CharacterScript>().IsColorFlashing)
                parentCharacter.GetComponent<CharacterScript>().StopColorFlashing();
            parentCharacter.GetComponent<CharacterScript>().FlashColor(Color.white, Duration - (WindUpDuration + CooldownDuration));
        }
        else if (GetComponent<Collider2D>().enabled && existTime > (Duration - CooldownDuration))
        {
            GetComponent<Collider2D>().enabled = false;

            if (parentCharacter.GetComponent<CharacterScript>().IsColorFlashing)
                parentCharacter.GetComponent<CharacterScript>().StopColorFlashing();
            parentCharacter.GetComponent<CharacterScript>().FlashColor(Color.blue, CooldownDuration);
        }



        // Updates the color of the parent character to give clear player feedback on the attacking status.
        
    }


    // The group of characters which the attack object will damage.
    private CharacterDamageGroup damageGroup;

    // Called when another object enters the trigger collider of the attack object.
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        // If the hitbox is active and the other collider belongs to a character, attempts to damage it.
        if (IsInitialised && !IsPaused && otherCollider.transform != parentCharacter)
        {
            // Ensures that the attack object will only damage the correct group of characters.
            if (damageGroup == CharacterDamageGroup.Player && otherCollider.GetComponent<PlayerCharacterScript>() != null ||
                damageGroup == CharacterDamageGroup.Enemy && otherCollider.GetComponent<EnemyCharacterScript>() != null)
                otherCollider.GetComponent<CharacterScript>().Damage();
        }
    }
}