using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // --- ??????????? ?????? (??????????? ????? ???????) ---
    private static bool introPlayed = false;
    private static Vector3 lastPlayerPosition;
    private static Quaternion lastPlayerRotation;
    private static Quaternion lastCameraRotation;
    private static bool returningFrom2D = false;
    // ----------------------------------------------------

    [Header("Components")]
    public CharacterController controller;
    public Transform playerCamera;
    public Volume globalVolume;
    public AudioSource breathAudio;
    public TextMeshProUGUI objectiveText;

    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip[] woodSteps;
    public float stepInterval = 0.5f;
    private float stepTimer;

    [Header("Look Settings")]
    public float mouseSensitivity = 15f;
    private float xRotation = 0f;
    private float yRotation = -92f;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float gravity = -15f;
    private Vector3 velocity;

    [Header("Interaction")]
    public float interactDistance = 3f;

    private bool canMove = false;
    private DepthOfField dof;
    public AudioSource boatHorn;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // ????????? DOF
        if (globalVolume != null && globalVolume.profile.TryGet<DepthOfField>(out dof))
        {
            // ???? ????? ??? ????, ????? ?????? ????? ?? 10
            dof.focusDistance.Override(introPlayed ? 10f : 0.1f);
        }

        // ????????: ???? ?? ????????? ?? 2D ?????
        if (returningFrom2D)
        {
            // ????????? CharacterController ????????, ????? ??????????? ?????? (????? ?? ??????????????)
            controller.enabled = false;
            transform.position = lastPlayerPosition;
            transform.rotation = lastPlayerRotation;
            playerCamera.localRotation = lastCameraRotation;
            controller.enabled = true;

            canMove = true;
            returningFrom2D = false; // ?????????? ???? ????????
        }
        else
        {
            // ???? ??? ????? ?????? ?????? (Main Menu -> Game)
            playerCamera.localRotation = Quaternion.Euler(0f, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            if (!introPlayed)
            {
                StartCoroutine(WaitUntilAwake());
            }
            else
            {
                canMove = true; // ???? ????? ? ????? ???????? (?? ?? ?? 2D), ?????? ???? ??????
            }
        }

        if (objectiveText != null) objectiveText.color = new Color(1, 1, 1, 0);
    }

    IEnumerator WaitUntilAwake()
    {
        introPlayed = true; // ????????, ??? ????? ????????

        if (breathAudio != null) breathAudio.Play();
        float totalDuration = 10f;
        float moveUnlockTime = 5f;
        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / totalDuration;
            if (dof != null) dof.focusDistance.value = Mathf.Lerp(0.1f, 10f, t);
            if (!canMove && elapsed >= moveUnlockTime) canMove = true;
            yield return null;
        }

        if (dof != null) dof.focusDistance.value = 10f;
        yield return StartCoroutine(FadeInText());
    }

    // ... (???? ?????? FadeInText ? FadeOutText ??? ?????????) ...
    IEnumerator FadeInText() { float fadeDuration = 2f; float elapsed = 0f; while (elapsed < fadeDuration) { elapsed += Time.deltaTime; if (objectiveText != null) objectiveText.color = new Color(1, 1, 1, elapsed / fadeDuration); yield return null; } yield return new WaitForSeconds(5f); StartCoroutine(FadeOutText()); }
    IEnumerator FadeOutText() { float fadeDuration = 2f; float elapsed = 0f; while (elapsed < fadeDuration) { elapsed += Time.deltaTime; if (objectiveText != null) objectiveText.color = new Color(1, 1, 1, 1 - (elapsed / fadeDuration)); yield return null; } }

    void Update()
    {
        if (!canMove) return;

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        Vector2 mouseDelta = mouse.delta.ReadValue() * mouseSensitivity * Time.deltaTime;

        // ????????? xRotation ?? ?????? ???????? ???????? ??????, ???? ?? ?????? ??? ???????????
        if (returningFrom2D)
        {
            xRotation = playerCamera.localEulerAngles.x;
            if (xRotation > 180) xRotation -= 360f;
        }

        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseDelta.x);

        float x = 0; float z = 0;
        if (keyboard.wKey.isPressed) z = 1f;
        if (keyboard.sKey.isPressed) z = -1f;
        if (keyboard.aKey.isPressed) x = -1f;
        if (keyboard.dKey.isPressed) x = 1f;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // --- ?????? ????? ---
        bool isHit = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.3f);
        if (isHit && move.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }

        if (controller.isGrounded && velocity.y < 0) velocity.y = -5f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (keyboard.eKey.wasPressedThisFrame) DoInteract();
    }

    void PlayFootstep()
    {
        if (woodSteps.Length > 0 && footstepSource != null)
        {
            int index = Random.Range(0, woodSteps.Length);
            footstepSource.pitch = Random.Range(0.9f, 1.1f);
            footstepSource.PlayOneShot(woodSteps[index]);
        }
    }

    void DoInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("ElectricBox"))
            {
                // ????????? ??????? ????? ?????????
                lastPlayerPosition = transform.position;
                lastPlayerRotation = transform.rotation;
                lastCameraRotation = playerCamera.localRotation;
                returningFrom2D = true;

                SceneManager.LoadScene("TestCurveMove");
            }
        }
    }
}