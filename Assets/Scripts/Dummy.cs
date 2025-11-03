using UnityEngine;

public class Dammy : MonoBehaviour
{
    [SerializeField] private Animator oAnimator2;
    [SerializeField] private Movimentacao scriptPlayer1; 
    [SerializeField] private bool dammyTomouDano;

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