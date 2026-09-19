using UnityEngine;

/// <summary>
/// Attach this script ONLY to objects you want to be grabbable.
/// Objects without this script are completely ignored by the grab logic.
///
/// Requirements on the GameObject:
///   - A Collider (BoxCollider recommended, Is Trigger = unchecked)
///   - A Rigidbody
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class VRGrabbableObject : MonoBehaviour
{
    [Header("Grab Settings")]
    [Tooltip("How fast the object moves toward the held position")]
    public float followSpeed = 20f;

    [Header("Highlight Settings")]
    [Tooltip("Material to apply when laser is hovering over this object")]
    public Material highlightMaterial;

    // ── private state ──────────────────────────────────────────────
    private Rigidbody rb;
    private Collider col;
    private Renderer rend;
    private Material originalMaterial;

    private bool isGrabbed = false;
    private Transform grabAnchor;
    private float grabDistance;          // distance from controller when grabbed
    private Quaternion grabRotationOffset; // world rotation at grab time — never changes

    // ── Unity lifecycle ────────────────────────────────────────────
    private void Awake()
    {
        rb   = GetComponent<Rigidbody>();
        col  = GetComponent<Collider>();
        rend = GetComponentInChildren<Renderer>();

        if (rend != null)
            originalMaterial = rend.material;

        // Triggers are invisible to raycasts — must be false
        col.isTrigger = false;
    }

    private void FixedUpdate()
    {
        if (!isGrabbed) return;

        // Keep object at the same distance it was when grabbed,
        // always in front of the controller — never behind
        Vector3 targetPosition = grabAnchor.position + grabAnchor.forward * grabDistance;
        rb.velocity = (targetPosition - transform.position) * followSpeed;

        // Lock rotation completely — no spinning
        rb.angularVelocity = Vector3.zero;
        transform.rotation = grabRotationOffset;
    }

    // ── Public API called by ControllerRayFollower ─────────────────

    /// <summary>Called when the laser starts hovering over this object.</summary>
    public void OnHoverEnter()
    {
        if (highlightMaterial != null && rend != null)
            rend.material = highlightMaterial;
    }

    /// <summary>Called when the laser stops hovering over this object.</summary>
    public void OnHoverExit()
    {
        if (rend != null)
            rend.material = originalMaterial;
    }

    /// <summary>Called when the user pulls the trigger to grab.</summary>
    public void Grab(Transform anchor)
    {
        if (isGrabbed) return;

        isGrabbed  = true;
        grabAnchor = anchor;

        // Store how far the object is from the controller right now
        // — it will stay at this distance while held (far grab works correctly)
        grabDistance = Vector3.Distance(anchor.position, transform.position);
        grabDistance = Mathf.Max(grabDistance, 0.1f); // minimum 10cm so it never sits inside the hand

        // Lock the current world rotation — object never rotates while held
        grabRotationOffset = transform.rotation;

        rb.useGravity     = false;
        rb.isKinematic    = false;
        rb.freezeRotation = true;

        OnHoverExit(); // remove highlight while held
    }

    /// <summary>Called when the user releases the trigger.</summary>
    public void Release()
    {
        if (!isGrabbed) return;

        isGrabbed  = false;
        grabAnchor = null;

        rb.useGravity     = true;
        rb.freezeRotation = false; // restore normal physics so it falls naturally
    }

    public bool IsGrabbed => isGrabbed;
}