using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class advanceSceneTimer : MonoBehaviour
{
        private void Update()
        {
                StartCoroutine(Delay());
                IEnumerator Delay()
                {
                        yield return new WaitForSeconds(5f);
                        SceneManager.LoadScene("MainMenu");
                }
        }
}
