//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        18/03/17
// Date last edited:    25/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// A script used to handle the actions of an enemy character derived from the abstract Character script.
public class EnemyCharacterScript : CharacterScript
{
    // The distance from the player character at which the enemy character will stop chasing and try to attack it.
    public float AttackProximity;
    // The time in seconds between each attack performed by the enemy.
    public float AttackRate;


    // Called when the enemy character has been damaged.
    protected override void DamageSupplement()
    {
        ResetAttackStatus();
    }

    // Called when the script is loaded.
    protected override void AwakeSupplement()
    {
        // Stores the player character for future referencing.
        playerCharacter = GameObject.FindWithTag("Player").transform;
        if (playerCharacter == null)
            throw new System.Exception("The player character could not be found by the enemy character through searching for the 'Player' tag.");
    }

    // Updates the movement of the enemy character.
    protected override void UpdateMovement()
    {
        if (!IsFrozen)
        {
            // A vector from the enemy character to the player character.
            Vector2 toPlayer = playerCharacter.position - this.transform.position;            
            if (toPlayer != Vector2.zero)
            {
                // If the player isn't within the attack proximity, moves the enemy towards it.
                if (toPlayer.magnitude > AttackProximity)
                    this.transform.Translate(toPlayer.normalized * MoveSpeed * Time.deltaTime);

                // Updates the heading vector.
                heading = toPlayer.normalized;
            }
        }
    }

    // Updates the attacking status of the enemy character.
    protected override void UpdateAttacking()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= AttackRate)
        {
            Attack();
            ResetAttackStatus();
        }
    }


    // The player character which the enemy character will chase and attack.
    private Transform playerCharacter;
    // A the number of seconds since the enemy character last attacked.
    private float attackTimer = 0.0f;

    // Spawns a self-destructing attack object which will damage enemies with which it collides.
    private void Attack()
    {
        // Ensures that no attack objects already exist.
        if (IsAttacking)
            throw new System.InvalidOperationException("The enemy character already has an active attack object currently spawned.");

        // Spawns a new attack object and initialises it.
        Transform attackObject = (Transform)Instantiate(AttackPrefab, this.transform.position, Quaternion.identity);
        attackObject.GetComponent<AttackScript>().InitialiseUsingCharacter(this.transform, AttackScript.CharacterDamageGroup.Player);
    }

    // Resets the attacking status of the enemy character.
    private void ResetAttackStatus()
    {
        attackTimer = 0.0f;

        // Causes the character to gradually change from it's default color to white until it attacks.
        if (IsColorFlashing)
            StopColorFlashing();
        FlashColor(Color.white, AttackRate, AttackPrefab.GetComponent<AttackScript>().ExistDuration);
    }
}