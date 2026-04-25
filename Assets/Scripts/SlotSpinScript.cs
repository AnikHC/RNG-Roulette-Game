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
