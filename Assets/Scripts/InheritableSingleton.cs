using UnityEngine;

// just copy this

#pragma warning disable 0649
public class InheritableSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    private static object lockObj = new object();
    private static bool shuttingDown = false;

    public static T Instance
    {
        get
        {
            if (shuttingDown)
            {

#if UNITY_EDITOR
                Debug.LogError("Shutting down is true...");
#endif

                return null;
            }

            lock (lockObj)
            {
                if (instance == null)
                {
                    T[] results = FindObjectsOfType<T>();

                    if (results.Length > 0)
                    {
                        if (results.Length == 1)
                        {
                            instance = results[0];
                        }
                        else
                        {
#if UNITY_EDITOR
                            Debug.LogWarning("Found " + results.Length + " results, returning none\nResults: ");

                            foreach (T obj in results)
                            {
                                Debug.Log(obj.gameObject.name);
                            }
#endif

                            return null;
                        }
                    }
                    else
                    {

#if UNITY_EDITOR
                        Debug.LogWarning("No results found. Returning null.");
#endif

                        return null;
                    }
                }

                return instance;
            }
        }
        private set
        {
            instance = value;
        }
    }

    private void Awake()
    {
        Instance = this.GetComponent<T>();
    }

    private void OnEnable()
    {
        shuttingDown = false;
    }

    private void OnDisable()
    {
        shuttingDown = true;
    }

    private void OnDestroy()
    {
        shuttingDown = true;
    }
}
#pragma warning restore 0649