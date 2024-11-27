using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Cinemachine;

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
    public GameObject[] playerPrefabs;

    // List of enemies
    [SerializeField] public GameObject enemyPrefabSlime;
    [SerializeField] public GameObject enemyPrefabTurtle;
    [SerializeField] public GameObject enemyPrefabMiniBossDog;
    [SerializeField] public GameObject enemyPrefabMiniBossPenguin;
    [SerializeField] public GameObject enemyPrefabBossCyclopes;
    [SerializeField] public GameObject enemySpawnArea;
    public BoxCollider[] enemySpawnBoxes;

    // Shop light source
    [SerializeField] public LightPulse shopIndicatorLight;
    [SerializeField] public LightPulse shopBeaconLight;

    public UnityEvent waveEvent;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private int currentWave = 0;
    private bool isSpawningWave = false; // Flag to prevent multiple waves from starting

    [SerializeField]
    private ShopController shopController;

    [SerializeField] 
    public TextMeshProUGUI waveCounterText; // For Unity UI Text
    public TextMeshProUGUI countdownText;

    private Coroutine restTimerCoroutine;

    private int currentDifficulty = 3;
    public bool enemiesAreSpawning = false;
    public bool isRunningWave;
    private Queue<string> enemyQueue = new Queue<string>();
    private Queue<string> nextEnemyQueue = new Queue<string>();
    private List<List<string>> possibleStagedOnslaughts;
    private Dictionary<string, EnemySpawnInfo> enemyScaleInfo;
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
        enemyScaleInfo = new Dictionary<string, EnemySpawnInfo>()
        {
            {"Slime", new(enemyPrefabSlime, 1, 6, 10, 70) },
            {"Turtle", new(enemyPrefabTurtle, 3, 4, 20, 180)},
            {"BossDog", new(enemyPrefabMiniBossDog, 10, 5, 40, 400)},
            {"BossPenguin", new(enemyPrefabMiniBossPenguin, 20, 4, 60, 600)},
            {"BossCyclopes", new(enemyPrefabBossCyclopes, 40, 10, 80, 1000)}
        };

        SpawnPlayer();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Prepare Enemies
        enemySpawnBoxes = enemySpawnArea.GetComponents<BoxCollider>();
        StartCoroutine(ComputeOnslaught());
        Debug.Log("Initial Spawn Generated: " + nextEnemyQueue.Count + " enemies");
        StartWave();
        waveEvent.AddListener(RequestNextWave);

        if (shopController == null) {
            Debug.LogWarning("No ShopController was assigned to WaveManager");
        }
        else {
            shopController.CloseShop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //currentEnemies.RemoveAll(enemy => enemy == null);
        ResponsiveShopLight(shopController.canTriggerShop);
        waveEvent.Invoke();
    }

    void SpawnPlayer()
    {
        GameObject playerGameObject = Instantiate(playerPrefabs[SelectChar.characterID], new Vector3(0, 0.052f, 0), Quaternion.identity);
        player = playerGameObject.GetComponent<PlayerController>();

        waveCounterText = playerGameObject.transform.Find("PlayerUI/WaveCounter").GetComponent<TextMeshProUGUI>();
        countdownText = playerGameObject.transform.Find("PlayerUI/TimerCountdown").GetComponent<TextMeshProUGUI>();
    }

    private void ScaleEnemyDifficulty()
    {
        if (currentWave > 6)
        {
            foreach (var enemyKey in enemyScaleInfo.Keys)
            {
                EnemySpawnInfo spawnInfo = enemyScaleInfo[enemyKey];
                EnemyBase enemyBase = spawnInfo.enemyPrefab.GetComponent<EnemyBase>();
                spawnInfo.health = enemyBase.baseHealth + (enemyBase.baseHealth * currentWave * scalingConfig["Health"]);
                spawnInfo.attackPower = enemyBase.baseAttackPower + (enemyBase.baseAttackPower * currentWave * scalingConfig["AttackPower"]);
                enemyBase.goldValueMin = (int)Mathf.Ceil(enemyBase.goldValueMinBase + (enemyBase.goldValueMinBase * currentWave * scalingConfig["GoldMin"]));
                enemyBase.goldValueMax = (int)Mathf.Ceil(enemyBase.goldValueMaxBase + (enemyBase.goldValueMaxBase * currentWave * scalingConfig["GoldMax"]));
            }
        }

        currentDifficulty = (int)Mathf.Ceil(currentDifficulty + (currentDifficulty * scalingConfig["Difficulty"]));
        Debug.Log("New difficulty: " + currentDifficulty);
    }

    private IEnumerator ComputeOnslaught()
    {
        Debug.Log("Computing Onslaught");

        if (currentWave + 1 == 3)
        {
            nextEnemyQueue.Clear();
            nextEnemyQueue = new Queue<string>(new List<string>() { "BossDog", "Slime", "Slime" });
            Debug.Log("Onslaught Computed");
            yield break;
        }

        if (currentWave + 1 == 6)
        {
            nextEnemyQueue.Clear();
            nextEnemyQueue = new Queue<string>( new List<string>(){"BossPenguin", "Turtle", "Turtle"});
            Debug.Log("Onslaught Computed");
            yield break;
        }

        if (currentWave + 1 == 10)
        {
            nextEnemyQueue.Clear();
            nextEnemyQueue = new Queue<string>( new List<string>(){"BossCyclopes", "BossDog", "BossPenguin"});
            Debug.Log("Onslaught Computed");
            yield break;
        }

        List<KeyValuePair<int, string>> enemyDifficultyPairs = new List<KeyValuePair<int, string>>();
        foreach (string enemy in enemyScaleInfo.Keys)
        {
            enemyDifficultyPairs.Add(new KeyValuePair<int, string>(enemyScaleInfo[enemy].difficultyCoefficient, enemy));
        }

        possibleStagedOnslaughts = OnslaughtMinion(currentDifficulty, enemyDifficultyPairs);

        Debug.Log(possibleStagedOnslaughts);
        nextEnemyQueue = new Queue<string>(possibleStagedOnslaughts[UnityEngine.Random.Range(0, possibleStagedOnslaughts.Count)]);
        Debug.Log(nextEnemyQueue.Count);

        Debug.Log("Onslaught Computed for difficulty: " + currentDifficulty);
        yield return null;
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
        List<List<List<string>>> dp = new List<List<List<string>>>(new List<List<string>>[difficulty + 1]);
        for (int a = 1; a < difficulty + 1; a++)
        {
            dp[a] = new List<List<string>>();
        }
        dp[0] = new List<List<string>>() { new List<string>() { } };

        List<List<List<string>>> nextDP = DeepCopyTriplyNestedList(dp);

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            Debug.Log("Computing dp for enemy: " + enemies[i].Value);
            for (int d = enemies[i].Key; d < difficulty + 1; d++)
            {
                nextDP[d] = DeepCopyDoublyNestedList(dp[d]);
                if (d - enemies[i].Key >= 0)
                {
                    foreach (List<string> possibleEnemyList in nextDP[d - enemies[i].Key])
                    {
                        List<string> newList = new List<string>(possibleEnemyList);
                        newList.Add(enemies[i].Value);
                        nextDP[d].Add(newList);
                    }
                }
            }
            dp = DeepCopyTriplyNestedList(nextDP);
        }

        for (int j = dp.Count - 1; j >= 0; j--)
        {
            if (dp[j].Count > 0)
            {
                return dp[j];
            }
        }

        return new List<List<string>>();
    }

    public void StartWave()
    {
        isRunningWave = true;
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

        // Increment wave
        currentWave += 1;
        waveCounterText.text = "Wave " + currentWave.ToString(); // Update wave counter

        // Start preparing next wave ahead of time
        enemyQueue = new Queue<string>(nextEnemyQueue);
        ScaleEnemyDifficulty();
        StartCoroutine(ComputeOnslaught());

        Debug.Log("Loading enemies");
        StartCoroutine(EnemyGroupHandler(enemyQueue));
        isRunningWave = false;
    }

    private IEnumerator EnemyGroupHandler(Queue<string> enemyQueue)
    {
        enemiesAreSpawning = true;
        float secondsToWait = 4f;
        while (enemyQueue.Count > 0)
        {
            List<string> enemyIds = new List<string>();
            int enemiesToSpawn = 4;
            while (enemyQueue.Count > 0 && enemiesToSpawn > 0)
            {
                enemiesToSpawn -= 1;
                enemyIds.Add(enemyQueue.Dequeue());
            }

            EnemyLoader(enemyIds);
            yield return new WaitForSeconds(secondsToWait);
        }
        enemiesAreSpawning = false;
    }

    private void EnemyLoader(List<string> group)
    {
        Debug.Log("Spawning group of enemies");
        foreach (string enemyId in group)
        {
            Vector3 randomPosition = GetRandomPosition();
            GameObject newEnemy = Instantiate(enemyScaleInfo[enemyId].enemyPrefab, randomPosition, Quaternion.identity);
            EnemyBase enemyScript = newEnemy.GetComponent<EnemyBase>();
            enemyScript.attackPower = enemyScaleInfo[enemyId].attackPower;
            enemyScript.moveSpeed = enemyScaleInfo[enemyId].speed;
            enemyScript.health = enemyScaleInfo[enemyId].health;

            // Add event listener: player attack ---> enemy takes damage
            void onPlayerAttackAction(float damage, int delayInMilli) => InitiateAttackTimer(enemyScript, damage, delayInMilli, true);
            player.AttackEvent.AddListener(onPlayerAttackAction);

            // Add event listener: enemy attacks ---> player takes damage
            void onEnemyAttackAction(float damage, int delayInMilli) => InitiateAttackTimer(enemyScript, damage, delayInMilli, false);
            enemyScript.AttackEvent.AddListener(onEnemyAttackAction);
            enemyScript.OnEnemyDeath += () => RemoveEnemyListener(onPlayerAttackAction, enemyScript);
            enemyScript.player = player;

            // Add type 2 attack event listener for boss: enemy attacks ---> player takes damage
            if (enemyId == "BossCyclopes")
            {
                EnemyBossCyclopes bossScript = newEnemy.GetComponent<EnemyBossCyclopes>();
                void onEnemyAttackAction2(float damage, int delayInMilli) => InitiateAttackTimer(enemyScript, damage, delayInMilli, false);
                bossScript.AttackEventType2.AddListener(onEnemyAttackAction2);
            }

            currentEnemies.Add(newEnemy);
        }
    }

    Vector3 GetRandomPosition()
    {
        BoxCollider selectedBox = enemySpawnBoxes[0];
        float shortestDistance = math.INFINITY;
        foreach (BoxCollider boxCollider in enemySpawnBoxes)
        {
            Vector3 boxColliderWorldCenter = boxCollider.transform.position + boxCollider.transform.TransformVector(boxCollider.center);
            float distanceToPlayer = (boxColliderWorldCenter - player.transform.position).magnitude;
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                selectedBox = boxCollider;
            }
        }

        Vector3 spawnPosition = new Vector3(
            UnityEngine.Random.Range(selectedBox.bounds.min.x, selectedBox.bounds.max.x),
            2f,
            UnityEngine.Random.Range(selectedBox.bounds.min.z, selectedBox.bounds.max.z)
        );

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
        if (enemy != null && player != null)
        {
            float distance = Vector3.Distance(enemy.transform.position, player.transform.position);

            // Hit condition1: Distance smaller than threshold
            if (playerAttacks) // This is for slayer type player only 
            {
                bool withinDistance = distance <= player.attackDistanceThreshold;
                bool withinAngle = math.abs(Vector3.Angle(player.transform.forward, enemy.transform.position - player.transform.position)) < 30 ;
                if (withinDistance && withinAngle)
                {
                    player.PlaySlayHitSound();
                    enemy.TakeDamage(damage);
                }
            }
            else if (enemy.isAttackInvalid() == false)
            {
                bool withinDistance = distance <= enemy.attackDistanceThreshold;
                bool withinAngle = math.abs(Vector3.Angle(enemy.transform.forward, player.transform.position - enemy.transform.position)) < 30 ;
                if (withinDistance && withinAngle)
                {
                    player.TakeDamage(damage);
                }
            }
        }
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
        if (currentEnemies.Count == 0 && !isSpawningWave && enemyQueue.Count == 0 && !isRunningWave && !enemiesAreSpawning)
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
        StartWave();
        isSpawningWave = false; // Reset the flag after spawning the wave
    }

    void RemoveEnemyListener(UnityEngine.Events.UnityAction<float, int> action, EnemyBase enemy)
    {
        player.AttackEvent.RemoveListener(action);
        currentEnemies.Remove(enemy.gameObject); // Safely remove the enemy from the list
    }

    public static List<List<List<string>>> DeepCopyTriplyNestedList(List<List<List<string>>> originalList)
    {
        if (originalList == null)
            return null;

        List<List<List<string>>> copy = new List<List<List<string>>>();
        foreach (var innerList1 in originalList)
        {
            List<List<string>> innerCopy1 = new List<List<string>>();
            foreach (var innerList2 in innerList1)
            {
                List<string> innerCopy2 = new List<string>(innerList2);
                innerCopy1.Add(innerCopy2);
            }
            copy.Add(innerCopy1);
        }

        return copy;
    }

    public static List<List<string>> DeepCopyDoublyNestedList(List<List<string>> originalList)
    {
        if (originalList == null)
            return null;

        List<List<string>> copy = new List<List<string>>();
        foreach (var innerList in originalList)
        {
            List<string> innerCopy = new List<string>(innerList);
            copy.Add(innerCopy);
        }

        return copy;
    }
}
