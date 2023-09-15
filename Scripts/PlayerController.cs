using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class PlayerController : MonoBehaviour
{
        Rigidbody2D rb;
        Animator anim;

        Vector2 moveInput;

        [SerializeField] private float speed;
        public List<string> plates = new List<string>();
        public List<GameObject> carriedPlates = new List<GameObject>();
        [SerializeField] private float endOfRoundTimer;
        [SerializeField] private float timerStart;
        [SerializeField] GameObject timerText;
        [SerializeField] Transform textTimerRect;
        [SerializeField] GameObject minutesObject;
        [SerializeField] GameObject secondsObject;
        [SerializeField] GameObject elapsedTimeObject;
        [SerializeField] GameObject gameOverScreen;
        [SerializeField] private float timeElapsed;

        TextMeshProUGUI text;
        TextMeshProUGUI minutes;
        TextMeshProUGUI seconds;
        TextMeshProUGUI elapsedTime;

        RectTransform rectTransform;

        public bool CanMove
        {
                get
                {
                        return anim.GetBool("canMove");
                }
        }

        public bool RoundOver
        {
                get
                {
                        return anim.GetBool("roundOver");
                }
        }

        private void Awake()
        {
                rb = GetComponent<Rigidbody2D>();
                anim = GetComponent<Animator>();
                text = timerText.GetComponent<TextMeshProUGUI>();
                textTimerRect = textTimerRect.GetComponent<RectTransform>();
                minutes = minutesObject.GetComponent<TextMeshProUGUI>();
                seconds = secondsObject.GetComponent<TextMeshProUGUI>();
                elapsedTime = elapsedTimeObject.GetComponent<TextMeshProUGUI>();

                timerStart = endOfRoundTimer;
        }

        private void Update()
        {
                anim.SetFloat("dirX", moveInput.x);
                anim.SetFloat("dirY", moveInput.y);

                float min = Mathf.FloorToInt(endOfRoundTimer/60);
                float sec = Mathf.FloorToInt(endOfRoundTimer%60);
                minutes.text = "Minutes: " + min.ToString();
                seconds.text = "Seconds: " + sec.ToString();
                float min_e = Mathf.FloorToInt(timeElapsed/60);
                float sec_e = Mathf.FloorToInt(timeElapsed%60);
                elapsedTime.text = string.Format("Elapsed Time: {0,00}:{1,00}", min_e, sec_e);

                if (endOfRoundTimer > 0)
                {
                        endOfRoundTimer -= Time.deltaTime;
                        timeElapsed = timerStart - endOfRoundTimer;
                }
                if (endOfRoundTimer <= 0.01)
                {
                        gameOverScreen.SetActive(true);
                        text.text = "00:00";
                        anim.SetBool("roundOver", true);
                        anim.SetFloat("dirX", 0);
                        anim.SetFloat("dirY", 0);
                }
                else if (endOfRoundTimer > 0)
                {
                        text.text = string.Format("{0,00}:{1,00}", min, sec);
                }
        }

        private void FixedUpdate()
        {
                if (CanMove && !RoundOver)
                {
                        rb.AddForce(moveInput * speed * Time.fixedDeltaTime);
                }
                if (plates.Count == 1)
                {
                        anim.SetBool("onePlate", true);
                        anim.SetBool("twoPlates", false);
                        anim.SetBool("handsEmpty", false);
                }
                if (plates.Count == 2)
                {
                        anim.SetBool("onePlate", false);
                        anim.SetBool("twoPlates", true);
                        anim.SetBool("handsEmpty", false);
                }
                else if (plates.Count == 0)
                {
                        anim.SetBool("onePlate", false);
                        anim.SetBool("twoPlates", false);
                        anim.SetBool("handsEmpty", true);
                }
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
                if (CanMove && !RoundOver)
                {
                        moveInput = ctx.ReadValue<Vector2>();
                }
        }
}
