using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public Transform playerCamera;
    public Animator eyeAnimator; // ???? ???????? Canvas ? ?????????

    [Header("Movement Settings")]
    public float speed = 5f;
    public float gravity = -9.81f;
    private Vector3 velocity;

    [Header("Look Settings")]
    public float mouseSensitivity = 15f;
    private float xRotation = 0f;

    [Header("Interaction")]
    public float interactDistance = 3f;

    private bool canMove = false; // ?????????? ??????????

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // 1. ???? ??????: ????????????? ???????? ???????? ? ??????
        xRotation = 0f;
        playerCamera.localRotation = Quaternion.Euler(0f, -92f, 0f);

        // 2. ????????? ???????? ???????? "???????? ????"
        StartCoroutine(WaitUntilAwake());
    }

    IEnumerator WaitUntilAwake()
    {
        // ????, ????????, 3 ??????? (????? ????? ????????)
        yield return new WaitForSeconds(3f);
        canMove = true;
    }

    void Update()
    {
        // ???? ??? ?????? ?????? — ??????? ?? ??????
        if (!canMove) return;

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        if (keyboard == null || mouse == null) return;

        // --- 1. ??????? ?????? ---
        Vector2 mouseDelta = mouse.delta.ReadValue() * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseDelta.x);

        // --- 2. ???????? ---
        float x = 0;
        float z = 0;

        if (keyboard.wKey.isPressed) z = 1f;
        if (keyboard.sKey.isPressed) z = -1f;
        if (keyboard.aKey.isPressed) x = -1f;
        if (keyboard.dKey.isPressed) x = 1f;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // --- 3. ?????? ---
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- 4. ???????? ---
        if (keyboard.eKey.wasPressedThisFrame)
        {
            DoInteract();
        }
    }

    void DoInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("ElectricBox"))
            {
                Debug.Log("????? ?????!");
            }
        }
    }
}