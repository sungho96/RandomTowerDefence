using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class WaveTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveText;

    private void Reset()
    {
        waveText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (GameState.Instance == null) return;
        waveText.text = $"Wave : {GameState.Instance.Wave}";
    }
}
