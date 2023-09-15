using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class customerQueueLine : MonoBehaviour
{
        [SerializeField] public List<GameObject> spawnedCustomers = new List<GameObject>();
        [SerializeField] private float delay;
        [SerializeField] Transform spawnLeft;
        [SerializeField] Transform spawnRight;
        [SerializeField] public List<GameObject> customerPrefabs = new List<GameObject>();
        [SerializeField] private int number = 0;
        [SerializeField] private int indexTwo;
        [SerializeField] private int customerCounter = 0;
        [SerializeField] private int customerIndex;
        [SerializeField] GameObject player;

        private void Update()
        {
                if (spawnedCustomers.Count < 8 && delay < .01f)
                {
                        Spawn();
                        delay = 2;
                }
                if (delay > 0)
                {
                        delay = delay - Time.deltaTime;
                }
                if (indexTwo >= customerPrefabs.Count)
                {
                        indexTwo = 0;
                }
        }

        public void Spawn()
        {
                GameObject customerLeft = Instantiate(customerPrefabs[indexTwo], spawnLeft.position, Quaternion.identity);
                customerLeft.name = "customer" + number;
                number = number + 1;
                indexTwo++;
                spawnedCustomers.Add(customerLeft);
                GameObject customerRight = Instantiate(customerPrefabs[indexTwo], spawnRight.position, Quaternion.identity);
                customerRight.name = "customer" + number;
                number = number + 1;
                indexTwo++;
                spawnedCustomers.Add(customerRight);
                customerCounter = customerCounter + 2;
        }

        public void DestroyCustomer()
        {
                GameObject customer = GameObject.Find("customer" + customerIndex);
                customerIndex++;
                GameObject customer2 = GameObject.Find("customer" + customerIndex);
                customerIndex++;
                Destroy(customer);
                Destroy(customer2);
                spawnedCustomers.RemoveAt(0);
                spawnedCustomers.RemoveAt(0);
        }
}
