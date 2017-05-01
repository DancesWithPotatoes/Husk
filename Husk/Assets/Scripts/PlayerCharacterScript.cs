//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        18/04/17
// Date last edited:    01/05/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// A script used to handle the actions of a player-controlled character derived from the abstract Character script.
public class PlayerCharacterScript : CharacterScript
{
    //// A list of non-combo attack objects which the player can use as prefabs to spawn attacks.
    //public List<Transform> MoveList;
    // TEST - an ability for the player to test implementing.
    public Transform TestAbility;


    // Called when the script is loaded.
    protected override void AwakeAddendum()
    {
        //// Ensures that each of the gameobjects in the move list are valid attack objects.
        //foreach (Transform attack in MoveList)
        //{
        //    if(attack.GetComponent<AttackScript>() == null)
        //        throw new System.Exception("Any gameobjects in the MoveList of the player character must have an AttackScript component attached.");
        //}
    }

    // Called when the player character has been damaged.
    protected override void DamageAddendum()
    {
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
        //if (!IsAttacking)
        //{
        //    if (Input.GetButtonDown("Attack"))  
        //        Attack(MoveList[0]);
        //    if (Input.GetButtonDown("Launcher"))
        //        Attack(MoveList[1]);
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Transform testAbility = Instantiate(TestAbility).transform;
            testAbility.GetComponent<AbilityScript>().InitialiseUsingCharacter(this.transform);
        }
    }


    //// Spawns a self-destructing attack object from the specified attack prefab.
    //private void Attack(Transform attackPrefab)
    //{
    //    // Ensures that no attack objects already exist.
    //    if (IsAttacking)
    //        throw new System.InvalidOperationException("The player character already has an active attack object currently spawned.");

    //    // Spawns a new attack object and initialises it.
    //    Transform attackObject = (Transform)Instantiate(attackPrefab, this.transform.position, Quaternion.identity);
    //    attackObject.GetComponent<AttackScript>().InitialiseUsingCharacter(this.transform, AttackScript.CharacterDamageGroup.Enemy, true);
    //}
}