//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        25/03/17
// Date last edited:    28/03/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// A script used to handle the actions of the player-controlled character.
public class PlayerScript : MonoBehaviour
{
    // The prefab used which allows the player to spawn an attack object and damage enemies.
    public Transform AttackPrefab;
    // The maximum movement speed of the player character.
    public float MoveSpeed;

    // The property used to get whether the player currently has an attack object spawned.
    public bool IsAttacking
    {
        get { return (this.transform.childCount > 0); }
    }

    // Called when the script is initialised.
    private void Start()
    {

    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        UpdateMovement();
        UpdateAttacking();
    }

    // Updates the movement of the player character.
    private void UpdateMovement()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // Clamps the movement magnitude to 1.0f so that the player will move at the same maximum speed in all directions.
        movement = Vector2.ClampMagnitude(movement, 1.0f);

        this.transform.Translate(movement * MoveSpeed * Time.deltaTime);
    }

    // Updates the attacking status of the player.
    private void UpdateAttacking()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsAttacking)
        {
            Attack();
        }
    }

    // If the player isn't already attacking, spawns a self-destructing attack object which will damage enemies that it contacts.
    private void Attack()
    {
        if (!IsAttacking)
        {
            // Spawns a new attack object and sets it to be a child of the player character.
            Transform attackObject = (Transform)Instantiate(AttackPrefab, this.transform.position, new Quaternion());
            attackObject.transform.SetParent(this.transform);
        }
    }
}