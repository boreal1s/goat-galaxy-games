using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Linq.Expressions;
using Cinemachine;
using System.Security.Cryptography.X509Certificates;
using KaimiraGames;
using System.Drawing.Text;
using TMPro.EditorUtilities;

public class WaveManager : MonoBehaviour
{
    public struct EnemySpawnInfo
    {
        public int speed;
        public float attackPower;
        public float health;
        public GameObject enemyPrefab;
        public int difficultyCoefficient;

        public EnemySpawnInfo(GameObject enemyPrefab, int dc, int speed, int attackPower, float health)
        {
            this.speed = speed;
            this.attackPower = attackPower;
            this.health = health;
            this.enemyPrefab = enemyPrefab;
            this.difficultyCoefficient = dc;
        }
    }

    public PlayerController player;
    public GameObject playerAimCamera;
    public GameObject playerFollowCamera;
    public GameObject[] playerPrefabs;
    public Transform cameraTransform;

    // List of enemies
    public static GameObject enemyPrefabSlime;
    public static GameObject enemyPrefabTurtle;
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

    private int currentDifficulty = 3;
    private Queue<List<string>> enemyQueue;
    private Queue<List<string>> nextEnemyQueue;
    private Dictionary<string, EnemySpawnInfo> enemyScaleInfo = new Dictionary<string, EnemySpawnInfo>()
    {
        {"Slime", new(enemyPrefabSlime, 1, 7, 10, 70) },
        {"Turtle", new(enemyPrefabTurtle, 3, 4, 20, 180)},
    };
    private Dictionary<string, float> scalingConfig = new Dictionary<string, float>()
    {
        { "Health", 0.04f},
        { "AttackPower", 0.02f},
        { "GoldMin", 0.05f},
        { "GoldMax", 0.05f},
        { "Difficulty", 0.33f},
    };

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

    private void ScaleEnemyDifficulty()
    {
        foreach(var enemyKey in enemyScaleInfo.Keys)
        {
            EnemySpawnInfo spawnInfo = enemyScaleInfo[enemyKey];
            EnemyBase enemyBase = spawnInfo.enemyPrefab.GetComponent<EnemyBase>();
            spawnInfo.health = enemyBase.baseHealth + (enemyBase.baseHealth * currentWave * scalingConfig["Health"]);
            spawnInfo.attackPower = enemyBase.baseAttackPower + (enemyBase.baseAttackPower * currentWave * scalingConfig["AttackPower"]);
            enemyBase.goldValueMin = (int)(enemyBase.goldValueMinBase + (enemyBase.goldValueMinBase * currentWave * scalingConfig["GoldMin"]));
            enemyBase.goldValueMax = (int)(enemyBase.goldValueMaxBase + (enemyBase.goldValueMaxBase * currentWave * scalingConfig["GoldMax"]));
        }

        currentDifficulty = (int)(currentDifficulty + (currentDifficulty * scalingConfig["Difficulty"]));
    }

    private Queue<List<string>> ComputeOnslaught()
    {
        List<KeyValuePair<int, string>> enemyDifficultyPairs = new List<KeyValuePair<int, string>>();
        foreach (string enemy in enemyScaleInfo.Keys)
        {
            enemyDifficultyPairs.Add(new KeyValuePair<int, string>(enemyScaleInfo[enemy].difficultyCoefficient, enemy));
        }
        List<List<string>> enemiesToSpawn = OnslaughtMinion(currentDifficulty, enemyDifficultyPairs);
        return new Queue<List<string>>((IEnumerable<List<string>>)enemiesToSpawn[UnityEngine.Random.Range(0, enemiesToSpawn.Count)]);
    }

    /* 
       Onslaught Minion aims to maximize the level of difficulty per wave up to a specified cap.
       This function receives a difficulty cap for the wave and a list of possible enemies to spawn.
       It then produces a list of enemy sets that meet or are closest to the difficulty cap.
       The minion produces multiple sets that can then be chosen from randomly, such that waves can
       be dynamic and somewhat randomized.
    
       This is essentially the unbounded knapsack problem (https://www.geeksforgeeks.org/unbounded-knapsack-repetition-items-allowed/)
       but instead of producing the number of possible weight combinations, the minion must produce
       the combinations themselves.

       To save time on writing a solution for the algorithm, I referenced NeetCode's existing solution
       for a similar problem (https://www.youtube.com/watch?v=Mjy4hd2xgrs&ab_channel=NeetCode); however,
       the logic for maintaining a list of possible combinations is homegrown.
    */
    public List<List<string>> OnslaughtMinion(int difficulty, List<KeyValuePair<int, string>> enemies)
    {
        List<List<List<string>>> dp = new List<List<List<string>>>();
        dp[0] = new List<List<string>>();
        List<List<List<string>>> nextDP = new List<List<List<string>>>();

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            nextDP = new List<List<List<string>>>();
            nextDP[0] = new List<List<string>>(); ;

            for (int d = 1; d <= difficulty; d++)
            {
                nextDP[d] = dp[d];
                if (d - enemies[i].Key >= 0)
                {
                    foreach (List<string> possibleEnemyList in nextDP[d - enemies[i].Key])
                    {
                        List<string> newList = possibleEnemyList;
                        newList.Add(enemies[i].Value);
                        nextDP[d].Add(newList);
                    }
                }
            }
            dp = nextDP;
        }

        for (int j = dp.Count - 1; j >= 0; j--)
        {
            if (dp[j].Count > 0)
            {
                return dp[j];
            }
        }

        for (int j = nextDP.Count - 1; j >= 0; j--)
        {
            if (nextDP[j].Count > 0)
            {
                return nextDP[j];
            }
        }

        return new List<List<string>>();
    }


    /// <summary>
    /// 
    /// </summary>

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
        currentWave = (currentWave + 1) % waveList.Count;

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
            bool withinDistance = distance <= player.attackDistanceThreshold;
            bool withinAngle = math.abs(Vector3.Angle(player.transform.forward, enemy.transform.position - player.transform.position)) < 30 ;
            if (withinDistance && withinAngle)
            {
                if (playerAttacks)
                    enemy.TakeDamage(damage, player.enemyStunDelayMilli);
                else if (enemy.isAttackInvalid() == false)
                    player.TakeDamage(damage, 0);
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
