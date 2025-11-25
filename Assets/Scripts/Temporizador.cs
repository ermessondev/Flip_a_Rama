using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Temporizador : MonoBehaviour
{
    [SerializeField] public float tempoRelogio = 30.0f;
    [SerializeField] private float velocidadeRelogio = 1f;

    [SerializeField] private ArenaManager arenaManager;

    [Header("Temporizador UI")]
    [SerializeField] TextMeshProUGUI textoRelogio;

    public float tempoRestante;
    private const float intervaloBase = 1f;

    private void Start()
    {
        
    }

    public void IniciarRelogio()
    {
        tempoRestante = tempoRelogio;
        UpdateTimerText();
        StartCoroutine(TemporizadorCoroutine());
    }

    IEnumerator TemporizadorCoroutine()
    {
        float tempoEspera = intervaloBase / velocidadeRelogio;

        while (tempoRestante > 0)
        {
            yield return new WaitForSeconds(tempoEspera);
            tempoRestante -= intervaloBase;

            UpdateTimerText();

            if (tempoRestante <= 30)
            {
                textoRelogio.color = Color.yellow;
            }
        }
        TemporizadorTerminou();
    }
    void UpdateTimerText()
    {
        textoRelogio.text = tempoRestante.ToString("0");

    }

    void TemporizadorTerminou()
    {
        Debug.Log("Acabou o Tempo");
        StartCoroutine(arenaManager.FinalGame());
    }



}
