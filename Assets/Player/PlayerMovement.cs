using System;
using BezierSolution;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] public BezierAttachment bezierAttachment;
    [SerializeField] float speed;

    void Start()
    {
        G.inputActions.Player.Move.performed += Move;
        G.Player = gameObject;
        //bezierAttachment = GetComponent<BezierAttachment>();
    }

    void Update()
    {   
        bezierAttachment.normalizedT += speed * Time.deltaTime;
    }

    private void Move(InputAction.CallbackContext context)
    {
        
    }

    void OnCollisionStay(Collision other) {
        Debug.Log("Triggered " + other.gameObject.name);
        if(other == null) return;
        if (Keyboard.current.fKey.isPressed)
        {
            IInteractable interactable = other.gameObject.GetComponent<IInteractable>();
            interactable?.Interact();
        }
    }

}
