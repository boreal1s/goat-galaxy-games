using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public PlayerController player;
    public GameObject fakeBoxEnemyPrefab;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private int currentWave = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartWave();
    }

    void StartWave()
    {
        for (int i = 0 ; i < 3 ; i++)
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
        
        // Add event listener: player attack ---event---> enemy takes damage
        UnityEngine.Events.UnityAction<float> onPlayerAttackAction = (float damage) => HandlePlayerAttack(enemyScript, damage);
        player.AttackEvent.AddListener(onPlayerAttackAction);

        enemyScript.AttackEvent.AddListener(player.TakeDamage);
        enemyScript.OnEnemyDeath += () => RemoveEnemyListener(onPlayerAttackAction);
        
        currentEnemies.Add(newEnemy);
    }

    Vector3 GetRandomPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * 6f;
        return new Vector3(randomCircle.x, 1f, randomCircle.y);
    }

    void HandlePlayerAttack(EnemyBase enemy, float damage)
    {
        Debug.Log("HandlePlayerAttack called");
        float distance = Vector3.Distance(player.transform.position, enemy.transform.position);

        if (distance <= player.enemyAttackDistanceThreshold)
        {
            Debug.Log("Enemy got damage");
            enemy.TakeDamage(damage);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentEnemies.RemoveAll(enemy => enemy == null);

        if (currentEnemies.Count == 0)
        {
            StartWave(); // We might want to add time delay here
        }
    }

    void RemoveEnemyListener(UnityEngine.Events.UnityAction<float> action)
    {
        player.AttackEvent.RemoveListener(action);
    }
}
