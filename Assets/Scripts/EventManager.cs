using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    private static EventManager instance;

    [Header("再生完了後に遷移するシーン名")]
    public string nextSceneName = "PostExperimentScene";  // ← Inspector で指定可能

    private PositionUpdater[] positionUpdatersWithRotation;
    private PositionUpdaterWORotation[] positionUpdatersWithoutRotation;
    private PlayerPositionUpdater[] playerPositionUpdaters;

    private AudioSource audioSource;
    public AudioClip sound1;

    private HeadRotationRecorder recorder;
    private bool waitingToSave = false;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
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
        Debug.Log("再生が完了しました。記録を停止し、次のシーンに遷移します。");

        recorder?.StopRecording();
        waitingToSave = true;

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("遷移先シーン名が設定されていません。");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (waitingToSave)
        {
            Debug.Log($"シーン {scene.name} で記録を保存します。");
            recorder = FindObjectOfType<HeadRotationRecorder>();
            recorder?.SaveRecording();
            waitingToSave = false;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
