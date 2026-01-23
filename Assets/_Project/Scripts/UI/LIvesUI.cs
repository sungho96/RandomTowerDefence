using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LIvesUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livesText;

    private void Reset()
    {
        livesText = GetComponent<TextMeshProUGUI>();
    }


    private void Update()
    {
        //GameState에있는 lives데이터를 가져와 화면에 표시
        if (GameState.Instance == null) return;
        livesText.text =  $"Lives: {GameState.Instance.Lives}"; 
    }

}
