//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        25/04/17
// Date last edited:    25/04/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// A script used to handle global actions that effect every gameplay object in the scene.
public class SceneControllerScript : MonoBehaviour
{
    public void PauseScene()
    {
        // TEST
        GameObject.FindWithTag("Player").GetComponent<PlayerCharacterScript>().enabled = false;

        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            enemy.GetComponent<EnemyCharacterScript>().enabled = false;

        foreach (GameObject attack in GameObject.FindGameObjectsWithTag("Attack"))
            attack.GetComponent<AttackScript>().enabled = false;


        Debug.Log("Paused.");
    }

    public void UnpauseScene()
    {
        // TEST
        GameObject.FindWithTag("Player").GetComponent<PlayerCharacterScript>().enabled = true;

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            enemy.GetComponent<EnemyCharacterScript>().enabled = true;

        foreach (GameObject attack in GameObject.FindGameObjectsWithTag("Attack"))
            attack.GetComponent<AttackScript>().enabled = true;


        Debug.Log("Unpaused.");
    }

    //// Causes all of the gameplay objects within the scene to temporarily 'stutter' before resuming
    //public void Stutter()
    //{
    //    // TEST
    //    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterScript>().enabled = false;
    //}


    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        // TEST
        if (Input.GetKeyDown(KeyCode.Space))
            PauseScene();
        if (Input.GetKeyDown(KeyCode.Return))
            UnpauseScene();
    }
}