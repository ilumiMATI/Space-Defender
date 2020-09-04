using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Waves list")]
    [SerializeField] List<WaveConfig> waveConfigs;

    [Header("Additional settings")]
    [SerializeField] int startingWave = 0;
    [SerializeField] bool looping = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        do
        {
            yield return StartCoroutine(SpawnAllWaves());
        } while (looping); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnAllWaves()
    {
        for (int waveIndex = startingWave; waveIndex < waveConfigs.Count; waveIndex++)
        {
            WaveConfig currentWave = waveConfigs[waveIndex];
            yield return StartCoroutine(SpawnAllEnemiesInWave(currentWave));
        }
    }

    private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig)
    {
        for (int enemyCount = 0; enemyCount < waveConfig.GetNumberOfEnemies(); enemyCount++)
        {
            GameObject newEnemy = Instantiate(
                waveConfig.GetEnemyPrefab(),
                waveConfig.GetWaypoints()[0].position,
                Quaternion.identity );
            newEnemy.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig);
            yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawns());
        }
    }
}
