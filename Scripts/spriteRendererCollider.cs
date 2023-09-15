using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spriteRendererCollider : MonoBehaviour
{
        private void OnTriggerEnter2D(Collider2D collision)
        {
                if (collision.CompareTag("burger") || collision.CompareTag("cake") || collision.CompareTag("chicken") || collision.CompareTag("donuts") || collision.CompareTag("egg") || collision.CompareTag("soup"))
                {
                        collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                }
        }
}
