using BezierSolution;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class InteractbleField2D : MonoBehaviour, IInteractable
{
    [SerializeField] BezierSpline beziers;
    [SerializeField] BezierPoint bezierPoint;
    [SerializeField] float TriggerRadius = 0.5f;
    float NewNormalizedT;
    CircleCollider2D circleCollider2D;

    void Start()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.radius = TriggerRadius;
        circleCollider2D.isTrigger = true;
    }
    public void Interact()
    {
        PlayerMovement playerMovement = G.Player.GetComponent<PlayerMovement>();
        if(bezierPoint.index == 0)
        {
            NewNormalizedT = 0;
        }
        else
        {
            NewNormalizedT = 1f / beziers.Count * bezierPoint.index;
        }
        Debug.Log("Index " + bezierPoint.index + " | normalizedT " + NewNormalizedT);
        playerMovement.bezierAttachment.normalizedT = NewNormalizedT;
        playerMovement.bezierAttachment.spline = beziers;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Triggered " + other.gameObject.tag);

            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            playerMovement.currentIInteractableObject = gameObject.GetComponent<InteractbleField2D>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Triggered " + other.gameObject.tag);

            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            playerMovement.currentIInteractableObject = null;
        }
    }
}
