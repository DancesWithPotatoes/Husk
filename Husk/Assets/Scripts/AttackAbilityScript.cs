//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        01/05/17
// Date last edited:    01/05/17
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


    // Called when the script is loaded.
    protected override void AwakeAddendum()
    {
        // Sets the collider of the attack object to be a trigger so that it will detect when other colliders enter it without physically colliding with them.
        GetComponent<Collider2D>().isTrigger = true;
        // Sets the collider rigidbody to be kinematic so that the rigidbody doesn't physically interact with the scene - this collider must have a rigidbody attached to prevent it from being classified as static.
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    // Called after the InitialiseUsingCharacter() method has been executed.
    protected override void InitialiseUsingCharacterAddendum()
    {
        // Rotates the attack object to face in the same direction as the heading of the character which spawned it.
        parentCharacter.GetComponent<CharacterScript>().RotateChildObjectToFaceCharacterHeading(this.transform);

        // Decides which group of characters the attack object will damage based on the tag of it's parent.
        if (parentCharacter.tag == "Player")
            damageGroup = CharacterDamageGroup.Enemy;
        else if (parentCharacter.tag == "Enemy")
            damageGroup = CharacterDamageGroup.Player;
    }

    // Called each frame and used to update gameplay logic.
    protected override void UpdateAddendum()
    {

    }


    // The group of characters which the attack object will damage.
    private CharacterDamageGroup damageGroup;

    // Called when another object enters the trigger collider of the attack object.
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        // If the other collider belongs to a character, attempts to damage it.
        if (IsInitialised && !IsPaused && otherCollider.transform != parentCharacter)
        {
            // Ensures that the attack object will only damage the correct group of characters.
            if (damageGroup == CharacterDamageGroup.Player && otherCollider.GetComponent<PlayerCharacterScript>() != null ||
                damageGroup == CharacterDamageGroup.Enemy && otherCollider.GetComponent<EnemyCharacterScript>() != null)
                otherCollider.GetComponent<CharacterScript>().Damage();
        }
    }
}