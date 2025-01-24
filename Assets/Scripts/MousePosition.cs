using UnityEngine;

public class MousePosition : MonoBehaviour
{
    public static MousePosition Instance { get; private set; }

    private Vector3 mousePosition;

    [SerializeField]
    private Camera targetCamera; // 対象となるカメラ（未設定時はデフォルトカメラ）

    private void Awake()
    {
        // シングルトンパターンでインスタンスを管理
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // デフォルトカメラの設定
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Update()
    {
        // マウスのスクリーン座標を取得
        mousePosition = Input.mousePosition;
    }

    public Vector3 GetMousePosition()
    {
        // スクリーン空間のマウス座標を取得
        return mousePosition;
    }

    public Vector3 GetMouseWorldPosition()
    {
        // スクリーン座標をワールド座標に変換
        if (targetCamera != null)
        {
            return targetCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, targetCamera.nearClipPlane));
        }
        else
        {
            Debug.LogWarning("Target Camera is not assigned.");
            return Vector3.zero;
        }
    }
}
