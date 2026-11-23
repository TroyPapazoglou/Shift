using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_IOS || UNITY_ANDROID
        QualitySettings.vSyncCount = 0;   
        Application.targetFrameRate = 120;   
#endif
    }

}
