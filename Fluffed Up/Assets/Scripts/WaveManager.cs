using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public struct EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        public int quantity;

        public EnemySpawnInfo(GameObject go, int quantity)
        {
            this.enemyPrefab = go;
            this.quantity = quantity;
        }
    }

    public PlayerController player;

    // List of enemies
    public GameObject enemyPrefabSlime;
    public GameObject enemyPrefabTurtle;

    public UnityEvent waveEvent;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private int currentWave = 0;
    private bool isSpawningWave = false; // Flag to prevent multiple waves from starting

    // Sound Events
    public UnityEvent<Vector3, AudioClip> playerSoundEvent;

    [SerializeField]
    private GameObject shopComponent;
    public bool shopIsOpen;

    [SerializeField] 
    private TextMeshProUGUI waveCounterText; // For Unity UI Text
    public TextMeshProUGUI countdownText;

    private Coroutine restTimerCoroutine;

    private List<List<EnemySpawnInfo>> waveList;

    private ShopTrigger shopTrigger;

    // Start is called before the first frame update
    void Start()
    {
        shopTrigger = FindObjectOfType<ShopTrigger>(); // Find the ShopManager in the scene
        if (shopTrigger == null)
        {
            Debug.LogError("No shopTrigger found in the scene.");
        }
        waveEvent.AddListener(RequestNextWave);
        PopulateWave();
        StartWave();

        if (shopComponent == null)
            Debug.Log("No shop component was assigned to the WaveManager");
    }

    void PopulateWave()
    {
        waveList = new List<List<EnemySpawnInfo>>
        {            
            new(){// First wave
                new(enemyPrefabSlime, 3)
            },
            new(){// Second wave
                new(enemyPrefabTurtle, 2)
            },
            new(){// Third wave
                new(enemyPrefabSlime, 2),
                new(enemyPrefabTurtle, 2)
            },
            new(){// Fourth wave
                new(enemyPrefabTurtle, 4)
            },
            new(){// Fifth wave
                new(enemyPrefabSlime, 6)
            },
            new(){// Sixth wave
                new(enemyPrefabSlime, 3),
                new(enemyPrefabTurtle, 3)
            },
            new(){// Sixth wave
                new(enemyPrefabSlime, 4),
                new(enemyPrefabTurtle, 4)
            },
            new(){// Seventh wave
                new(enemyPrefabSlime, 10)
            },
            new(){// Eigth wave
                new(enemyPrefabTurtle, 9)
            },
            new(){// Ninth wave
                new(enemyPrefabSlime, 7),
                new(enemyPrefabTurtle, 5)
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

        shopTrigger.canTriggerShop = false;
        shopTrigger.CloseShop();
        Debug.Log("Times up. Disabling Shop");

        // Clear the countdown text when finished
        countdownText.text = "";
        currentWave++; // update wave count
        UpdateWaveCounter();
        SpawnEnemy();
    }

    void UpdateWaveCounter()
    {
        waveCounterText.text = "Wave " + currentWave.ToString();
    }

    void SpawnEnemy()
    {
        if (currentWave % 9 == 0)
        {
            currentWave = 0;
        }
        EnemyLoader(waveList[currentWave - 1]);
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

                // Add event listener: player attack ---> enemy takes damage
                void onPlayerAttackAction(float damage, int delayInMilli) => StartPlayerAttack(enemyScript, damage, delayInMilli);
                player.AttackEvent.AddListener(onPlayerAttackAction);

                enemyScript.AttackEvent.AddListener(player.TakeDamage);
                enemyScript.OnEnemyDeath += () => RemoveEnemyListener(onPlayerAttackAction, enemyScript);
                enemyScript.player = player;

                currentEnemies.Add(newEnemy);
            }
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * 18f;
        return new Vector3(randomCircle.x, 1f, randomCircle.y);
    }

    void StartPlayerAttack(EnemyBase enemy, float damage, int delayInMilli)
    {
        playerSoundEvent.Invoke(player.transform.position, player.attackSound);
        StartCoroutine(DelayedPlayerAttack(enemy, damage, delayInMilli));
    }

    IEnumerator DelayedPlayerAttack(EnemyBase enemy, float damage, int delayInMilli)
    {
        yield return new WaitForSeconds(delayInMilli/1000f);
        HandlePlayerAttack(enemy, damage);
    }

    void HandlePlayerAttack(EnemyBase enemy, float damage)
    {
        if (enemy != null)
        {
            // Debug.Log("HandlePlayerAttack called");
            float distance = Vector3.Distance(player.transform.position, enemy.transform.position);

            // Hit condition1: Distance smaller than threshold
            bool withinDistance = distance <= player.attackDistanceThreshold;
            bool withinAngle = math.abs(Vector3.Angle(player.transform.forward, enemy.transform.position - player.transform.position)) < 90 ;
            if (withinDistance && withinAngle)
            {
                // Debug.Log("Enemy got damage");
                enemy.TakeDamage(damage);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentEnemies.RemoveAll(enemy => enemy == null);

        waveEvent.Invoke();

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
        shopTrigger.canTriggerShop = true;

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
