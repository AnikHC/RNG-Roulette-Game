using UnityEngine;

public class ReelScript : MonoBehaviour
{
    public RectTransform reel;
    public float spinSpeed = 3000f;
    public float symbolHeight = 93.75f;
    public float reelOffset = -17.6f;
    private float currentSpeed;
    private float maxheight;
    private bool isSpinning = false;
    void Start()
    {
        currentSpeed = spinSpeed;
        maxheight = reel.rect.height;
    }
    private void Update()
    {
        if(isSpinning){
            reel.anchoredPosition += Vector2.down * currentSpeed * Time.deltaTime;
            if(reel.anchoredPosition.y <= -205.1){
                reel.anchoredPosition = new Vector2(reel.anchoredPosition.x, reelOffset);
            }
        }
    }
    public void StartSpin(){
        isSpinning = true;
        currentSpeed = spinSpeed;
    }

    public void StopSpin(int resultIndex){
        isSpinning = false;
        resultIndex = resultIndex-2;
        float targetPos = resultIndex * symbolHeight + reelOffset;
        reel.anchoredPosition = new Vector2(reel.anchoredPosition.x, targetPos);
    }
}
