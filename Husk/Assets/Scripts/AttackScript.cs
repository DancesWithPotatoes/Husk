//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        25/03/17
// Date last edited:    25/03/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// A script used to handle an attack deployed by a weapon which will damage living entities that it hits.
public class AttackScript : MonoBehaviour
{
    // Called when the script is initialised.
    private void Start()
    {

    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {

    }

    // .
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Entered trigger of " + collider.name);
    }
}