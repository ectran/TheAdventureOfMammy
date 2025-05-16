using UnityEngine;
using Cinemachine;

[ExecuteAlways]
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class PixelPerfectCamera : MonoBehaviour
{
    public float pixelsPerUnit = 100f;

    private void FixedUpdate()
    {
        // Snap the virtual camera's position to the nearest pixel
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x * pixelsPerUnit) / pixelsPerUnit;
        pos.y = Mathf.Round(pos.y * pixelsPerUnit) / pixelsPerUnit;
        transform.position = pos;
    }
}
