using UnityEngine;

public class Dammy : MonoBehaviour
{
    [SerializeField] private Animator oAnimator2;
    [SerializeField] private Movimentacao scriptPlayer1; 
    [SerializeField] private bool dammyTomouDano;

   private void Awake()
    {
        // Se o campo estiver vazio no Inspector, tenta encontrar o script automaticamente
        if (scriptPlayer1 == null)
        {
            scriptPlayer1 = FindObjectOfType<Movimentacao>();
            if (scriptPlayer1 == null)
            {
                Debug.LogError("Nenhum script 'Movimentacao' foi encontrado na cena!");
            }
        }
    }

   private void OnTriggerEnter2D(Collider2D other)
   {
      Debug.Log($"{gameObject.name} entrou no limitador!");
      transform.position = new Vector3(0f, 0f, 20f);
   }

    void Update()
    {
      dammyTomouDano = scriptPlayer1.acertouDammy;
      oAnimator2.SetBool("TomouDano", dammyTomouDano);
    }
}