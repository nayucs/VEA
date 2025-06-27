using UnityEngine;

public class RealWorldSync : MonoBehaviour
{
    public Transform playerAvatar; // 仮想空間内で動かしたいオブジェクト（例：アバター）

    private Transform hmdTransform;

    void Start()
    {
        // Meta Quest Pro などで使用する場合、HMDのTransformを取得
        hmdTransform = Camera.main.transform;
    }

    void Update()
    {
        if (hmdTransform == null || playerAvatar == null) return;

        // 現実空間の位置・回転を仮想空間に反映
        playerAvatar.position = hmdTransform.position;
    }
}
