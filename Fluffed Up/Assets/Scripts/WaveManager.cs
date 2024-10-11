using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public PlayerController player;
    public GameObject fakeBoxEnemyPrefab;
    public UnityEvent waveEvent;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private int currentWave = 0;
    private bool isSpawningWave = false; // Flag to prevent multiple waves from starting
    private float waveSpawnDelay = 2f;

    // Start is called before the first frame update
    void Start()
    {
        waveEvent.AddListener(RequestNextWave);
        StartWave();
    }

    void StartWave()
    {
        for (int i = 0; i < 3; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Debug.Log("Spawning enemy");
        Vector3 randomPosition = GetRandomPosition();
        GameObject newEnemy = Instantiate(fakeBoxEnemyPrefab, randomPosition, Quaternion.identity);
        EnemyBase enemyScript = newEnemy.GetComponent<EnemyBase>();

        // Add event listener: player attack ---> enemy takes damage
        UnityEngine.Events.UnityAction<float> onPlayerAttackAction = (float damage) => HandlePlayerAttack(enemyScript, damage);
        player.AttackEvent.AddListener(onPlayerAttackAction);

        enemyScript.AttackEvent.AddListener(player.TakeDamage);
        enemyScript.OnEnemyDeath += () => RemoveEnemyListener(onPlayerAttackAction, enemyScript);
        enemyScript.player = player;

        currentEnemies.Add(newEnemy);
    }

    Vector3 GetRandomPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * 6f;
        return new Vector3(randomCircle.x, 1f, randomCircle.y);
    }

    void HandlePlayerAttack(EnemyBase enemy, float damage)
    {
        if (enemy != null)
        {
            Debug.Log("HandlePlayerAttack called");
            float distance = Vector3.Distance(player.transform.position, enemy.transform.position);

            if (distance <= player.attackDistanceThreshold)
            {
                Debug.Log("Enemy got damage");
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

    IEnumerator StartNextWave()
    {
        isSpawningWave = true; // Set the flag to true to prevent multiple triggers
        yield return new WaitForSeconds(waveSpawnDelay); // Delay before starting next wave
        StartWave();
        isSpawningWave = false; // Reset the flag after spawning the wave
    }

    void RemoveEnemyListener(UnityEngine.Events.UnityAction<float> action, EnemyBase enemy)
    {
        player.AttackEvent.RemoveListener(action);
        currentEnemies.Remove(enemy.gameObject); // Safely remove the enemy from the list
    }
}
