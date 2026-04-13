using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
   public bool dontDestroyOnLoad;

    private void Awake()
    {
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
