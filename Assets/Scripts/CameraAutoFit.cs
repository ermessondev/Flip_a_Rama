using UnityEngine;
using System.Collections;

public class CameraAutoFit : MonoBehaviour
{
    [SerializeField] private Transform p1, p2;
    [SerializeField] private float padding = 3f;
    [SerializeField] private float minSize = 12f;
    [SerializeField] private float maxSize = 25f;
    [SerializeField] private float zoomSmooth = 0.2f;
    [SerializeField] private float moveSmooth = 0.7f;

    private Camera cam;
    private float sizeVel;
    private Vector3 shakeOffset;
    private Vector3 currentVelocity; // <- usado pelo SmoothDamp da posição

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = GetComponentInChildren<Camera>();
    }

    void LateUpdate()
    {
        if (cam == null || p1 == null) return;

        // Calcula o centro entre os jogadores (ou só o P1 se for single)
        Vector3 targetCenter = (p2 != null)
            ? (p1.position + p2.position) / 2f
            : p1.position;

        // Aplica suavização de movimento + shake
        Vector3 smoothCenter = Vector3.SmoothDamp(
            transform.position,
            new Vector3(targetCenter.x, targetCenter.y, transform.position.z),
            ref currentVelocity,
            moveSmooth
        );

        transform.position = smoothCenter + shakeOffset;

        // Ajuste de zoom se tiver 2 jogadores
        if (p2 != null)
        {
            float width = Mathf.Abs(p1.position.x - p2.position.x);
            float height = Mathf.Abs(p1.position.y - p2.position.y);
            float halfWidth = width * 0.5f + padding;
            float halfHeight = height * 0.5f + padding;

            float targetSize = Mathf.Max(halfHeight, halfWidth / cam.aspect);
            targetSize = Mathf.Clamp(targetSize, minSize, maxSize);

            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref sizeVel, zoomSmooth);
        }
    }

    public void SetPlayers(Transform player1, Transform player2)
    {
        p1 = player1;
        p2 = player2;
    }

    #region CameraShake

    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;
        shakeOffset = Vector3.zero;

        while (elapsed < duration)
        {
            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * magnitude,
                Random.Range(-1f, 1f) * magnitude,
                0
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }
    #endregion
}
