using System.Collections;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private PositionUpdaterWORotation[] positionUpdaters;
    public AudioClip StartSE;
    AudioSource audioSource;

    void Start()
    {
        positionUpdaters = FindObjectsOfType<PositionUpdaterWORotation>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(DelayedPlayback());
    }

    IEnumerator DelayedPlayback()
    {
        yield return new WaitForSeconds(7f); // 7秒待機

        // 音を鳴らす
        if (audioSource != null)
        {
            audioSource.PlayOneShot(StartSE);
        }

        yield return new WaitForSeconds(3f); // 3秒待機

        // 各PositionUpdaterに再生開始命令
        foreach (var updater in positionUpdaters)
        {
            if (updater != null)
            {
                updater.StartPlayback();
            }
        }
    }
}