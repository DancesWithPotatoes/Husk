//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        25/03/17
// Date last edited:    25/03/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// A script used to handle the actions of the player-controlled character.
public class PlayerScript : MonoBehaviour
{
    // The maximum movement speed of the player character.
    public float MoveSpeed;

    // Called when the script is initialised.
    private void Start()
    {

    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        // The movement of the player character for this frame as taken from the player input device.
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // Ensures that the length of the movement vector is less than 1.0f so that the player won't move faster when travelling diagonally.
        movement = Vector2.ClampMagnitude(movement, 1.0f);
        // Translates the player character according to its given direction and movement speed.
        this.transform.Translate(movement * MoveSpeed * Time.deltaTime);
    }
}