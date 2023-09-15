using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sortingOrderForCustomerPlayerCollision : MonoBehaviour
{
        private void OnTriggerEnter2D(Collider2D collision)
        {
                if (collision != null)
                {
                        collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
                }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
                collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
        }
}
