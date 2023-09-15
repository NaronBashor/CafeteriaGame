using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.XR.Haptics;
using Unity.VisualScripting;
using System.Linq;
using System;
using System.Threading;

public class tableProgress : MonoBehaviour
{
        CapsuleCollider2D coll;
        Animator anim;

        private int seatsAvailable;
        private bool tookOrder = false;
        private bool waitingForFood = false;
        private bool reactedToFood = false;
        private bool dirtyPlatesAtTable = false;
        private bool angryUpdate = false;

        [SerializeField] public bool customersAngry = false;

        [SerializeField] List<string> tableProgressList = new List<string>();
        [SerializeField] List<Transform> openBackChairs = new List<Transform>();
        [SerializeField] List<Transform> openFrontChairs = new List<Transform>();
        [SerializeField] List<GameObject> customerFrontPrefabs = new List<GameObject>();
        [SerializeField] List<GameObject> customerBackPrefabs = new List<GameObject>();
        [SerializeField] List<GameObject> instantiatedPlayers = new List<GameObject>();
        [SerializeField] List<Transform> foodLocationForCustomers = new List<Transform>();
        [SerializeField] List<GameObject> foodItems = new List<GameObject>();
        [SerializeField] List<GameObject> foodItemsToRemoveFromTable = new List<GameObject>();
        [SerializeField] List<GameObject> itemsToBusFomTable = new List<GameObject>();
        [SerializeField] List<string> instantiatedFoodItems = new List<string>();
        [SerializeField] List<Transform> foodIcons = new List<Transform>();
        [SerializeField] List<int> randomNumbers = new List<int>();
        [SerializeField] Transform dirtyPlatesLocation;
        [SerializeField] private float playerDistance;
        [SerializeField] private int cuOrderDelay = 2;
        [SerializeField] LayerMask playerMask;
        [SerializeField] GameObject player;
        [SerializeField] GameObject dirtyPlates;
        [SerializeField] GameObject uiSeatTableText;
        [SerializeField] GameObject uiCashoutTableText;
        [SerializeField] GameObject tableBubble;
        [SerializeField] GameObject cashoutTableBubble;
        [SerializeField] GameObject eggChef;
        [SerializeField] GameObject burgerChef;
        [SerializeField] GameObject chickenChef;
        [SerializeField] GameObject soupChef;
        [SerializeField] GameObject cakeChef;
        [SerializeField] GameObject donutChef;
        [SerializeField] GameObject customerWaitLine;
        [SerializeField] private int currentState;
        [SerializeField] private float timeWaitingForFood;
        [SerializeField] private float timeTakenToEat;
        [SerializeField] private int tableNumber;
        [SerializeField] private int platesAtTable;
        [SerializeField] private float plateCount = 0;
        [SerializeField] private bool resetRandom = true;

        public bool HandsEmpty
        {
                get
                {
                        return player.GetComponent<Animator>().GetBool("handsEmpty");
                }
        }

        private int CurrentReward
        {
                get
                {
                        return anim.GetInteger("currentReward");
                }
                set
                {
                        anim.SetInteger("currentReward", value);
                }
        }

        private int CustomersServed
        {
                set
                {
                        anim.SetInteger("customersServed", value);
                }
        }

        private bool Eating
        {
                get
                {
                        return instantiatedPlayers[0].GetComponent<Animator>().GetBool("eating");
                }
        }

        private bool ReadyToOrder
        {
                get
                {
                        return anim.GetBool("readyToOrder");
                }
        }

        private bool TookOrder
        {
                get
                {
                        return anim.GetBool("tookOrder");
                }
        }

        private void Awake()
        {
                coll = GetComponent<CapsuleCollider2D>();
                anim = GetComponent<Animator>();
                uiSeatTableText.SetActive(true);
                uiCashoutTableText.SetActive(false);
                cashoutTableBubble.SetActive(false);
                tableBubble.SetActive(true);

                CurrentReward = 15;

                tableProgressList.Add("OpenTable");
                tableProgressList.Add("SeatTable");
                tableProgressList.Add("CanOrder");
                tableProgressList.Add("BringFood");
                tableProgressList.Add("BusTable");
                tableProgressList.Add("ReadyToSeat");

                currentState = 0;
        }

