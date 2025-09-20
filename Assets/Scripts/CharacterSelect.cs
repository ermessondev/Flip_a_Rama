using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private GameObject player1SelectReference;
    [SerializeField] private GameObject player2SelectReference;
    [SerializeField] private List<Button> listaPersonagens = new List<Button>();

    private GameObject seletorInstance;

    void Start()
    {
        selecaoPersonagens(GameManager.instance.singleMode);
    }

    void selecaoPersonagens(bool singleMode)
    {
        if (singleMode)
        {
            // Instancia como filho do botão 0
            seletorInstance = Instantiate(player1SelectReference, listaPersonagens[0].transform);

            // Garante que o seletor esteja centralizado e do tamanho do botão
            RectTransform seletorRect = seletorInstance.GetComponent<RectTransform>();
            RectTransform botaoRect = listaPersonagens[0].GetComponent<RectTransform>();

            seletorRect.anchoredPosition = Vector2.zero;
            seletorRect.sizeDelta = botaoRect.sizeDelta;

            // Opcional: força o seletor a ficar acima visualmente
            seletorInstance.transform.SetAsLastSibling();
        }
    }
}
