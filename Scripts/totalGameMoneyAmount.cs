using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class totalGameMoneyAmount : MonoBehaviour
{
        [SerializeField] GameObject playerMoneyToAdd;
        [SerializeField] TextMeshProUGUI tmpText;

        private int moneyTotal;
        private int roundMoney;

        TextMeshProUGUI textMeshProUGUI;

        private void Awake()
        {
                textMeshProUGUI = tmpText.GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
                roundMoney = playerMoneyToAdd.GetComponent<Animator>().GetInteger("moneyTotal");
                textMeshProUGUI.text = roundMoney.ToString();
        }

        public void AddToTotalMoney()
        {                
                moneyTotal = PlayerPrefs.GetInt("MoneyTotal") + roundMoney;
                PlayerPrefs.SetInt("MoneyTotal", moneyTotal);
        }
}
