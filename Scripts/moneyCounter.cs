using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class moneyCounter : MonoBehaviour
{
        [SerializeField] private GameObject tmp_moneyCounter;
        [SerializeField] private GameObject player;

        TextMeshProUGUI tmp_MoneyCounterText;

        public int CurrentMoney
        {
                get
                {
                        return player.GetComponent<Animator>().GetInteger("moneyTotal");
                }
        }

        private void Awake()
        {
                tmp_MoneyCounterText = tmp_moneyCounter.GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
                tmp_MoneyCounterText.text = CurrentMoney.ToString();
        }
}
