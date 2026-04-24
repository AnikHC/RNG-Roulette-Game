using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;
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
    [SerializeField] private GameObject[] itemButtons;

    [Header("Spin Settings")]
    [SerializeField] private float reelSpinTime = 3f;

    [Header("Bet Settings")]
    [SerializeField] private int[] betAmountChoices;
    [SerializeField] private int winMultiplier = 10;

    [Header("Item Settings")]
    [SerializeField] private int itemsPermittedPerRound = 2;
    [SerializeField] private Sprite[] items;

    private int betAmount;
    private int betAmountPosition=0;
    private int currentMoney = 300;
    private int r1;
    private int r2;
    private int r3;
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
        r1 = Random.Range(1,symbolCount+1);
        r2 = Random.Range(1,symbolCount+1);
        r3 = Random.Range(1,symbolCount+1);
    }
    public void SpinButton()
    {
        if(!isSpinning)StartCoroutine(ReelSpin());
    }
    
    IEnumerator ReelSpin()
    {
        itemsUsedThisRound = 0;
        isSpinning = true;
        //animation for reel1
        //animation for reel2
        //animation for reel3
        reel1.GetComponent<ReelScript>().StartSpin();
        reel2.GetComponent<ReelScript>().StartSpin();
        reel3.GetComponent<ReelScript>().StartSpin();
        yield return new WaitForSeconds (reelSpinTime);
        //stop animation for reel1
        reel1.GetComponent<ReelScript>().StopSpin(r1-1);
        yield return new WaitForSeconds (reelSpinTime);
        //stop animation for reel2
        reel2.GetComponent<ReelScript>().StopSpin(r2-1);
        yield return new WaitForSeconds (reelSpinTime);
        //stop animation for reel3
        reel3.GetComponent<ReelScript>().StopSpin(r3-1);

        Debug.Log($"{r1},{r2},{r3}");
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
         if (r1 == r2 && r2 == r3)
        {
            if(!isDoubleMultiplier){
                currentMoney += winMultiplier*betAmount;
            }
            else {
                currentMoney += winMultiplier*betAmount*2;
            }
            currentMoneyText.text = currentMoney.ToString();
        }
        else if(r1 == r2)
        {
            ReceiveItem(r1);
            ApplyLossForForMatchTwo();
        }
        else if(r2 == r3)
        {
            ReceiveItem(r2);
            ApplyLossForForMatchTwo();
        }
        else if(r1 == r3)
        {
            ReceiveItem(r1);
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
    }
    private void BetChange()
    {
        betAmount = betAmountChoices[betAmountPosition];
        betAmountText.text = betAmount.ToString();
    }
    public void BetIncrease(bool yes)
    {
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
    private void ReceiveItem(int reel)
    {
        itemButtons[reel-1].GetComponent<Button>().interactable = true;
    }
    public void UsePeek()
    {   
        if(!isSpinning&&itemsUsedThisRound<itemsPermittedPerRound){
            itemsUsedThisRound++;
            peekPrediction1.sprite = items[r1-1];
            AlphaChange(peekPrediction1, 1f);
            peekPrediction2.sprite = items[r2-1];
            AlphaChange(peekPrediction2, 1f);
            itemButtons[0].GetComponent<Button>().interactable = false;
        }
    }
    public void UseLock()
    {   if(!isSpinning&&itemsUsedThisRound<itemsPermittedPerRound){
            itemsUsedThisRound++;
            lockCanvas.SetActive(true);
            itemButtons[1].GetComponent<Button>().interactable = false;
        }
    }
    public void LockReelSet(int reel3)
    {
        r3 = reel3;
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
            itemsUsedThisRound++;
            reRollCanvas.SetActive(true);
            itemButtons[3].GetComponent<Button>().interactable = false;
        }
    }
    public void RerollReelSet(int reel)
    {
            StartCoroutine(OneReelSpin(reel));
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
        isSpinning=true;
        if (reel == 1)
        {
            r1 = Random.Range(1,symbolCount+1);
            reel1.GetComponent<ReelScript>().StartSpin();
            //animation for r1
            yield return new WaitForSeconds(reelSpinTime);
            reel1.GetComponent<ReelScript>().StopSpin(r1-1);
        }
        if (reel == 2)
        {
            r2 = Random.Range(1,symbolCount+1);
            reel2.GetComponent<ReelScript>().StartSpin();
            //animation for r2
            yield return new WaitForSeconds(reelSpinTime);
            reel2.GetComponent<ReelScript>().StopSpin(r2-1);
        }
        if (reel == 3)
        {
            r3 = Random.Range(1,symbolCount+1);
            reel3.GetComponent<ReelScript>().StartSpin();
            //animation for r3
            yield return new WaitForSeconds(reelSpinTime);
            reel3.GetComponent<ReelScript>().StopSpin(r3-1);
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
}
