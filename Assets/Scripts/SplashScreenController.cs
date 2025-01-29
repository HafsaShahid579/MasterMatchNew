using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro
using DG.Tweening; // For DOTween animations
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
    public Image Logo; // Reference to your logo image
    public RectTransform LogoRect; // Reference to your logo RectTransform
    public TextMeshProUGUI loadingText; // Reference to your Loading... text
    public Image FillBar;
    public RectTransform movingImageRect;

    private int count = 0; // For Loading... animation

    private void Start()
    {
        // Start both animations
        LogoAnimation();
        LoadingBar();
        InvokeRepeating("UpdateLoadingText", 0f, 0.5f); // 0.5f is the interval for text update
    }

    private void LoadingBar()
    {
        if (FillBar != null)
        {
            FillBar.DOPause();
            FillBar.fillAmount = 0;

            // Animate the fill bar and move the image along it.
            DOTween.To(
                () => FillBar.fillAmount,
                x => {
                    FillBar.fillAmount = x;
                    UpdateMovingImagePosition(x); // Update image position based on fill amount.
            },
                1f,
                5
            ).SetEase(Ease.Linear).OnComplete(() =>
            {
                SceneManager.LoadScene("Gameplay");
            });
        }

    }

    private void LogoAnimation()
    {
        // Move logo to initial position instantly
        Logo.transform.DOLocalMoveY(1500, 0.01f);
        
        // Animate logo moving and rotating
        Logo.transform.DOLocalMoveY(370, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            LogoRect.DOLocalRotate(new Vector3(0, 360, 0), 0.3f, RotateMode.FastBeyond360).OnComplete(() =>
            {
                Logo.transform.DOLocalMoveY(350, 1f).SetLoops(-1, LoopType.Yoyo);
            });
        });
    }

    private void UpdateLoadingText()
    {
        // Update Loading... text based on count value
        if (count == 0)
        {
            loadingText.text = "Loading.";
        }
        else if (count == 1)
        {
            loadingText.text = "Loading..";
        }
        else if (count == 2)
        {
            loadingText.text = "Loading...";
        }

        count++; // Increment count

        // Reset count to 0 after reaching 3
        if (count > 2)
        {
            count = 0;
        }
    }
    private void UpdateMovingImagePosition(float fillAmount)
    {
        if (movingImageRect != null && FillBar != null)
        {
            RectTransform fillBarRect = FillBar.GetComponent<RectTransform>();

            // Calculate the center position of the fill bar.
            Vector3 fillBarCenter = fillBarRect.position;

            // Calculate the width of the fill bar in world space.
            float fillBarWidth = fillBarRect.rect.width * fillBarRect.lossyScale.x;

            // Calculate the new position for the moving image starting from the center.
            float offsetX = (fillAmount - 0.5f) * fillBarWidth; // Offset from the center (negative to positive).
            Vector3 newPosition = fillBarCenter + new Vector3(offsetX, 0, 0);

            movingImageRect.position = newPosition;
        }
    }
}
