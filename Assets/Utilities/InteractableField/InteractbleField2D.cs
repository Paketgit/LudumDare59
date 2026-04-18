using BezierSolution;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class InteractbleField2D : MonoBehaviour, IInteractable
{
    [SerializeField] BezierSpline beziers;

    public void Interact()
    {
        PlayerMovement playerMovement = G.Player.GetComponent<PlayerMovement>();
        playerMovement.bezierAttachment.spline = beziers;
    }

    public void SetNextBezier(BezierSpline currentBezier)
    {
        if(beziers == null) return;
        currentBezier = beziers;
    }

    void OnTriggerStay2D(Collider2D other) {
        Debug.Log("Triggered " + other.gameObject.name);
    }
}
