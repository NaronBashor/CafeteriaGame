using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class totalMoneyMainScreen : MonoBehaviour
{
        [SerializeField] TextMeshProUGUI tmpText;

        TextMeshProUGUI textMeshProUGUI;

        private void Awake()
        {
                textMeshProUGUI = tmpText.GetComponent<TextMeshProUGUI>();
                textMeshProUGUI.text = PlayerPrefs.GetInt("MoneyTotal").ToString();
        }

        private void Update()
        {
                textMeshProUGUI.text = PlayerPrefs.GetInt("MoneyTotal").ToString() ;
        }
}