        private void Update()
        {
                platesAtTable = foodItemsToRemoveFromTable.Count;
                if (instantiatedPlayers.Count > 0 && !angryUpdate)
                {
                        if (instantiatedPlayers[0].GetComponent<Animator>().GetBool("angryWaitTime"))
                        {
                                RewardUpdate();
                        }
                }
                if (randomNumbers.Count < foodItems.Count && resetRandom)
                {
                        RandomNumber();
                }
                if (randomNumbers.Count == foodItems.Count)
                {
                        resetRandom = false;
                }
                if (currentState == 0)
                {
                        uiSeatTableText.SetActive(true);
                        tableBubble.SetActive(true);
                }
                else if (currentState != 0)
                {
                        uiSeatTableText.SetActive(false);
                        tableBubble.SetActive(false);
                }
                if (TookOrder && waitingForFood)
                {
                        timeWaitingForFood = timeWaitingForFood + Time.deltaTime;
                        if (openBackChairs.Count == 1)
                        {
                                instantiatedPlayers[0].GetComponent<Animator>().SetFloat("timeWaitingForFood", timeWaitingForFood);
                        }
                        if (openBackChairs.Count == 2)
                        {
                                instantiatedPlayers[0].GetComponent<Animator>().SetFloat("timeWaitingForFood", timeWaitingForFood);
                                instantiatedPlayers[1].GetComponent<Animator>().SetFloat("timeWaitingForFood", timeWaitingForFood);
                        }
                }
                if (currentState == 3 && plateCount == ((openBackChairs.Count * 2) + (openFrontChairs.Count * 2)))
                {
                        currentState = 4;
                        StartEating();
                }
                PlayerHere();
                if (currentState == 6)
                {
                        currentState = 0;
                        foodItemsToRemoveFromTable.Clear();
                        uiCashoutTableText.SetActive(false);
                        cashoutTableBubble.SetActive(false);
                        anim.SetBool("readyToOrder", false);
                        anim.SetBool("tookOrder", false);
                        anim.SetBool("foodRan", false);
                        resetRandom = true;
                }
        }

        private void StartEating()
        {
                currentState = 4;
                instantiatedFoodItems.Clear();
                randomNumbers.Clear();
                player.GetComponent<PlayerController>().carriedPlates.Clear();
                anim.SetBool("foodRan", true);
                timeWaitingForFood = 0;
                waitingForFood = false;
                if (platesAtTable <= 2)
                {
                        instantiatedPlayers[0].GetComponent<Animator>().SetFloat("timeWaitingForFood", 0);
                        instantiatedPlayers[0].GetComponent<Animator>().SetBool("eating", true);
                        reactedToFood = true;
                        StartCoroutine(EatDelay());
                        IEnumerator EatDelay()
                        {
                                yield return new WaitForSeconds(timeTakenToEat);
                                instantiatedPlayers[0].GetComponent<Animator>().SetBool("eating", false);
                                instantiatedPlayers[0].GetComponent<Animator>().SetTrigger("satisfied");
                                cashoutTableBubble.SetActive(true);
                                uiCashoutTableText.SetActive(true);
                                foreach (var objects in foodItemsToRemoveFromTable)
                                {
                                        Destroy(objects);
                                }
                                if (!dirtyPlatesAtTable)
                                {
                                        currentState = 5;
                                        AddDirtyPlates();
                                }
                        }
                }
                else if (platesAtTable > 2)
                {
                        instantiatedPlayers[0].GetComponent<Animator>().SetFloat("timeWaitingForFood", 0);
                        instantiatedPlayers[0].GetComponent<Animator>().SetBool("eating", true);
                        instantiatedPlayers[1].GetComponent<Animator>().SetFloat("timeWaitingForFood", 0);
                        instantiatedPlayers[1].GetComponent<Animator>().SetBool("eating", true);
                        reactedToFood = true;
                        StartCoroutine(EatDelay());
                        IEnumerator EatDelay()
                        {
                                yield return new WaitForSeconds(timeTakenToEat);
                                instantiatedPlayers[0].GetComponent<Animator>().SetBool("eating", false);
                                instantiatedPlayers[0].GetComponent<Animator>().SetTrigger("satisfied");
                                instantiatedPlayers[1].GetComponent<Animator>().SetBool("eating", false);
                                instantiatedPlayers[1].GetComponent<Animator>().SetTrigger("satisfied");
                                cashoutTableBubble.SetActive(true);
                                uiCashoutTableText.SetActive(true);
                                foreach (var objects in foodItemsToRemoveFromTable)
                                {
                                        Destroy(objects);
                                }
                                if (!dirtyPlatesAtTable)
                                {
                                        currentState = 5;
                                        AddDirtyPlates();
                                }
                        }
                }
        }

