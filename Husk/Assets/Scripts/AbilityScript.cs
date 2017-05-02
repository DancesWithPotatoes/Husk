//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        29/04/17
// Date last edited:    01/05/17
//////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

// An abstract script used to implement the different types of actions/attacks which characters can perform by attaching 'ability objects' to them as children.
public abstract class AbilityScript : MonoBehaviour
{
    // Whether or not the ability object is currently locked into an unchanging state.
    [HideInInspector]
    public bool IsPaused { get; set; }
    // The duration in seconds for which the ability object is active before it self-destructs.
    public float Duration;

    // The property used to get whether the ability object has been initialised using a valid character.
    public bool IsInitialised
    {
        get { return (parentCharacter != null); }
    }

    // Initialises the ability object by utilising the specified character and attaching the ability object to it as a child.
    public void InitialiseUsingCharacter(Transform character)
    {
        // Ensures that the specified character object is valid and that the ability object hasn't already been initialised.
        if (character.GetComponent<CharacterScript>() == null)
            throw new System.ArgumentException("The ability object must be initialised using an object which has a derivative of CharacterScript attached to it as a component.");
        if (IsInitialised)
            throw new System.InvalidOperationException("The ability object has already been initialised.");

        parentCharacter = character;
        this.transform.position = parentCharacter.position;
        this.transform.SetParent(parentCharacter);

        InitialiseUsingCharacterAddendum();
    }


    // The parent character which the ability object effects.
    protected Transform parentCharacter;
    // The amount of time in seconds for which the ability object has existed.
    protected float existTime = 0.0f;

    // An empty virtual method which can be used by derived classes to add extra functionality to the InitialisedUsingCharacter() method.
    protected virtual void InitialiseUsingCharacterAddendum() { }

    // An empty virtual method which can be used by derived classes to add extra functionality to the Awake() method.
    protected virtual void AwakeAddendum() { }

    // An empty virtual method which can be used by derived classes to add extra functionality to the Update() method.
    protected virtual void UpdateAddendum() { }
    

    // Called when the script is loaded.
    private void Awake() 
    {
        // Ensures that the lifetime duration of the ability object is greater than zero.
        if (Duration <= 0.0f)
            throw new System.Exception("The Duration value of the ability object must be greater than zero.");

        AwakeAddendum();
    }

    // Called when the script is initialised.
    private void Start()
    {
        IsPaused = false;
    }

    // Called each frame and used to update gameplay logic.
    private void Update()
    {
        if (!IsPaused)
        {
            existTime += Time.deltaTime;
            if (existTime >= Duration)
                Destroy(this.gameObject);

            UpdateAddendum();
        }
    }
}