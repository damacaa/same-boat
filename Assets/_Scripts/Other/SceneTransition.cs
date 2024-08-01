using System.Threading.Tasks;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("References")]
    public CanvasGroup canvasGroup;

    [Header("Parameters")]
    public float fadeDuration;

    public delegate AsyncOperation LoadSceneDelegate(int sceneId);
    private LoadSceneDelegate loadSceneDelegate;

    private int sceneId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void InitTransition(LoadSceneDelegate loadSceneDelegate, int sceneId)
    {
        this.sceneId = sceneId;
        this.loadSceneDelegate = loadSceneDelegate;

        Test();
    }

    private async Task Test()
    {
        await FadeInAsync();
        AsyncOperation asyncHandler = loadSceneDelegate(sceneId);
        asyncHandler.completed += FadeOutAsync;
    }

    private async Task FadeInAsync()
    {
        float transcurredTime = 0;

        while (transcurredTime < fadeDuration)
        {
            transcurredTime += Time.unscaledDeltaTime;
            canvasGroup.alpha += Time.unscaledDeltaTime;

            await Task.Yield();
        }
    }

    private async void FadeOutAsync(AsyncOperation a)
    {
        await Task.Delay(500);

        float transcurredTime = 0;

        while (transcurredTime < fadeDuration)
        {
            transcurredTime += Time.unscaledDeltaTime;
            canvasGroup.alpha -= Time.unscaledDeltaTime;

            await Task.Yield();
        }
    }
}