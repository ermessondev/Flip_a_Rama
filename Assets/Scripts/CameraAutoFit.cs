using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraAutoFit : MonoBehaviour
{
    [SerializeField] private Transform p1, p2;
    [SerializeField] private float padding = 3f; // folga nas bordas
    [SerializeField] private float minSize = 12f;    // zoom máximo (chegar perto)
    [SerializeField] private float maxSize = 25f;   // zoom mínimo (afastar)
    [SerializeField] private float zoomSmooth = 0.2f; // suavização do zoom (segundos)

    private Camera cam;
    private float sizeVel; // usado pelo SmoothDamp

    void Awake()
    {
        string cenaAtual = SceneManager.GetActiveScene().name;
        if (cenaAtual != "Treinamento")
        {
            p1 = GetComponentInChildren<Transform>();
        }
        // Tenta achar a câmera no próprio objeto; se não, em filhos
        cam = GetComponent<Camera>();
        if (cam == null) cam = GetComponentInChildren<Camera>();
    }

    void LateUpdate()
    {
        if (cam == null || p1 == null || p2 == null) return;

        // Garantir que o objeto "meio" siga o centro dos dois
        // Meio inútil, decidi manter por hora.
        Vector3 centro = (p1.position + p2.position) / 2f;
        transform.position = new Vector3(centro.x, centro.y, transform.position.z);

        // Extensões entre os dois pontos
        float width = Mathf.Abs(p1.position.x - p2.position.x);
        float height = Mathf.Abs(p1.position.y - p2.position.y);

        float halfWidth = width * 0.5f + padding;
        float halfHeight = height * 0.5f + padding;

        // Para ortográfica: o tamanho tem que cobrir a metade da altura,
        float targetSize = Mathf.Max(halfHeight, halfWidth / cam.aspect);
        targetSize = Mathf.Clamp(targetSize, minSize, maxSize);

        // Suaviza o zoom (pode melhorar :/ )
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref sizeVel, zoomSmooth);
    }
    public void SetPlayers(Transform player1,Transform player2)
    {
        p1 = player1;
        p2 = player2;
    }
}
