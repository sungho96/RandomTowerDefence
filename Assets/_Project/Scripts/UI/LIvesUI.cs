using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LIvesUI : MonoBehaviour
{
    [SerializeField] private TMP_Text livesText;

    private void Update()
    {
        if (GameState.Instance == null) return;
        livesText.text =  $"Lives: {GameState.Instance.Lives}"; 
    }

}
