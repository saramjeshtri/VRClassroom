using UnityEngine;

public class ControllerRayFollower : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private ColorPickerPanel lastHovered;
    private OVRCameraRig rig;

    // ── Grab state ─────────────────────────────────────────────────
    private VRGrabbableObject grabbedObject;
    private VRGrabbableObject hoveredGrabbable;

    [Header("Ray Settings")]
    [Tooltip("How far the laser ray reaches (increase for far grabbing)")]
    public float rayDistance = 50f;

    void Start()
    {
        rig = FindObjectOfType<OVRCameraRig>();

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.005f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        if (rig == null)
        {
            rig = FindObjectOfType<OVRCameraRig>();
            return;
        }

        Transform anchor = rig.rightHandAnchor;
        transform.position = anchor.position;
        transform.rotation = anchor.rotation;

        Ray ray = new Ray(transform.position, transform.forward);
        Vector3 endPoint = transform.position + transform.forward * rayDistance;

        bool triggerDown = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        bool triggerUp   = OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger,   OVRInput.Controller.RTouch);
        bool triggerHeld = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger,      OVRInput.Controller.RTouch);

        // ── Release grabbed object ─────────────────────────────────
        if (triggerUp && grabbedObject != null)
        {
            grabbedObject.Release();
            grabbedObject = null;
        }

        // ── If holding a grabbed object, skip raycasting ───────────
        if (grabbedObject != null)
        {
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor   = Color.green;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward * rayDistance);
            return;
        }

        // ── Raycast ────────────────────────────────────────────────
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        ColorPickerPanel hoveredThisFrame    = null;
        VRGrabbableObject grabbableThisFrame = null;

        foreach (RaycastHit hit in hits)
        {
            GameObject hitObj = hit.collider.gameObject;

            // 1. Drawing Logic
            WhiteboardDrawer canvas = hitObj.GetComponent<WhiteboardDrawer>();
            if (canvas != null)
            {
                endPoint = hit.point;
                if (triggerHeld)
                    canvas.DrawAtRaycastHit(hit);
                else
                    canvas.StopDrawing();
                break;
            }

            // 2. Grabbable object — only objects with VRGrabbableObject script
            VRGrabbableObject grabbable = hitObj.GetComponent<VRGrabbableObject>();
            if (grabbable != null)
            {
                grabbableThisFrame = grabbable;
                endPoint = hit.point;

                if (triggerDown)
                {
                    grabbedObject = grabbable;
                    grabbedObject.Grab(transform);
                    break;
                }
                break; // laser stops at this object even when not grabbing
            }

            // 3. Color Panel Hover Logic
            ColorPickerPanel hoveredPicker = hitObj.GetComponent<ColorPickerPanel>();
            if (hoveredPicker != null)
            {
                hoveredThisFrame = hoveredPicker;
            }

            // 4. Selection / Action Logic
            if (triggerDown)
            {
                if (hoveredPicker != null)
                {
                    hoveredPicker.SelectColor();
                    endPoint = hit.point;
                    break;
                }

                MapImageCycler cycler = hitObj.GetComponent<MapImageCycler>();
                if (cycler != null)
                {
                    cycler.OnLaserTrigger();
                    endPoint = hit.point;
                    break;
                }
            }
        }

        // ── Grabbable hover highlight ──────────────────────────────
        if (grabbableThisFrame != hoveredGrabbable)
        {
            if (hoveredGrabbable != null) hoveredGrabbable.OnHoverExit();
            if (grabbableThisFrame != null) grabbableThisFrame.OnHoverEnter();
            hoveredGrabbable = grabbableThisFrame;
        }

        // ── Color panel hover states ───────────────────────────────
        if (hoveredThisFrame != lastHovered)
        {
            if (lastHovered != null) lastHovered.SetHover(false);
            if (hoveredThisFrame != null) hoveredThisFrame.SetHover(true);
            lastHovered = hoveredThisFrame;
        }

        // ── Laser color feedback ───────────────────────────────────
        // Red   = nothing / normal
        // Yellow = hovering a grabbable object
        // Green  = currently holding a grabbed object
        if (grabbableThisFrame != null)
        {
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor   = Color.yellow;
        }
        else
        {
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor   = Color.red;
        }

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);
    }
}