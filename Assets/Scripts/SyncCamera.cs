using UnityEngine;

public class SyncCamera : MonoBehaviour
{
    public Transform centerEyeAnchor; // OVRCameraRig‚ÌCenterEyeAnchor
    private Transform myCamTransform;

    void Start()
    {
        myCamTransform = transform; // ƒJƒƒ‰‚ÌTransform‚ğæ“¾
    }

    void LateUpdate()
    {
        if (centerEyeAnchor != null)
        {
            myCamTransform.position = centerEyeAnchor.position;
            myCamTransform.rotation = centerEyeAnchor.rotation;
        }
    }
}
