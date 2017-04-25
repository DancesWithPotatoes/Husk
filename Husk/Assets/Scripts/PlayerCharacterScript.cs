//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        18/04/17
// Date last edited:    25/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// A script used to handle the actions of a player-controlled character derived from the abstract Character script.
public class PlayerCharacterScript : CharacterScript
{
    // Damages the player character.
    public override void Damage()
    {
        // Causes the character to flash red.
        if (IsColorFlashing)
            StopColorFlashing();
        FlashColor(Color.red, 0.2f);

        // The script used to control the main camera.
        MainCameraScript cameraScript = Camera.main.GetComponent<MainCameraScript>();
        // Shakes the camera.
        if (cameraScript.IsShaking)
            cameraScript.StopShaking();
        cameraScript.Shake(0.2f, 0.05f);
    }

    // Updates the movement of the player character.
    protected override void UpdateMovement()
    {
        if (!IsFrozen)
        {
            // A vector representing the current values of the movement input axis.
            Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            // Clamps the movement magnitude to 1.0f so that the player will move at the same maximum speed in all directions.
            movement = Vector2.ClampMagnitude(movement, 1.0f);

            this.transform.Translate(movement * MoveSpeed * Time.deltaTime);

            // If the movement direction of the player has changed, updates the normalised heading vector.
            if (movement.magnitude > 0.0f && heading != movement.normalized)
                heading = movement.normalized;
        }
    }

    // Updates the attacking status of the player character.
    protected override void UpdateAttacking()
    {
        if (Input.GetButtonDown("LightAttack"))
        {
            if (!IsAttacking)
                Attack();
        }
    }


    // Spawns a self-destructing attack object which will damage enemies with which it collides.
    private void Attack()
    {
        // Ensures that no attack objects already exist.
        if (IsAttacking)
            throw new System.InvalidOperationException("The player character already has an active attack object currently spawned.");

        // Spawns a new attack object and initialises it.
        Transform attackObject = (Transform)Instantiate(AttackPrefab, this.transform.position, Quaternion.identity);
        attackObject.GetComponent<AttackScript>().InitialiseUsingCharacter(this.transform, AttackScript.CharacterDamageGroup.Enemy, true);
    }
}