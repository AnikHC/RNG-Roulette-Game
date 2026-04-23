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
    [SerializeField] private Text reel1;
    [SerializeField] private Text reel2;
    [SerializeField] private Text reel3;
    [SerializeField] private Text betAmountText;
    [SerializeField] private Text currentMoneyText;
    [SerializeField] private Text peekPrediction1;
    [SerializeField] private Text peekPrediction2;
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
        reel1.text = "Sp";
        reel2.text = "Sp";
        reel3.text = "Sp";
        yield return new WaitForSeconds (reelSpinTime);
        //stop animation for reel1
        reel1.text = r1.ToString();
        yield return new WaitForSeconds (reelSpinTime);
        //stop animation for reel2
        reel2.text = r2.ToString();
        yield return new WaitForSeconds (reelSpinTime);
        //stop animation for reel3
        reel3.text = r3.ToString();
        CheckResult();
        SetReels();
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
        itemButtons[reel-1].SetActive(true);
    }
    public void UsePeek()
    {   
        if(!isSpinning&&itemsUsedThisRound<itemsPermittedPerRound){
            itemsUsedThisRound++;
            peekPrediction1.text = r1.ToString();
            peekPrediction2.text = r2.ToString();
            itemButtons[0].SetActive(false);
        }
    }
    public void UseLock()
    {   if(!isSpinning&&itemsUsedThisRound<itemsPermittedPerRound){
            itemsUsedThisRound++;
            lockCanvas.SetActive(true);
            itemButtons[1].SetActive(false);
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
            itemButtons[2].SetActive(false);
        }
    }
    public void UseReroll()
    {
        if(!isSpinning&&itemsUsedThisRound<itemsPermittedPerRound){
            itemsUsedThisRound++;
            reRollCanvas.SetActive(true);
            itemButtons[3].SetActive(false);
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
            itemButtons[4].SetActive(false);
        }
    }
    IEnumerator OneReelSpin(int reel)
    {
        isSpinning=true;
        if (reel == 1)
        {
            r1 = Random.Range(1,symbolCount+1);
            reel1.text = "Sp";
            //animation for r1
            yield return new WaitForSeconds(reelSpinTime);
            reel1.text = r1.ToString();
        }
        if (reel == 2)
        {
            r2 = Random.Range(1,symbolCount+1);
            reel2.text = "Sp";
            //animation for r2
            yield return new WaitForSeconds(reelSpinTime);
            reel2.text = r2.ToString();
        }
        if (reel == 3)
        {
            r3 = Random.Range(1,symbolCount+1);
            reel3.text = "Sp";
            //animation for r3
            yield return new WaitForSeconds(reelSpinTime);
            reel3.text = r3.ToString();
        }
        CheckResult();
        SetReels();
        isSpinning=false;
    }
}
