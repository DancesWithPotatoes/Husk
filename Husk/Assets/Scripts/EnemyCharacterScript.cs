//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        18/03/17
// Date last edited:    24/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// A script used to handle the actions of an enemy character derived from the abstract Character script.
public class EnemyCharacterScript : CharacterScript
{
    // The time in seconds between each attack performed by the enemy.
    public float AttackRate;

    // Damages the enemy character.
    public override void Damage()
    {
        // Causes the character to flash red.
        if (IsColorFlashing)
            StopColorFlashing();
        FlashColor(Color.red, 0.2f);

        ResetAttackStatus();
    }


    // Updates the movement of the enemy character.
    protected override void UpdateMovement()
    {
        if (!IsFrozen)
        {
            
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