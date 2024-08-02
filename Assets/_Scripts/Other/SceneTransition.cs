using System.Threading;
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

    CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    Task _t;

    public void InitTransition(LoadSceneDelegate loadSceneDelegate, int sceneId)
    {
        this.sceneId = sceneId;
        this.loadSceneDelegate = loadSceneDelegate;

        _t = Test(_cancellationTokenSource.Token);
    }

    private async Task Test(CancellationToken cancellationToken)
    {
        await FadeInAsync(cancellationToken);
        AsyncOperation asyncHandler = loadSceneDelegate(sceneId);
        asyncHandler.completed += FadeOutAsync;
    }

    private async Task FadeInAsync(CancellationToken cancellationToken)
    {
        float transcurredTime = 0;

        while (transcurredTime < fadeDuration && !cancellationToken.IsCancellationRequested)
        {
            transcurredTime += Time.unscaledDeltaTime;
            canvasGroup.alpha += Time.unscaledDeltaTime;

            await Task.Yield();
        }

        if(cancellationToken.IsCancellationRequested)
            canvasGroup.alpha = 0;
    }

    private async void FadeOutAsync(AsyncOperation a)
    {
        await Task.Delay(500);

        float transcurredTime = 0;

        while (transcurredTime < fadeDuration && !_cancellationTokenSource.IsCancellationRequested)
        {
            transcurredTime += Time.unscaledDeltaTime;
            canvasGroup.alpha -= Time.unscaledDeltaTime;

            await Task.Yield();
        }

        if (_cancellationTokenSource.IsCancellationRequested)
            canvasGroup.alpha = 0;
    }

    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
    }
}