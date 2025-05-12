using System.Collections;
using UnityEngine;

/// <summary>
/// Represents a scary sound prefab and its valid spawn range.
/// Each prefab should have its own AudioSource component.
/// </summary>
[System.Serializable]
public class ScarySound
{
    [Tooltip("Prefab GameObject with its own AudioSource on it")]
    public GameObject prefab;

    [Tooltip("Min spawn distance for this sound")]
    public float minDistance = 2f;

    [Tooltip("Max spawn distance for this sound")]
    public float maxDistance = 15f;
}

/// <summary>
/// Randomly spawns and plays spooky sound prefabs around the player at timed intervals.
/// Each sound is instantiated at a random distance and direction within its specified range.
/// </summary>
public class RandomScarySoundManager : MonoBehaviour
{
    [Header("Configure each spooky sound prefab + its range")]
    [Tooltip("Array of different spooky sound prefabs with their spawn ranges")]
    public ScarySound[] sounds;

    [Header("Global Timing (seconds)")]
    [Tooltip("Minimum time between sound events")]
    public float minInterval = 20f;

    [Tooltip("Maximum time between sound events")]
    public float maxInterval = 30f;

    /// <summary>
    /// Starts the loop to spawn random scary sounds if any are configured.
    /// </summary>
    void Start()
    {
        if (sounds == null || sounds.Length == 0)
            Debug.LogWarning("No spooky sounds configured!", this);
        else
            StartCoroutine(PlayRandomScares());
    }

    /// <summary>
    /// Coroutine that randomly plays scary sounds around the GameObject.
    /// </summary>
    private IEnumerator PlayRandomScares()
    {
        while (true)
        {
            // 1) Wait for a random interval between sounds
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            // 2) Pick a random sound entry from the configured array
            var entry = sounds[Random.Range(0, sounds.Length)];

            if (entry.prefab == null)
            {
                Debug.LogWarning("Missing prefab in RandomScarySoundManager!", this);
                continue;
            }

            // 3) Pick a random horizontal direction
            Vector3 dir = Random.insideUnitSphere;
            dir.y = 0;
            dir.Normalize();

            // 4) Determine random distance within min/max range
            float dist = Random.Range(entry.minDistance, entry.maxDistance);
            Vector3 spawnPos = transform.position + dir * dist;

            // 5) Instantiate the sound prefab at the chosen position
            GameObject go = Instantiate(entry.prefab, spawnPos, Quaternion.identity);

            // 6) Play its audio
            var src = go.GetComponent<AudioSource>();
            if (src == null)
            {
                Debug.LogError("ScarySound prefab has no AudioSource!", go);
                Destroy(go);
                continue;
            }
            src.Play();

            // 7) Destroy the prefab after the clip has finished playing
            Destroy(go, src.clip.length + 0.1f);
        }
    }
}
