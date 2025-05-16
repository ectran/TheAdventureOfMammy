using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Camera))]
public class PixelPerfectMCamera : MonoBehaviour
{
    public float pixelsPerUnit = 100f;

    private void FixedUpdate()
    {
        // Snap the *rendered* camera position, not just the virtual camera
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x * pixelsPerUnit) / pixelsPerUnit;
        pos.y = Mathf.Round(pos.y * pixelsPerUnit) / pixelsPerUnit;
        pos.z = -10f; // Lock z depth
        transform.position = pos;
    }
}
