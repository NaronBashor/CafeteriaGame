using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameOverText : MonoBehaviour
{
        [SerializeField] GameObject player;
        [SerializeField] GameObject moneyTextCounter;

        TextMeshProUGUI moneyText;

        public int CustomersServed
        {
                get
                {
                        return player.GetComponent<Animator>().GetInteger("customersServed");
                }
        }

        private void Awake()
        {
                moneyText = moneyTextCounter.GetComponent<TextMeshProUGUI>();
                GameObject roundOver = GameObject.Find("RoundOverScreen");
                roundOver.SetActive(false);
        }

        private void Update()
        {
                moneyText.text = "You have served " + CustomersServed.ToString() + " customers this round!  Great Job!";
        }
}
