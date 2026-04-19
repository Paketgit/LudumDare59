using UnityEngine;
using System.Collections;

public class RandomBird : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] clips; 
    public float minDelay = 15f;
    public float maxDelay = 45f;

    void Start()
    {
        StartCoroutine(PlayRandomly());
    }

    IEnumerator PlayRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            source.clip = clips[Random.Range(0, clips.Length)];
            source.pitch = Random.Range(0.9f, 1.1f);
            source.Play();
        }
    }
}