        private void AddDirtyPlates()
        {
                GameObject dirtyPlateStack = Instantiate(dirtyPlates, dirtyPlatesLocation.position, Quaternion.identity);
                itemsToBusFomTable.Add(dirtyPlateStack);
                dirtyPlateStack.name = "dirtyPlates" + tableNumber;
                dirtyPlatesAtTable = true;
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
                if (collision.CompareTag("burger") || collision.CompareTag("cake") || collision.CompareTag("chicken") || collision.CompareTag("donuts") || collision.CompareTag("egg") || collision.CompareTag("soup"))
                {
                        plateCount = plateCount + 1f;
                }
        }

        public void OnTriggerExit2D(Collider2D collision)
        {
                if (collision.CompareTag("burger") || collision.CompareTag("cake") || collision.CompareTag("chicken") || collision.CompareTag("donuts") || collision.CompareTag("egg") || collision.CompareTag("soup"))
                {
                        plateCount = plateCount - 1f;
                }
        }

        private void OnMouseDown()
        {
                if (PlayerHere())
                {
                        TableProgressUpdate();
                }
        }

        public void TableProgressUpdate()
        {
                if (currentState == 0 && HandsEmpty && customerWaitLine.GetComponent<customerQueueLine>().spawnedCustomers.Count >= openBackChairs.Count + openFrontChairs.Count)
                {
                        currentState++;
                }
                if (currentState == 1 && customerWaitLine.GetComponent<customerQueueLine>().spawnedCustomers.Count >= openBackChairs.Count + openFrontChairs.Count && HandsEmpty)
                {
                        SeatTable();
                        currentState++;
                }
                if (currentState == 2  && ReadyToOrder && !TookOrder && HandsEmpty)
                {
                        player.gameObject.GetComponent<Animator>().SetTrigger("takingOrder");
                        StartCoroutine(Delay());
                        IEnumerator Delay()
                        {
                                yield return new WaitForSeconds(2);
                                int indexOrder = 0;
                                for (int i = 0; i < openBackChairs.Count; i++)
                                {
                                        instantiatedPlayers[indexOrder].GetComponent<Animator>().SetBool("readMenu", false);
                                        instantiatedPlayers[indexOrder].GetComponent<Animator>().SetBool("readyToOrder", false);
                                        instantiatedPlayers[indexOrder].GetComponent<Animator>().SetBool("waitForFood", true);
                                        indexOrder++;
                                }
                                anim.SetBool("tookOrder", true);
                                tookOrder = true;
                                waitingForFood = true;
                                int index = 0;
                                int count = openBackChairs.Count + openFrontChairs.Count;
                                for (int i = 0; i < count; i++)
                                {
                                        int randInt = UnityEngine.Random.Range(0, randomNumbers.Count - 1);
                                        if (randomNumbers[randInt] == 0)
                                        {
                                                GameObject burgerInstance = Instantiate(foodItems[0], foodIcons[index].position, Quaternion.identity);
                                                burgerChef.GetComponent<kitchenFood>().Cook(1, 0, "burger", tableNumber);
                                                burgerInstance.name = "burger" + tableNumber;
                                                instantiatedFoodItems.Add("burger" + tableNumber);
                                        }
                                        if (randomNumbers[randInt] == 1)
                                        {
                                                GameObject cakeInstance = Instantiate(foodItems[1], foodIcons[index].position, Quaternion.identity);
                                                cakeChef.GetComponent<kitchenFood>().Cook(1, 1, "cake", tableNumber);
                                                cakeInstance.name = "cake" + tableNumber;
                                                instantiatedFoodItems.Add("cake" + tableNumber);
                                        }
                                        if (randomNumbers[randInt] == 2)
                                        {
                                                GameObject chickenInstance = Instantiate(foodItems[2], foodIcons[index].position, Quaternion.identity);
                                                chickenChef.GetComponent<kitchenFood>().Cook(1, 2, "chicken", tableNumber);
                                                chickenInstance.name = "chicken" + tableNumber;
                                                instantiatedFoodItems.Add("chicken" + tableNumber);
                                        }
                                        if (randomNumbers[randInt] == 3)
                                        {
                                                GameObject donutsInstance = Instantiate(foodItems[3], foodIcons[index].position, Quaternion.identity);
                                                donutChef.GetComponent<kitchenFood>().Cook(1, 3, "donut", tableNumber);
                                                donutsInstance.name = "donut" + tableNumber;
                                                instantiatedFoodItems.Add("donut" + tableNumber);
                                        }
                                        if (randomNumbers[randInt] == 4)
                                        {
                                                GameObject eggInstance = Instantiate(foodItems[4], foodIcons[index].position, Quaternion.identity);
                                                eggChef.GetComponent<kitchenFood>().Cook(1, 4, "egg", tableNumber);
                                                eggInstance.name = "egg" + tableNumber;
                                                instantiatedFoodItems.Add("egg" + tableNumber);
                                        }
                                        if (randomNumbers[randInt] == 5)
                                        {
                                                GameObject soupInstance = Instantiate(foodItems[5], foodIcons[index].position, Quaternion.identity);
                                                soupChef.GetComponent<kitchenFood>().Cook(1, 5, "soup", tableNumber);
                                                soupInstance.name = "soup" + tableNumber;
                                                instantiatedFoodItems.Add("soup" + tableNumber);
                                        }
                                        index++;
                                        randomNumbers.Remove(randomNumbers[randInt]);
                                }
                        }
                        currentState++;
                }
                if (currentState == 3)
                {
                        int foodIndex = 0;
                        int plateNumber = player.GetComponent<PlayerController>().plates.Count;
                        int orderedNumber = instantiatedFoodItems.Count;
                        if (plateNumber == 1 && orderedNumber > 0)
                        {
                                if (platesAtTable == 0)
                                {
                                        foodIndex = 0;
                                }
                                if (platesAtTable == 1)
                                {
                                        foodIndex = 1;
                                }
                                if (platesAtTable == 2)
                                {
                                        foodIndex = 2;
                                }
                                if (platesAtTable == 3)
                                {
                                        foodIndex = 3;
                                }
                                for (int i = 0; i < plateNumber; i++)
                                {
                                        if (player.GetComponent<PlayerController>().plates.Contains("cooked" + "burger" + tableNumber))
                                        {
                                                GameObject burger = GameObject.Find("burger" + tableNumber);
                                                burger.transform.position = foodLocationForCustomers[foodIndex].position;
                                                foodItemsToRemoveFromTable.Add(burger);
                                                player.GetComponent<PlayerController>().plates.Remove("cooked" + "burger" + tableNumber);
                                                player.GetComponent<PlayerController>().carriedPlates.Remove(GameObject.Find("cooked" + "burger" + tableNumber));
                                                Destroy(GameObject.Find("cooked" + "burger" + tableNumber));
                                        }
                                        else if (player.GetComponent<PlayerController>().plates.Contains("cooked" + "cake" + tableNumber))
                                        {
                                                GameObject cake = GameObject.Find("cake" + tableNumber);
                                                cake.transform.position = foodLocationForCustomers[foodIndex].position;
                                                foodItemsToRemoveFromTable.Add(cake);
                                                player.GetComponent<PlayerController>().plates.Remove("cooked" + "cake" + tableNumber);
                                                player.GetComponent<PlayerController>().carriedPlates.Remove(GameObject.Find("cooked" + "cake" + tableNumber));
                                                Destroy(GameObject.Find("cooked" + "cake" + tableNumber));
                                        }
                                        else if (player.GetComponent<PlayerController>().plates.Contains("cooked" + "chicken" + tableNumber))
                                        {
                                                GameObject chicken = GameObject.Find("chicken" + tableNumber);
                                                chicken.transform.position = foodLocationForCustomers[foodIndex].position;
                                                foodItemsToRemoveFromTable.Add(chicken);
                                                player.GetComponent<PlayerController>().plates.Remove("cooked" + "chicken" + tableNumber);
                                                player.GetComponent<PlayerController>().carriedPlates.Remove(GameObject.Find("cooked" + "chicken" + tableNumber));
                                                Destroy(GameObject.Find("cooked" + "chicken" + tableNumber));
                                        }
                                        else if (player.GetComponent<PlayerController>().plates.Contains("cooked" + "donut" + tableNumber))
                                        {
                                                GameObject donut = GameObject.Find("donut" + tableNumber);
                                                donut.transform.position = foodLocationForCustomers[foodIndex].position;
                                                foodItemsToRemoveFromTable.Add(donut);
                                                player.GetComponent<PlayerController>().plates.Remove("cooked" + "donut" + tableNumber);
                                                instantiatedFoodItems.Remove("donut" + tableNumber);
                                                player.GetComponent<PlayerController>().carriedPlates.Remove(GameObject.Find("cooked" + "donut" + tableNumber));
                                                Destroy(GameObject.Find("cooked" + "donut" + tableNumber));
                                        }
                                        else if (player.GetComponent<PlayerController>().plates.Contains("cooked" + "egg" + tableNumber))
                                        {
                                                GameObject egg = GameObject.Find("egg" + tableNumber);
                                                egg.transform.position = foodLocationForCustomers[foodIndex].position;
                                                foodItemsToRemoveFromTable.Add(egg);
                                                player.GetComponent<PlayerController>().plates.Remove("cooked" + "egg" + tableNumber);
                                                instantiatedFoodItems.Remove("egg" + tableNumber);
                                                player.GetComponent<PlayerController>().carriedPlates.Remove(GameObject.Find("cooked" + "egg" + tableNumber));
                                                Destroy(GameObject.Find("cooked" + "egg" + tableNumber));
                                        }
                                        else if (player.GetComponent<PlayerController>().plates.Contains("cooked" + "soup" + tableNumber))
                                        {
                                                GameObject soup = GameObject.Find("soup" + tableNumber);
                                                soup.transform.position = foodLocationForCustomers[foodIndex].position;
                                                foodItemsToRemoveFromTable.Add(soup);
                                                player.GetComponent<PlayerController>().plates.Remove("cooked" + "soup" + tableNumber);
                                                instantiatedFoodItems.Remove("soup" + tableNumber);
                                                player.GetComponent<PlayerController>().carriedPlates.Remove(GameObject.Find("cooked" + "soup" + tableNumber));
                                                Destroy(GameObject.Find("cooked" + "soup" + tableNumber));
                                        }
                                        foodIndex++;
                                }
                        }
                        else if (plateNumber == 2 && orderedNumber > 0)
                        {
                                if (platesAtTable == 0)
                                {
                                        foodIndex = 0;
                                }
                                if (platesAtTable == 1)
                                {
                                        foodIndex = 1;
                                }
                                if (platesAtTable == 2)
                                {
                                        foodIndex = 2;
                                }
                                if (platesAtTable == 3)
                                {
                                        foodIndex = 3;
                                }
                                for (int i = 0; i < plateNumber; i++)
                                {
                                        if (player.GetComponent<PlayerController>().plates.Contains("cooked" + "burger" + tableNumber))
                                        {
                                                GameObject burger = GameObject.Find("burger" + tableNumber);
                                                burger.transform.position = foodLocationForCustomers[foodIndex].position;
                                                foodItemsToRemoveFromTable.Add(burger);
                                                player.GetComponent<PlayerController>().plates.Remove("cooked" + "burger" + tableNumber);
                                                player.GetComponent<PlayerController>().carriedPlates.Remove(GameObject.Find("cooked" + "burger" + tableNumber));
                                                Destroy(GameObject.Find("cooked" + "burger" + tableNumber));
                                        }
                                        else if (player.GetComponent<PlayerController>().plates.Contains("cooked" + "cake" + tableNumber))
                                        {
                                                GameObject cake = GameObject.Find("cake" + tableNumber);
                                                cake.transform.position = foodLocationForCustomers[foodIndex].position;
                                                foodItemsToRemoveFromTable.Add(cake);
                                                player.GetComponent<PlayerController>().plates.Remove("cooked" + "cake" + tableNumber);
                                                player.GetComponent<PlayerController>().carriedPlates.Remove(GameObject.Find("cooked" + "cake" + tableNumber));
                                                Destroy(GameObject.Find("cooked" + "cake" + tableNumber));
                                        }
                                        else if (player.GetComponent<PlayerController>().plates.Contains("cooked" + "chicken" + tableNumber))
                                        {
                                                GameObject chicken = GameObject.Find("chicken" + tableNumber);
                                                chicken.transform.position = foodLocationForCustomers[foodIndex].position;
                                                foodItemsToRemoveFromTable.Add(chicken);
                                                player.GetComponent<PlayerController>().plates.Remove("cooked" + "chicken" + tableNumber);
                                                player.GetComponent<PlayerController>().carriedPlates.Remove(GameObject.Find("cooked" + "chicken" + tableNumber));
                                                Destroy(GameObject.Find("cooked" + "chicken" + tableNumber));
                                        }
                                        else if (player.GetComponent<PlayerController>().plates.Contains("cooked" + "donut" + tableNumber))
                                        {
                                                GameObject donut = GameObject.Find("donut" + tableNumber);
                                                donut.transform.position = foodLocationForCustomers[foodIndex].position;
                                                foodItemsToRemoveFromTable.Add(donut);
                                                player.GetComponent<PlayerController>().plates.Remove("cooked" + "donut" + tableNumber);
                                                player.GetComponent<PlayerController>().carriedPlates.Remove(GameObject.Find("cooked" + "donut" + tableNumber));
                                                Destroy(GameObject.Find("cooked" + "donut" + tableNumber));
                                        }
                                        else if (player.GetComponent<PlayerController>().plates.Contains("cooked" + "egg" + tableNumber))
                                        {
                                                GameObject egg = GameObject.Find("egg" + tableNumber);
                                                egg.transform.position = foodLocationForCustomers[foodIndex].position;
                                                foodItemsToRemoveFromTable.Add(egg);
                                                player.GetComponent<PlayerController>().plates.Remove("cooked" + "egg" + tableNumber);
                                                player.GetComponent<PlayerController>().carriedPlates.Remove(GameObject.Find("cooked" + "egg" + tableNumber));
                                                Destroy(GameObject.Find("cooked" + "egg" + tableNumber));
                                        }
                                        else if (player.GetComponent<PlayerController>().plates.Contains("cooked" + "soup" + tableNumber))
                                        {
                                                GameObject soup = GameObject.Find("soup" + tableNumber);
                                                soup.transform.position = foodLocationForCustomers[foodIndex].position;
                                                foodItemsToRemoveFromTable.Add(soup);
                                                player.GetComponent<PlayerController>().plates.Remove("cooked" + "soup" + tableNumber);
                                                player.GetComponent<PlayerController>().carriedPlates.Remove(GameObject.Find("cooked" + "soup" + tableNumber));
                                                Destroy(GameObject.Find("cooked" + "soup" + tableNumber));
                                        }
                                        foodIndex++;
                                }
                        }
                }
                if (currentState == 5 && HandsEmpty)
                {
                        int tip = anim.GetInteger("currentReward");
                        player.GetComponent<Animator>().SetInteger("moneyTotal", (player.GetComponent<Animator>().GetInteger("moneyTotal")) + tip);
                        player.GetComponent<Animator>().SetInteger("customersServed", (player.GetComponent<Animator>().GetInteger("customersServed")) + instantiatedPlayers.Count);
                        for (int i = 0; i < openFrontChairs.Count + openBackChairs.Count; i++)
                        {
                                Destroy(instantiatedPlayers[0]);
                                instantiatedPlayers.RemoveAt(0);
                        }
                        GameObject busTable = GameObject.Find("dirtyPlates" + tableNumber);
                        Destroy(busTable);
                        dirtyPlatesAtTable = false;
                        currentState = 6;
                }
        }

