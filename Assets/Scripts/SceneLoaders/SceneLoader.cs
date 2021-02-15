using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public AssetReference Scene;
    [HideInInspector]public AsyncOperationHandle<SceneInstance> handle;
    public static Stack<AsyncOperationHandle<SceneInstance>> SceneStack;//used to manage additive scene loading as a popup stack
#if UNITY_EDITOR
    private Scene playScene;
    static bool cleared= false;
#endif

    public void Awake()
    {
        if (SceneStack == null) SceneStack = new Stack<AsyncOperationHandle<SceneInstance>>();
#if UNITY_EDITOR
        playScene = SceneManager.GetActiveScene();
#endif
    }
    public void LoadSceneAsPopup()//called by UI button setup in the editor
    {
        Addressables.LoadSceneAsync(Scene, UnityEngine.SceneManagement.LoadSceneMode.Additive, true).Completed += OnLoadAdditiveSceneComplete;
    }
    public void LoadSceneAsPopup(AssetReference scene)
    {
        if(scene == null)
        {
            Debug.LogError("Passed a null reference to a scene loader, aborting popup loading", this);
            return;
        }
        Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive, true).Completed += OnLoadAdditiveSceneComplete;
    }
    private void OnLoadAdditiveSceneComplete(AsyncOperationHandle<SceneInstance> obj)
    {
        
        SceneStack.Push(obj);
    }
    public void PopScene()
    {
        if (SceneStack.Count == 0)
        {
#if UNITY_EDITOR
            Debug.Log("Request to pop the scene registered, but ignored, as there are currently no scene under this one.\n You might need to load this popup from another scene to test this behavior");
            return;
#else
            Debug.LogError("Attempted to pop the last scene. Aborting Scene unloading.");
            return;
#endif
        }
        StartCoroutine(PopSceneRoutine());
    }
    public IEnumerator PopSceneRoutine()
    {
        if (false) yield return null;
        
        var handle = SceneStack.Pop();
        
        Addressables.UnloadSceneAsync(handle, true).Completed += op =>
        {
            if (op.Status != AsyncOperationStatus.Succeeded) Debug.LogError("Failed to unload scene: " + op.DebugName);
        };
    }
    public void LoadSceneSingle()
    {
        if(Scene == null)
        {
            Debug.LogError("No default scene to load on SceneLoader.");
        }
        handle = Addressables.LoadSceneAsync(Scene, LoadSceneMode.Additive, false);
        handle.Completed += OnLoadSceneSingleComplete;


    }
    public void LoadSceneSingle(AssetReference scene)
    {
        if (scene == null)
        {
            Debug.LogError("Passed a null reference to a scene loader, aborting scene loading", this);
            return;
        }
        Addressables.LoadSceneAsync(scene, LoadSceneMode.Single, true).Completed += OnLoadSceneSingleComplete;
    }
    private void OnLoadSceneSingleComplete(AsyncOperationHandle<SceneInstance> obj)
    {

        StartCoroutine(LoadSceneSingleRoutine(obj));
        
    }
    private IEnumerator LoadSceneSingleRoutine(AsyncOperationHandle<SceneInstance> obj)
    {
        
        if(false)yield return null;
        
        while (SceneStack.Count > 0)
        {
            Addressables.UnloadSceneAsync(SceneStack.Pop());
        }
        obj.Result.ActivateAsync();
        SceneStack.Push(obj);
#if UNITY_EDITOR
        if (!cleared)
        {
            SceneManager.UnloadSceneAsync(playScene);//should take care of cases where we open a scene from the editor
            cleared = true;
        }

#endif
        
    }
    
}
