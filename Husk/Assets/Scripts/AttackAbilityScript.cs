//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        01/05/17
// Date last edited:    09/05/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
// An ability object which allows a parent character to perform an attack.
public class AttackAbilityScript : AbilityScript
{
    // Enumerated values representing the different stages in the lifetime of the attack ability object.
    public enum AttackStage
    {
        Windup,
        Active,
        Cooldown
    }
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
    // The amount of time in seconds for which a character hit by the attack object will be invincible.
    public float InvincibilityDuration;
    // The value used to scale the force of the knockback applied to any character hit by the attack object.
    public float KnockbackForce;
    // The amount of time in seconds for which a character hit by the attack object will be frozen and unable to act after the knockback effect has been endured.
    public float StaggerDuration;
    // The magnitude of the camera shake which occurs either whilst the attack is active if the attack object is attached to a player, or when the attack object damages the player if it is attached to an enemy.
    public float ScreenShakeMagnitude;
    // The duration of the hit stutter pause which occurs if the attack object damages another character.
    public float HitStutterDuration;

    // The property used to get the duration of the active stage of the attack object.
    public float ActiveDuration
    {
        get { return (Duration - (WindUpDuration + CooldownDuration)); }
    }


    // Called when the script is loaded.
    protected override void AwakeAddendum()
    {
        // Ensures that the windup and cooldown durations of the attack object are greater than or equal to zero.
        if (WindUpDuration < 0.0f)
            throw new System.Exception("The WindUpDuration value of the attack ability object must be greater than or equal to zero.");
        if (CooldownDuration < 0.0f)
            throw new System.Exception("The CooldownDuration value of the attack ability object must be greater than or equal to zero.");
        // Ensures that the windup and cooldown duration values fit within the overall lifetime duration of the attack object while also providing an active hitbox window.
        if (ActiveDuration <= 0.0f)
            throw new System.Exception("The combined values of the WindUpDuration and CooldownDuration must be less than the overall Duration value in order to allow a valid hitbox activation window.");
        // Ensures that the rest of the public member variables are all greater than or equal to zero.
        if (InvincibilityDuration < 0.0f)
            throw new System.Exception("The InvincibilityDuration value of the attack ability object must be greater than or equal to zero.");
        if (KnockbackForce < 0.0f)
            throw new System.Exception("The KnockbackForce value of the attack ability object must be greater than or equal to zero.");
        if (StaggerDuration < 0.0f)
            throw new System.Exception("The StaggerDuration value of the attack ability object must be greater than or equal to zero.");
        if (ScreenShakeMagnitude < 0.0f)
            throw new System.Exception("The ScreenShakeMagnitude value of the attack ability object must be greater than or equal to zero.");
        if (HitStutterDuration < 0.0f)
            throw new System.Exception("The HitStutterDuration value of the attack ability object must be greater than or equal to zero.");


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

        // Sets the initial attack stage.
        if (WindUpDuration > 0.0f)
        {
            attackStage = AttackStage.Windup;
            // Disables the hitbox until the attack is active.
            IsHitboxEnabled = false;
        }
        else
            attackStage = AttackStage.Active;

        // Decides which group of characters the attack object will damage based on the tag of it's parent.
        if (parentCharacter.tag == "Player")
            damageGroup = CharacterDamageGroup.Enemy;
        else if (parentCharacter.tag == "Enemy")
            damageGroup = CharacterDamageGroup.Player;

        // Causes the character to flash white in to provide the player with feedback on the windup, active, and cooldown periods of the attack.
        if (parentCharacterScript.IsColorFlashing)
            parentCharacterScript.StopColorFlashing();
        parentCharacterScript.FlashColor(Color.white, WindUpDuration, Duration - (WindUpDuration + CooldownDuration), CooldownDuration);
    }

    // Called each frame and used to update gameplay logic.
    protected override void UpdateAddendum()
    {
        // If the windup is complete, enables the attack object hitbox.
        if (attackStage == AttackStage.Windup && WindUpDuration > 0.0f && existTime > WindUpDuration)
        {
            attackStage = AttackStage.Active;
            IsHitboxEnabled = true;

            // The script used to handle the parent character.
            CharacterScript parentCharacterScript = parentCharacter.GetComponent<CharacterScript>();
            // Freezes the parent character in place while the attack is active.
            if (parentCharacterScript.IsFrozen)
                parentCharacterScript.Unfreeze();
            parentCharacterScript.Freeze(ActiveDuration);

            // If the attack object is attached to a player character, causes the screen to shake whilst it is active.
            if (ScreenShakeMagnitude > 0.0f && damageGroup == CharacterDamageGroup.Enemy)
            {
                // The script used to handle the main camera.
                MainCameraScript cameraScript = Camera.main.GetComponent<MainCameraScript>();
                // Shakes the camera while the attack is active.
                if (cameraScript.IsShaking)
                    cameraScript.StopShaking();
                cameraScript.Shake(ActiveDuration, ScreenShakeMagnitude);
            }
        }
        // Else if the active duration of the attack object hitbox has been completed, disables the hitbox.
        else if (attackStage == AttackStage.Active && CooldownDuration > 0.0f && existTime > (WindUpDuration + ActiveDuration))
        {
            attackStage = AttackStage.Cooldown;
            IsHitboxEnabled = false;
        }
    }

    // The current attack stage of the ability object.
    private AttackStage attackStage;
    // The group of characters which the attack object will damage.
    private CharacterDamageGroup damageGroup;

    // The property used to get and set whether the hitbox of the attack object is enabled or not.
    private bool IsHitboxEnabled
    {
        get { return (GetComponent<Collider2D>().enabled && GetComponentInChildren<SpriteRenderer>().enabled); }
        set
        {
            GetComponent<Collider2D>().enabled = value;
            GetComponentInChildren<SpriteRenderer>().enabled = value;
        }
    }

    // Called when another object enters the trigger collider of the attack object.
    private void OnTriggerStay2D(Collider2D otherCollider)
    {
        // If the hitbox is active and the other collider belongs to a character, attempts to damage it.
        if (IsInitialised && !IsPaused && otherCollider.transform != parentCharacter)
        {
            // Ensures that the attack object will only damage the correct group of characters.
            if (damageGroup == CharacterDamageGroup.Player && otherCollider.GetComponent<PlayerCharacterScript>() != null ||
                damageGroup == CharacterDamageGroup.Enemy && otherCollider.GetComponent<EnemyCharacterScript>() != null)
            {
                // Applies the various damage effects of the attack object to the other character.
                otherCollider.GetComponent<CharacterScript>().Damage(parentCharacter.GetComponent<CharacterScript>().Heading * KnockbackForce, InvincibilityDuration, StaggerDuration, ScreenShakeMagnitude, HitStutterDuration);               
            }
        }
    }
}