using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class customerWaitingLine : MonoBehaviour
{
        CapsuleCollider2D coll;
        Rigidbody2D rb;
        Animator anim;

        [SerializeField] LayerMask endOfLine;
        [SerializeField] private float frontDistance;
        [SerializeField] private float speed;
        [SerializeField] private float customerWaitTime;

        private void Awake()
        {
                coll = GetComponent<CapsuleCollider2D>();
                rb = GetComponent<Rigidbody2D>();
                anim = GetComponent<Animator>();
                customerWaitTime = 0;
        }

        private void Update()
        {
                DistanceFront();
                customerWaitTime = customerWaitTime + Time.deltaTime;

                if (customerWaitTime > 20)
                {
                        anim.SetBool("bored", true);
                }
                if (customerWaitTime > 30)
                {
                        anim.SetBool("bored", false);
                        anim.SetBool("angry", true);
                }
                CustomerMood();
        }

        public void CustomerMood()
        {
                if (anim.GetBool("angry"))
                {
                        GameObject player = GameObject.FindGameObjectWithTag("Player");
                        player.GetComponent<Animator>().SetBool("cuStandAngry", true);
                }
        }

        private bool DistanceFront()
        {
                Vector2 location = new Vector2(coll.bounds.max.x, coll.bounds.min.y);
                RaycastHit2D hit = Physics2D.Raycast(location, Vector2.down, frontDistance, endOfLine);
                if (hit)
                {
                        Debug.DrawRay(location, Vector2.down * frontDistance, Color.green);
                        rb.velocity = Vector2.zero;
                        return true;
                }
                if (!hit)
                {
                        Debug.DrawRay(location, Vector2.down * frontDistance, Color.red);
                        rb.velocity = Vector2.down * speed;
                        return false;
                }
                else
                        return false;
        }
}