        public int RandomNumber()
        {
                int a = 0;
                while (a == 0)
                {
                        a = UnityEngine.Random.Range(0, foodItems.Count);
                        if (!randomNumbers.Contains(a))
                        {
                                randomNumbers.Add(a);
                        }
                }
                return a;
        }

        public void SeatTable()
        {
                if (player.GetComponent<Animator>().GetBool("cuStandAngry"))
                {
                        CurrentReward = 10;
                        GameObject player = GameObject.FindGameObjectWithTag("Player");
                        player.GetComponent<Animator>().SetBool("cuStandAngry", false);
                }
                int index = 0;
                seatsAvailable = openBackChairs.Count;
                for (int i = 0; i < seatsAvailable; i++)
                {
                        customerWaitLine.GetComponent<customerQueueLine>().DestroyCustomer();
                        int randomBack = UnityEngine.Random.Range(0, customerBackPrefabs.Count);
                        GameObject customerInstance = Instantiate(customerBackPrefabs[randomBack], openBackChairs[index].position, Quaternion.identity);
                        instantiatedPlayers.Add(customerInstance);
                        customerInstance.name = "Customer" + index;
                        customerInstance.GetComponent<Animator>().SetBool("readMenu", true);
                        StartCoroutine(OrderDelay());
                        IEnumerator OrderDelay()
                        {
                                yield return new WaitForSeconds(cuOrderDelay);
                                customerInstance.GetComponent<Animator>().SetBool("readyToOrder", true);
                                anim.SetBool("readyToOrder", true);
                        }
                        index++;
                }
                int secondIndex = 0;
                seatsAvailable = openFrontChairs.Count;
                for (int i = 0; i < seatsAvailable; i++)
                {
                        int randomFront = UnityEngine.Random.Range(0, customerFrontPrefabs.Count);
                        GameObject customerInstance = Instantiate(customerFrontPrefabs[randomFront], openFrontChairs[secondIndex].position, Quaternion.identity);
                        instantiatedPlayers.Add(customerInstance);
                        customerInstance.name = "Customer" + secondIndex;
                        secondIndex++;
                }
        }

        public bool PlayerHere()
        {
                RaycastHit2D hit = Physics2D.Raycast(coll.bounds.center, Vector2.left, playerDistance, playerMask);
                if (hit)
                {
                        Debug.DrawRay(coll.bounds.center, Vector2.left * playerDistance, Color.green);
                        return true;
                }
                else if (!hit)
                {
                        Debug.DrawRay(coll.bounds.center, Vector2.left * playerDistance, Color.red);
                        return false;
                }
                else
                {
                        return false;
                }
        }

        private void RewardUpdate()
        {
                if (CurrentReward == 15)
                {
                        CurrentReward = 10;
                        angryUpdate = true;
                }
                else if (CurrentReward == 10)
                {
                        CurrentReward = 5;
                        angryUpdate = true;
                }
        }
}
