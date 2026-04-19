using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.SceneManagement; 
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public Transform playerCamera;
    public Volume globalVolume;
    public AudioSource breathAudio;
    public TextMeshProUGUI objectiveText;

    [Header("Look Settings")]
    public float mouseSensitivity = 15f;
    private float xRotation = 0f;
    private float yRotation = -92f;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float gravity = -9.81f;
    private Vector3 velocity;

    [Header("Interaction")]
    public float interactDistance = 3f; 

    private bool canMove = false;
    private DepthOfField dof;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera.localRotation = Quaternion.Euler(0f, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

        if (objectiveText != null) objectiveText.color = new Color(1, 1, 1, 0);

        if (globalVolume != null && globalVolume.profile.TryGet<DepthOfField>(out dof))
        {
            dof.focusDistance.Override(0.1f);
        }

        StartCoroutine(WaitUntilAwake());
    }

    IEnumerator WaitUntilAwake()
    {
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

    IEnumerator FadeInText()
    {
        float fadeDuration = 2f;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (objectiveText != null)
                objectiveText.color = new Color(1, 1, 1, elapsed / fadeDuration);
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
            if (objectiveText != null)
                objectiveText.color = new Color(1, 1, 1, 1 - (elapsed / fadeDuration));
            yield return null;
        }
    }

    void Update()
    {
        if (!canMove) return;

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        Vector2 mouseDelta = mouse.delta.ReadValue() * mouseSensitivity * Time.deltaTime;
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

        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

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
                SceneManager.LoadScene("NextSceneName");
            }
        }
    }
}