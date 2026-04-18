using System;
using BezierSolution;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] public BezierAttachment bezierAttachment;
    [SerializeField] float speed;
    public IInteractable currentIInteractableObject;
    void Start()
    {
        G.inputActions.Enable();
        // G.inputActions.Player.Move.performed += Move;
        G.inputActions.Player.Interact.started += PerformInteract;
        G.Player = gameObject;
        //bezierAttachment = GetComponent<BezierAttachment>();
    }

    void Update()
    {   
        //bezierAttachment.normalizedT += speed * Time.deltaTime;
        Move();
    }

    private void Move()
    {
        float direction = 0f;
        if (Keyboard.current.wKey.isPressed)
        {
            direction = 1f;
        }
        else if(Keyboard.current.sKey.isPressed)
        {
            direction = -1f;
        }
        bezierAttachment.normalizedT += speed * Time.deltaTime * direction;

    }
    private void PerformInteract(InputAction.CallbackContext context)
    {
        if(currentIInteractableObject == null) return;

        currentIInteractableObject.Interact();
    }
}
