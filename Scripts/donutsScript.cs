using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class donutsScript : MonoBehaviour
{
        Collider2D coll;
        SpriteRenderer sprite;

        [SerializeField] private float playerDistance;
        [SerializeField] LayerMask playerMask;

        private GameObject player;

        private bool donutsOnePlate = false;
        private bool donutsTwoPlate = false;

        public float playerDirX
        {
                get
                {
                        return player.GetComponent<Animator>().GetFloat("dirX");
                }
        }

        public float playerDirY
        {
                get
                {
                        return player.GetComponent<Animator>().GetFloat("dirY");
                }
        }

        public bool OnePlate
        {
                get
                {
                        return player.GetComponent<Animator>().GetBool("onePlate");
                }
        }

        private void Awake()
        {
                coll = GetComponent<Collider2D>();
                sprite = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
                player = GameObject.FindGameObjectWithTag("Player");

                if (playerDirY <= 0)
                {
                        sprite.sortingOrder = 5;
                }
                else if (playerDirY > 0.01)
                {
                        sprite.sortingOrder = 1;
                }
                PlayerHere();
                if (donutsOnePlate)
                {
                        GameObject onePlate = GameObject.FindGameObjectWithTag("onePlate");
                        transform.position = onePlate.transform.position;
                        coll.enabled = false;
                }
                else if (donutsTwoPlate)
                {
                        GameObject twoPlate = GameObject.FindGameObjectWithTag("twoPlate");
                        transform.position = twoPlate.transform.position;
                        coll.enabled = false;
                }
        }

        public void OnMouseDown()
        {
                if (PlayerHere())
                {
                        player.GetComponent<PlayerController>().plates.Add(gameObject.name);
                        player.GetComponent<PlayerController>().carriedPlates.Add(gameObject);
                        if (!OnePlate)
                        {
                                donutsOnePlate = true;
                                donutsTwoPlate = false;
                        }
                        else if (OnePlate)
                        {
                                donutsOnePlate = false;
                                donutsTwoPlate = true;
                        }
                }
        }

        public bool PlayerHere()
        {
                RaycastHit2D hit = Physics2D.Raycast(coll.bounds.center, Vector2.down, playerDistance, playerMask);
                if (hit)
                {
                        Debug.DrawRay(coll.bounds.center, Vector2.down * playerDistance, Color.green);
                        return true;
                }
                else if (!hit)
                {
                        Debug.DrawRay(coll.bounds.center, Vector2.down * playerDistance, Color.red);
                        return false;
                }
                else
                {
                        return false;
                }
        }
}
