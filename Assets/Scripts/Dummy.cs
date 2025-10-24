using UnityEngine;

public class Dammy : MonoBehaviour
{
    [SerializeField] private Animator oAnimator2;
    [SerializeField] private Movimentacao movimentacao; 
    [SerializeField] private bool dammyTomouDano;



    void Update()
    {
        dammyTomouDano = movimentacao.acertouDammy;

        oAnimator2.SetBool("TomouDano", dammyTomouDano);
    }
}