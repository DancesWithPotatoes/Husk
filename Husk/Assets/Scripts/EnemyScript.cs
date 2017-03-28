//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        28/03/17
// Date last edited:    28/03/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
// A script used to handle the actions of an enemy character.
public class EnemyScript : MonoBehaviour
{
    // Damages the enemy character.
    public void Damage()
    {
        Debug.Log(name + " has been damaged.");
    }

    // Called when the script is loaded.
    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = false;
        // Sets the enemy rigidbody to be kinematic so that it can be moved programmatically whilst still colliding with the physics objects in the scene.
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    // Called when the script is initialised.
    private void Start()
    {

    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {

    }
}