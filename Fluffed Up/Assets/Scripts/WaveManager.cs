using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Linq.Expressions;
using Cinemachine;
using System.Security.Cryptography.X509Certificates;

public class WaveManager : MonoBehaviour
{
    public struct EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        public int quantity;
        public int speed;
        public int attackPower;

        public EnemySpawnInfo(GameObject go, int quantity, int speed, int attackPower)
        {
            this.enemyPrefab = go;
            this.quantity = quantity;
            this.speed = speed;
            this.attackPower = attackPower;
        }
    }

    public PlayerController player;
    public GameObject playerAimCamera;
    public GameObject playerFollowCamera;
    public GameObject[] playerPrefabs;
    public Transform cameraTransform;

    // List of enemies
    public GameObject enemyPrefabSlime;
    public GameObject enemyPrefabTurtle;
    public GameObject enemyPrefabMiniBossDog;
    public GameObject enemyPrefabMiniBossPenguin;
    [SerializeField] public GameObject enemySpawnArea;
    public BoxCollider[] enemySpawnBoxes;

    // Shop light source
    [SerializeField] public LightPulse shopIndicatorLight;
    [SerializeField] public LightPulse shopBeaconLight;

    public UnityEvent waveEvent;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private int currentWave = -1; // Will be switched to 0 by StartWave
    private bool isSpawningWave = false; // Flag to prevent multiple waves from starting

    [SerializeField]
    private ShopController shopController;

    [SerializeField] 
    public TextMeshProUGUI waveCounterText; // For Unity UI Text
    public TextMeshProUGUI countdownText;

    private Coroutine restTimerCoroutine;

    private List<List<EnemySpawnInfo>> waveList;

    private void Awake()
    {
        SpawnPlayer();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Prepare Enemies
        enemySpawnBoxes = enemySpawnArea.GetComponents<BoxCollider>();
        waveEvent.AddListener(RequestNextWave);
        PopulateWave();
        currentWave = waveList.Count - 1;
        StartWave();

        if (shopController == null) {
            Debug.LogWarning("No ShopController was assigned to WaveManager");
        }
        else {
            shopController.CloseShop();
        }
    }

    void SpawnPlayer()
    {
        GameObject playerGameObject = Instantiate(playerPrefabs[SelectChar.characterID], new Vector3(0, 0.052f, 0), Quaternion.identity);
        player = playerGameObject.GetComponent<PlayerController>();
        player.cameraTransform = cameraTransform;
        Transform playerCameraRoot = playerGameObject.transform.Find("CameraRoot");
        
        CinemachineFreeLook aimCameraFreeLook = playerAimCamera.GetComponent<CinemachineFreeLook>();
        aimCameraFreeLook.Follow = playerCameraRoot;
        aimCameraFreeLook.LookAt = playerCameraRoot;

        CinemachineFreeLook followCameraFreeLook = playerFollowCamera.GetComponent<CinemachineFreeLook>();
        followCameraFreeLook.Follow = playerCameraRoot;
        followCameraFreeLook.LookAt = playerCameraRoot;

        waveCounterText = playerGameObject.transform.Find("PlayerUI/WaveCounter").GetComponent<TextMeshProUGUI>();
        countdownText = playerGameObject.transform.Find("PlayerUI/TimerCountdown").GetComponent<TextMeshProUGUI>();
    }

    void PopulateWave()
    {
        waveList = new List<List<EnemySpawnInfo>>
        {            
            new(){// First wave
                new(enemyPrefabMiniBossDog, 1, 8, 15)//new(enemyPrefabSlime, 3, 7, 5)
            },
            new(){// Second wave
                new(enemyPrefabTurtle, 2, 8, 10)
            },
            new(){// Third wave
                new(enemyPrefabMiniBossDog, 1, 8, 15)
            },
            new(){// Fourth wave
                new(enemyPrefabTurtle, 4, 9, 15)
            },
            new(){// Fifth wave
                new(enemyPrefabSlime, 6, 11, 10)
            },
            new(){// Sixth wave
                new(enemyPrefabSlime, 3, 10, 12),
                new(enemyPrefabTurtle, 3, 12, 17)
            },
            new(){// Seventh wave
                new(enemyPrefabMiniBossPenguin, 1, 15, 30)
            },
            new(){// Eigth wave
                new(enemyPrefabSlime, 10, 13, 15)
            },
            new(){// Ninth wave
                new(enemyPrefabTurtle, 9, 13, 20)
            },
            new(){// Tenth wave
                new(enemyPrefabMiniBossDog, 1, 17, 30),
                new(enemyPrefabMiniBossPenguin, 1, 15, 40)
            },
        };
    }

    public void StartWave()
    {
        // Stop the rest timer if it's running
        if (restTimerCoroutine != null)
        {
            StopCoroutine(restTimerCoroutine);
            restTimerCoroutine = null; // Clear the reference
        }

        shopController.canTriggerShop = false;
        shopController.RestockShop();
        if (shopController.shopIsOpen)
            shopController.CloseShop();
        Debug.Log("Times up. Disabling Shop");

        // Clear the countdown text when finished
        countdownText.text = "";

        // Increment Wave
        currentWave = (currentWave + 1)%waveList.Count;

        waveCounterText.text = "Wave " + (currentWave + 1).ToString();// Update wave counter
        EnemyLoader(waveList[currentWave]);// Spawn enemy
    }

    private void EnemyLoader(List<EnemySpawnInfo> wave)
    {
        foreach (EnemySpawnInfo spawnInfo in wave)
        {
            for (int i = 0 ; i < spawnInfo.quantity ; i++)
            {
                Vector3 randomPosition = GetRandomPosition();
                GameObject newEnemy = Instantiate(spawnInfo.enemyPrefab, randomPosition, Quaternion.identity);
                EnemyBase enemyScript = newEnemy.GetComponent<EnemyBase>();
                enemyScript.attackPower = spawnInfo.attackPower;
                enemyScript.moveSpeed = spawnInfo.speed;

                // Add event listener: player attack ---> enemy takes damage
                void onPlayerAttackAction(float damage, int delayInMilli) => InitiateAttackTimer(enemyScript, damage, delayInMilli, true);
                player.AttackEvent.AddListener(onPlayerAttackAction);

                // Add event listener: enemy attacks ---> player takes damage
                void onEnemyAttackAction(float damage, int delayInMilli) => InitiateAttackTimer(enemyScript, damage, delayInMilli, false);
                enemyScript.AttackEvent.AddListener(onEnemyAttackAction);
                enemyScript.OnEnemyDeath += () => RemoveEnemyListener(onPlayerAttackAction, enemyScript);
                enemyScript.player = player;

                currentEnemies.Add(newEnemy);
            }
        }
    }

    Vector3 GetRandomPosition()
    {
        BoxCollider selectedBox = enemySpawnBoxes[UnityEngine.Random.Range(0, enemySpawnBoxes.Length)];
        Vector3 spawnPosition = new Vector3(
            UnityEngine.Random.Range(selectedBox.bounds.min.x, selectedBox.bounds.max.x),
            2f,
            UnityEngine.Random.Range(selectedBox.bounds.min.z, selectedBox.bounds.max.z)
        );

        // Raycast down to find the terrain level
        // RaycastHit hit;
        // if (Physics.Raycast(spawnPosition, Vector3.down, out hit, Mathf.Infinity))
        // {
        //     // Return the ground level plus a small offset to prevent sinking
        //     return new Vector3(spawnPosition.x, hit.point.y + 0.1f, spawnPosition.y);
        // }

        // Fallback if the raycast fails
        return spawnPosition;
    }

    void InitiateAttackTimer(EnemyBase enemy, float damage, int delayInMilli, bool playerAttacks)
    {
        StartCoroutine(DelayedAttack(enemy, damage, delayInMilli, playerAttacks));
    }

    IEnumerator DelayedAttack(EnemyBase enemy, float damage, int delayInMilli, bool playerAttacks)
    {
        yield return new WaitForSeconds(delayInMilli/1000f);
        ExecuteAttack(enemy, damage, playerAttacks);
    }

    void ExecuteAttack(EnemyBase enemy, float damage, bool playerAttacks)
    {
        if (enemy != null)
        {
            float distance = Vector3.Distance(player.transform.position, enemy.transform.position);

            // Hit condition1: Distance smaller than threshold
            if (playerAttacks)
            {
                bool withinDistance = distance <= player.attackDistanceThreshold;
                bool withinAngle = math.abs(Vector3.Angle(player.transform.forward, enemy.transform.position - player.transform.position)) < 30 ;
                if (withinDistance && withinAngle)
                {
                    enemy.TakeDamage(damage, player.enemyStunDelayMilli);
                }
            }
            else if (enemy.isAttackInvalid() == false)
            {
                bool withinDistance = distance <= enemy.attackDistanceThreshold;
                bool withinAngle = math.abs(Vector3.Angle(enemy.transform.forward, player.transform.position - enemy.transform.position)) < 30 ;
                if (withinDistance && withinAngle)
                {
                    player.TakeDamage(damage, 0);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentEnemies.RemoveAll(enemy => enemy == null);
        ResponsiveShopLight(shopController.canTriggerShop);        
        waveEvent.Invoke();
    }

    private void ResponsiveShopLight(bool activate)
    {
        if (activate)
        {
            float distanceThreshold = 12.0f;
            float distanceFromShopToPlayer = Vector3.Distance(player.transform.position, shopIndicatorLight.transform.position);
            if (distanceFromShopToPlayer <= distanceThreshold)
            {
                shopIndicatorLight.lightActivate = false;
                shopBeaconLight.lightActivate = true;
            }
            else
            {
                shopIndicatorLight.lightActivate = true;
                shopBeaconLight.lightActivate = false;
            }
        }
        else
        {
            shopIndicatorLight.lightActivate = false;
            shopBeaconLight.lightActivate = false;
        }
    }

    private void RequestNextWave()
    {
        if (currentEnemies.Count == 0 && !isSpawningWave)
        {
            StartCoroutine(StartNextWave());
        }
    }

    private IEnumerator StartRestTimer()
    {
        if (currentWave >= 0)
            shopController.canTriggerShop = true;

        float countdownDuration = 10f; // Total countdown time
        float elapsed = 0f;

        while (elapsed < countdownDuration)
        {
            elapsed += Time.deltaTime; // Increment elapsed time
            float remainingTime = countdownDuration - elapsed; // Calculate remaining time

            // Update the UI text
            countdownText.text = "Monsters spawning in " + Mathf.Ceil(remainingTime).ToString() + " seconds"; // Show seconds remaining

            yield return null; // Wait for the next frame
        }

        // Clear the countdown text when finished
        countdownText.text = "";
    }

    IEnumerator StartNextWave()
    {
        isSpawningWave = true; // Set the flag to true to prevent multiple triggers
        
        // Start the rest timer and wait for it to finish
        if (restTimerCoroutine != null)
        {
            StopCoroutine(restTimerCoroutine);
        }
        restTimerCoroutine = StartCoroutine(StartRestTimer());
        
        yield return restTimerCoroutine; // Wait for the timer to finish
        isSpawningWave = false; // Reset the flag after spawning the wave
        StartWave();
    }

    void RemoveEnemyListener(UnityEngine.Events.UnityAction<float, int> action, EnemyBase enemy)
    {
        player.AttackEvent.RemoveListener(action);
        currentEnemies.Remove(enemy.gameObject); // Safely remove the enemy from the list
    }
}
