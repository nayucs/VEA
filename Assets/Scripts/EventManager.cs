using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    private static EventManager instance;

    [Header("再生完了後に遷移するシーン名")]
    public string nextSceneName = "PostExperimentScene";

    private PositionUpdater[] positionUpdatersWithRotation;
    private PositionUpdaterWORotation[] positionUpdatersWithoutRotation;
    private PlayerPositionUpdater[] playerPositionUpdaters;

    private AudioSource audioSource;
    public AudioClip sound1;

    private HeadRotationRecorder recorder;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(DelayedInit());
    }

    IEnumerator DelayedInit()
    {
        yield return null;

        positionUpdatersWithRotation = FindObjectsOfType<PositionUpdater>();
        positionUpdatersWithoutRotation = FindObjectsOfType<PositionUpdaterWORotation>();
        playerPositionUpdaters = FindObjectsOfType<PlayerPositionUpdater>();
        recorder = FindObjectOfType<HeadRotationRecorder>();

        if (recorder == null)
            Debug.LogWarning("HeadRotationRecorder が見つかりません。");

        if (positionUpdatersWithRotation.Length > 0)
            positionUpdatersWithRotation[0].OnPlaybackFinished += OnAllFinished;
        else if (positionUpdatersWithoutRotation.Length > 0)
            positionUpdatersWithoutRotation[0].OnPlaybackFinished += OnAllFinished;
        else if (playerPositionUpdaters.Length > 0)
            playerPositionUpdaters[0].OnPlaybackFinished += OnAllFinished;

        StartCoroutine(DelayedPlayback());
    }

    IEnumerator DelayedPlayback()
    {
        yield return new WaitForSeconds(7f);

        if (audioSource != null)
            audioSource.PlayOneShot(sound1);

        yield return new WaitForSeconds(3f);

        Debug.Log("再生開始");
        recorder?.StartRecording();

        foreach (var updater in positionUpdatersWithRotation) updater.StartPlayback();
        foreach (var updater in positionUpdatersWithoutRotation) updater.StartPlayback();
        foreach (var updater in playerPositionUpdaters) updater.StartPlayback();
    }

    void OnAllFinished()
    {
        Debug.Log("再生が完了しました。記録を停止 → 保存 → シーン遷移します。");

        if (recorder != null)
        {
            recorder.StopRecording();
            recorder.SaveRecording(); // 保存をここで実行
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName); // 保存後にシーン遷移
        }
        else
        {
            Debug.LogError("遷移先シーン名が未設定です。");
        }
    }
}
