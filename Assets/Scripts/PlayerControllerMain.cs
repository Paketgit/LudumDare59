using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // puzzleSolved — ????? ??????? (????? ??????)
    // gameFinished — ????????? ????????
    public static bool puzzleSolved = false;
    public static bool gameFinished = false;

    private static bool introPlayed = false;
    private static Vector3 lastPlayerPosition;
    private static Quaternion lastPlayerRotation;
    private static Quaternion lastCameraRotation;
    private static bool returningFrom2D = false;

    [Header("Components")]
    public CharacterController controller;
    public Transform playerCamera;
    public Volume globalVolume;
    public AudioSource breathAudio;
    public TextMeshProUGUI objectiveText;

    [Tooltip("???? ???????? ?????")]
    public AudioSource electricSparkSound;

    [Header("Eyes Animation")]
    public Animator eyesAnimator;
    public string idleStateName = "LidAnimation";

    [Header("UI & Final")]
    [Tooltip("?????? ? ??????? 'Success! Go to the upper deck'")]
    public GameObject winTextObject;
    public Animator shipAnimator;
    public string shipSailTrigger = "FinalSail";

    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip[] woodSteps;
    public float stepInterval = 0.5f;
    private float stepTimer;

    [Header("Settings")]
    public float mouseSensitivity = 15f;
    private float xRotation = 0f;
    private float yRotation = -92f;
    public float speed = 5f;
    public float gravity = -15f;
    private Vector3 velocity;
    public float interactDistance = 3f;

    private bool canMove = false;
    private DepthOfField dof;
    public AudioSource boatHorn;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (globalVolume != null && globalVolume.profile.TryGet<DepthOfField>(out dof))
        {
            dof.focusDistance.Override(introPlayed ? 10f : 0.1f);
        }

        // ???????? ???
        if (introPlayed || returningFrom2D)
        {
            if (eyesAnimator != null) eyesAnimator.Play(idleStateName, 0, 1f);
        }

        // ?????? ???????? ?? 2D
        if (returningFrom2D)
        {
            RestorePosition();
            canMove = true;
            returningFrom2D = false;

            // ???????? ????????? ?????
            if (puzzleSolved && winTextObject != null)
            {
                winTextObject.SetActive(true);
                StartCoroutine(HideObjectiveText(7f)); // ?????? ????? 7 ??????
            }

            // ????????? ?????
            if (puzzleSolved && electricSparkSound != null)
            {
                electricSparkSound.Stop();
            }
        }
        else
        {
            playerCamera.localRotation = Quaternion.Euler(0f, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            if (!introPlayed) StartCoroutine(WaitUntilAwake());
            else canMove = true;
        }

        // ?? ?????? ?????? ????? ???? ???? ??? ????? ??????, ???? ???? ??? ?????
        if (puzzleSolved && electricSparkSound != null) electricSparkSound.Stop();

        if (objectiveText != null) objectiveText.color = new Color(1, 1, 1, 0);
    }

    private void RestorePosition()
    {
        controller.enabled = false;
        transform.position = lastPlayerPosition;
        transform.rotation = lastPlayerRotation;
        playerCamera.localRotation = lastCameraRotation;
        controller.enabled = true;
        if (breathAudio != null) breathAudio.Stop();
    }

    IEnumerator HideObjectiveText(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (winTextObject != null) winTextObject.SetActive(false);
    }

    public IEnumerator FinalCutscene()
    {
        gameFinished = true;
        canMove = false;
        yield return new WaitForSeconds(1f);

        if (shipAnimator != null) shipAnimator.SetTrigger(shipSailTrigger);
        if (boatHorn != null) boatHorn.Play();

        yield return new WaitForSeconds(10f);
        Debug.Log("????? ????????.");
    }

    IEnumerator WaitUntilAwake()
    {
        introPlayed = true;
        if (breathAudio != null) breathAudio.Play();
        float elapsed = 0f;
        while (elapsed < 10f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 10f;
            if (dof != null) dof.focusDistance.value = Mathf.Lerp(0.1f, 10f, t);
            if (!canMove && elapsed >= 5f) canMove = true;
            yield return null;
        }
    }

    // ????? ????????? (?????? ??????????)
    IEnumerator FadeInText()
    {
        float fadeDuration = 2f;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (objectiveText != null) objectiveText.color = new Color(1, 1, 1, elapsed / fadeDuration);
            yield return null;
        }
        yield return new WaitForSeconds(5f);
        StartCoroutine(FadeOutText());
    }

    IEnumerator FadeOutText()
    {
        float fadeDuration = 2f;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (objectiveText != null) objectiveText.color = new Color(1, 1, 1, 1 - (elapsed / fadeDuration));
            yield return null;
        }
    }

    void Update()
    {
        if (!canMove || gameFinished) return;

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        // ???????
        Vector2 mouseDelta = mouse.delta.ReadValue() * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseDelta.x);

        // ??????
        float x = (keyboard.aKey.isPressed ? -1f : 0f) + (keyboard.dKey.isPressed ? 1f : 0f);
        float z = (keyboard.sKey.isPressed ? -1f : 0f) + (keyboard.wKey.isPressed ? 1f : 0f);

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // ???? (Footsteps) - ??????!
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

        // ??????????
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
        if (puzzleSolved || gameFinished) return;

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("ElectricBox"))
            {
                lastPlayerPosition = transform.position;
                lastPlayerRotation = transform.rotation;
                lastCameraRotation = playerCamera.localRotation;
                returningFrom2D = true;
                SceneManager.LoadScene("TestCurveMove");
            }
        }
    }
}