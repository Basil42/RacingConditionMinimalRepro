
using UnityEngine;



public class Initializer : MonoBehaviour
{
    public SceneLoader Loader;
    
    private void Awake()
    {
#if DEVELOPMENT_BUILD
        Debug.Log("Initializing");
#endif
        Loader.LoadSceneSingle();

        DontDestroyOnLoad(Camera.main);
    }
#if DEVELOPMENT_BUILD
    private void OnGUI()
    {
        if (Loader.handle.IsValid()) GUILayout.Label(Loader.handle.PercentComplete.ToString());
        else GUILayout.Label("No valid handle");
        GUILayout.Label(Time.realtimeSinceStartup.ToString());
    }
#endif
}
