using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deleteScore : MonoBehaviour
{
        public void DeleteScore()
        {
                PlayerPrefs.DeleteKey("MoneyTotal");
        }
}
