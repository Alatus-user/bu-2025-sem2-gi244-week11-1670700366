using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject powerUpPrefab;

    [Header("Spawn Points (ใส่ทุกจุดที่มีใน Scene)")]
    [SerializeField] private Transform[] allSpawnPoints;

    [Header("PowerUp Spawn Area (ใช้ Collider bounds หรือกำหนด range)")]
    [SerializeField] private Vector3 powerUpSpawnCenter = Vector3.zero;
    [SerializeField] private Vector3 powerUpSpawnRange = new Vector3(10f, 0f, 10f);

    [Header("Wave Configs")]
    [SerializeField] private WaveConfig[] waveDataList; // ลาก ScriptableObject ทั้ง 4 wave มาใส่

    // ─── State ───────────────────────────────────────────
    private int currentWave = 0;
    private bool isSpawning = false;

    // ─── Tracked enemies per wave (สำหรับเช็ค win condition) ───
    private List<GameObject> activeEnemies = new List<GameObject>();

    // ═════════════════════════════════════════════════════
    void Start()
    {
        StartCoroutine(RunAllWaves());
    }

    // ═══════════════ MAIN LOOP ════════════════════════════
    private IEnumerator RunAllWaves()
    {
        for (int i = 0; i < waveDataList.Length; i++)
        {
            currentWave = i + 1;
            Debug.Log($"─── Wave {currentWave} เริ่มต้น ───");

            yield return StartCoroutine(SpawnWave(waveDataList[i]));

            // รอให้ enemy ทั้งหมดใน wave นี้หายไปก่อน (ถูก destroy โดย timer)
            yield return StartCoroutine(WaitForWaveClear());

            Debug.Log($"─── Wave {currentWave} เสร็จสิ้น ───");
        }

        Debug.Log("✅ ชนะ! ทุก Wave เสร็จสิ้นแล้ว");
    }

    // ═══════════════ SPAWN WAVE ═══════════════════════════
    private IEnumerator SpawnWave(WaveConfig data)
    {
        isSpawning = true;
        activeEnemies.Clear();

        // 1) สุ่ม spawn points สำหรับ wave นี้
        List<Transform> selectedPoints = GetRandomSpawnPoints(data.numberOfRandomSpawnPoint);
        Debug.Log($"Wave {currentWave}: สุ่มได้ {selectedPoints.Count} spawn point");

        // 2) Spawn PowerUp ทันทีตอนเริ่ม wave
        for (int i = 0; i < data.numberOfPowerUp; i++)
        {
            SpawnPowerUp();
        }

        // 3) รอ delayStart ก่อน spawn ศัตรูตัวแรก
        Debug.Log($"Wave {currentWave}: รอ {data.delayStart} วินาทีก่อน spawn ศัตรู");
        yield return new WaitForSeconds(data.delayStart);

        // 4) Spawn ศัตรูทั้งหมด โดยใช้ selected points เท่านั้น
        for (int i = 0; i < data.totalSpawnEnemies; i++)
        {
            // วนเวียนระหว่าง selected spawn points
            Transform spawnPoint = selectedPoints[i % selectedPoints.Count];

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            activeEnemies.Add(enemy);

            Debug.Log($"Wave {currentWave}: Spawn ศัตรู {i + 1}/{data.totalSpawnEnemies} ที่ {spawnPoint.name}");

            // รอ interval ยกเว้นตัวสุดท้าย
            if (i < data.totalSpawnEnemies - 1)
            {
                yield return new WaitForSeconds(data.spawnInterval);
            }
        }

        isSpawning = false;
    }

    // ═══════════════ WAIT FOR CLEAR ═══════════════════════
    private IEnumerator WaitForWaveClear()
    {
        Debug.Log($"Wave {currentWave}: รอให้ศัตรูทั้งหมดถูกทำลาย...");

        // รอจนกว่า enemy ทุกตัวใน list จะถูก destroy (null = ถูก destroy แล้ว)
        bool allCleared = false;
        while (!allCleared)
        {
            allCleared = true;
            foreach (GameObject enemy in activeEnemies)
            {
                if (enemy != null) // ยังไม่ถูก destroy
                {
                    allCleared = false;
                    break;
                }
            }
            yield return new WaitForSeconds(0.5f); // เช็คทุก 0.5 วินาที
        }
    }

    // ═══════════════ HELPERS ══════════════════════════════

    // สุ่ม spawn points โดยไม่ซ้ำกัน
    private List<Transform> GetRandomSpawnPoints(int count)
    {
        List<Transform> pool = new List<Transform>(allSpawnPoints);
        List<Transform> selected = new List<Transform>();

        // clamp ไม่ให้เกินจำนวนที่มี
        int selectCount = Mathf.Min(count, pool.Count);

        for (int i = 0; i < selectCount; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            selected.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex); // ไม่ให้สุ่มซ้ำ
        }

        return selected;
    }

    // Spawn PowerUp ที่ตำแหน่งสุ่มใน range ที่กำหนด
    private void SpawnPowerUp()
    {
        Vector3 randomPos = powerUpSpawnCenter + new Vector3(
            Random.Range(-powerUpSpawnRange.x, powerUpSpawnRange.x),
            powerUpSpawnRange.y,
            Random.Range(-powerUpSpawnRange.z, powerUpSpawnRange.z)
        );
        Instantiate(powerUpPrefab, randomPos, Quaternion.identity);
        Debug.Log($"Wave {currentWave}: Spawn PowerUp ที่ {randomPos}");
    }
}