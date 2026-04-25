using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlotSpinScript : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private GameObject reel1;
    [SerializeField] private GameObject reel2;
    [SerializeField] private GameObject reel3;
    [SerializeField] private Text betAmountText;
    [SerializeField] private Text currentMoneyText;
    [SerializeField] private Image peekPrediction1;
    [SerializeField] private Image peekPrediction2;
    [SerializeField] private int symbolCount = 5;
    [SerializeField] private GameObject lockCanvas;
    [SerializeField] private GameObject reRollCanvas;
    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private GameObject[] itemButtons;

    [Header("Spin Settings")]
    [SerializeField] private float reelSpinTime = 3f;

    [Header("Bet Settings")]
    [SerializeField] private int[] betAmountChoices;
    [SerializeField] private int winMultiplier = 10;
    [SerializeField] private int currentMoney = 300;

    [Header("Item Settings")]
    [SerializeField] private int itemsPermittedPerRound = 2;
    [SerializeField] private Sprite[] items;

    private int betAmount;
    private int betAmountPosition=0;
    private int[] reelItemNum = new int[3];

    // To store the old reel values when using the reroll item, 
    // so that only the selected reel is changed when the re roll item is used.
    private int r1Old;
    private int r2Old;
    private int r3Old;

    private int itemsUsedThisRound = 0;
    private bool isSpinning = false;
    private bool isDoubleMultiplier = false;
    private bool insuranceUsed = false;
    void Start()
    {
        betAmount = betAmountChoices[betAmountPosition];
        currentMoneyText.text = currentMoney.ToString();
        AlphaChange(peekPrediction1, 0f);
        AlphaChange(peekPrediction2, 0f);
        SetReels();
    }
    private void SetReels()
    {
        reelItemNum[0] = Random.Range(1,symbolCount+1);
        reelItemNum[1] = Random.Range(1,symbolCount+1);
        reelItemNum[2] = Random.Range(1,symbolCount+1);
    }
    // Overloaded method to set the reels to the old values when using the re roll item, 
    // so that only the selected reel is changed.
    private void SetReels(int reelIndex)
    {
        if (reelIndex == 1)
        {
            reelItemNum[1] = r2Old;
            reelItemNum[2] = r3Old;
        }
        if(reelIndex == 2)
        {
            reelItemNum[0] = r1Old;
            reelItemNum[2] = r3Old;
        }
        if(reelIndex == 3)
        {
            reelItemNum[0] = r1Old;
            reelItemNum[1] = r2Old;
        }
    }
    public void SpinButton()
    {
        if(!isSpinning)StartCoroutine(ReelSpin());
    }
    
    IEnumerator ReelSpin()
    {
        r1Old = reelItemNum[0];
        r2Old = reelItemNum[1];
        r3Old = reelItemNum[2];

        itemsUsedThisRound = 0;
        isSpinning = true;

        reel1.GetComponent<ReelScript>().StartSpin();
        reel2.GetComponent<ReelScript>().StartSpin();
        reel3.GetComponent<ReelScript>().StartSpin();

        yield return new WaitForSeconds (reelSpinTime);
        reel1.GetComponent<ReelScript>().StopSpin(reelItemNum[0]-1);

        yield return new WaitForSeconds (reelSpinTime);
        reel2.GetComponent<ReelScript>().StopSpin(reelItemNum[1]-1);

        yield return new WaitForSeconds (reelSpinTime);
        reel3.GetComponent<ReelScript>().StopSpin(reelItemNum[2]-1);

        CheckResult();
        SetReels();

        peekPrediction1.sprite = null;
        AlphaChange(peekPrediction1, 0f);

        peekPrediction2.sprite = null;
        AlphaChange(peekPrediction2, 0f);

        isSpinning = false;
    }
    // Checks the result of the spin, and applies the appropriate win/loss and item effects based on the result.
    // excluding match of 2 reels, as that is handled in the ApplyLossForForMatchTwo method, to avoid code repetition.
    private void CheckResult()
    {
        Debug.Log($"{reelItemNum[0]},{reelItemNum[1]},{reelItemNum[2]}");
         if (reelItemNum[0] == reelItemNum[1] && reelItemNum[1] == reelItemNum[2])
        {
            if(!isDoubleMultiplier){
                currentMoney += winMultiplier*betAmount;
            }
            else {
                currentMoney += winMultiplier*betAmount*2;
            }
            currentMoneyText.text = currentMoney.ToString();
            if(currentMoney >= 2000){
                EndGame(1);
            }
        }
        else if(reelItemNum[0] == reelItemNum[1])
        {
            ReceiveItem(reelItemNum[0]);
            ApplyLossForForMatchTwo();
        }
        else if(reelItemNum[1] == reelItemNum[2])
        {
            ReceiveItem(reelItemNum[1]);
            ApplyLossForForMatchTwo();
        }
        else if(reelItemNum[0] == reelItemNum[2])
        {
            ReceiveItem(reelItemNum[0]);
            ApplyLossForForMatchTwo();
        }
        else
        {
            if(!insuranceUsed){
                if(!isDoubleMultiplier){
                    currentMoney -= betAmount;
                }
                else{
                    currentMoney -= betAmount*2;
                }
                currentMoneyText.text = currentMoney.ToString();
        }
        insuranceUsed = false;
        }
        isDoubleMultiplier = false;
    }
    // Used when there is a match of 2 symbols, to apply the loss for the match of 2, 
    // and to check if the player has gone bankrupt.
    private void ApplyLossForForMatchTwo()
    {   if(!insuranceUsed){
            if(!isDoubleMultiplier){
                currentMoney -= betAmount/2;
            }
            else{
                currentMoney -= betAmount;
            }
            currentMoneyText.text = currentMoney.ToString();
        }
        insuranceUsed = false;
        if (currentMoney <= 0){
            EndGame(0);
        }
    }
    private void BetChange()
    {
        betAmount = betAmountChoices[betAmountPosition];
        betAmountText.text = betAmount.ToString();
    }
    // Used to increase or decrease the bet amount, depending on the input. 
    // Only works if the reels are not currently spinning. 
    // Used bool to determine whether to increase or decrease the bet amount, to avoid code repetition.
    public void BetIncrease(bool yes)
    {
        if(!isSpinning){
            if (yes)
            {
                if (betAmountPosition < betAmountChoices.Length-1)
                {
                    betAmountPosition++;
                    BetChange();
                }
            }
            if (!yes)
            {
                if (betAmountPosition != 0)
                {
                    betAmountPosition--;
                    BetChange();
                }
            }
        }
    }
    // Reel-1 to make the unity inspector more user friendly, 
    // as the reels are numbered 1-3 in the inspector, but are indexed 0-2 in the code.
    private void ReceiveItem(int reel)
    {
        itemButtons[reel-1].GetComponent<Button>().interactable = true;
    }
    public void UsePeek()
    {   
        if(!isSpinning&&itemsUsedThisRound<itemsPermittedPerRound){
            itemsUsedThisRound++;
            peekPrediction1.sprite = items[reelItemNum[0]-1];
            AlphaChange(peekPrediction1, 1f);
            peekPrediction2.sprite = items[reelItemNum[1]-1];
            AlphaChange(peekPrediction2, 1f);
            itemButtons[0].GetComponent<Button>().interactable = false;
        }
    }
    public void UseLock()
    {   if(!isSpinning&&itemsUsedThisRound<itemsPermittedPerRound){
            lockCanvas.SetActive(true);
        }
    }
    public void LockReelSet(int reel3)
    {
        if(reel3!=6){
            reelItemNum[2] = reel3;
            itemsUsedThisRound++;
            itemButtons[1].GetComponent<Button>().interactable = false;
        }
        lockCanvas.SetActive(false);
    }
    public void UseDouble()
    {   if(!isSpinning&&!isDoubleMultiplier&&itemsUsedThisRound<itemsPermittedPerRound){
            itemsUsedThisRound++;
            isDoubleMultiplier = true;
            itemButtons[2].GetComponent<Button>().interactable = false;
        }
    }
    public void UseReroll()
    {
        if(!isSpinning&&itemsUsedThisRound<itemsPermittedPerRound){
            reRollCanvas.SetActive(true);
        }
    }
    public void RerollReelSet(int reel)
    {
        if(reel!=4){
            itemsUsedThisRound++;
            itemButtons[3].GetComponent<Button>().interactable = false;
            SetReels(reel);
            StartCoroutine(OneReelSpin(reel));
        }
        reRollCanvas.SetActive(false);
    }
    public void UseInsurance()
    {
        if (!isSpinning&&itemsUsedThisRound<itemsPermittedPerRound){
            itemsUsedThisRound++;
            insuranceUsed = true;
            itemButtons[4].GetComponent<Button>().interactable = false;
        }
    }
    // Used to spin one reel when the re roll item is used, so that only the selected reel is changed, 
    // and to avoid code repetition.
    IEnumerator OneReelSpin(int reel)
    {
        r1Old = reelItemNum[0];
        r2Old = reelItemNum[1];
        r3Old = reelItemNum[2];

        isSpinning=true;
        if (reel == 1)
        {
            reelItemNum[0] = Random.Range(1,symbolCount+1);
            
            reel1.GetComponent<ReelScript>().StartSpin();

            yield return new WaitForSeconds(reelSpinTime);
            reel1.GetComponent<ReelScript>().StopSpin(reelItemNum[0]-1);
        }
        if (reel == 2)
        {
            reelItemNum[1] = Random.Range(1,symbolCount+1);

            reel2.GetComponent<ReelScript>().StartSpin();

            yield return new WaitForSeconds(reelSpinTime);
            reel2.GetComponent<ReelScript>().StopSpin(reelItemNum[1]-1);
        }
        if (reel == 3)
        {
            reelItemNum[2] = Random.Range(1,symbolCount+1);

            reel3.GetComponent<ReelScript>().StartSpin();

            yield return new WaitForSeconds(reelSpinTime);
            reel3.GetComponent<ReelScript>().StopSpin(reelItemNum[2]-1);
        }
        CheckResult();
        SetReels();
        isSpinning=false;
    }
    // Used to change the alpha of an image, used for the peek item to make the peek predictions visible or invisible.
    // Used because if sprite is null, the image will still be visible.
    private void AlphaChange(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
    private void EndGame(int condition)
    {
        endGameCanvas.SetActive(true);
        endGameCanvas.transform.GetChild(condition).gameObject.SetActive(true);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
