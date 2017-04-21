//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        18/03/17
// Date last edited:    21/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// A script used to handle the actions of a enemy character, derived from the abstract Character script.
public class EnemyCharacterScript : CharacterScript
{
    // The time in seconds between each enemy attack.
    public float AttackRate = 2.0f;


    // Updates the movement of the enemy character.
    protected override void UpdateMovement()
    {
        if (!isFrozen)
        {
            
        }
    }

    // Updates the attacking status of the enemy character.
    protected override void UpdateAttacking()
    {
        attackTimer += Time.deltaTime;

        if(!IsFlashing && Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(FlashColor(Color.white, (AttackRate / 2.0f), 0.2f));

        if (attackTimer >= AttackRate)
        {
            Attack();
            attackTimer = 0.0f;
        }
    }


    // A the number of seconds since the enemy character last attacked.
    private float attackTimer = 0.0f;

    // If the enemy isn't already attacking, spawns a temporary attack object which will damage enemies with which it collides.
    private void Attack()
    {
        if (!IsAttacking)
        {
            // Spawns a new attack object and sets it to be a child of the character.
            Transform attackObject = (Transform)Instantiate(AttackPrefab, this.transform.position, Quaternion.identity);
            attackObject.GetComponent<AttackScript>().InitialiseUsingCharacter(this.transform, AttackScript.CharacterDamageGroup.Player);
        }
    }
}