using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kitchenFood : MonoBehaviour
{
        Animator anim;

        [SerializeField] Transform prefabLocation;
        [SerializeField] List<GameObject> foodItems = new List<GameObject>();

        private void Awake()
        {
                anim = GetComponent<Animator>();
        }

        public void Cook(int amount, int foodItemPrefab, string name, int tableNumber)
        {
                anim.SetBool("cooking", true);
                for (int i = 0; i < amount; i++)
                {
                        StartCoroutine(Delay());
                        IEnumerator Delay()
                        {
                                yield return new WaitForSeconds(3);

                                {
                                        GameObject foodInstance = Instantiate(foodItems[foodItemPrefab], prefabLocation.position, Quaternion.identity);
                                        foodInstance.name = "cooked" + name + tableNumber;
                                        anim.SetBool("cooking", false);
                                }
                        }
                }
        }
}
