using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
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
    [SerializeField] private Text betAmmountText;
    [SerializeField] private Text currentMoneyText;
    [SerializeField] private Text peekPrediction1;
    [SerializeField] private Text peekPrediction2;
    [SerializeField] private int symbolCount = 7;
    [SerializeField] private GameObject LockCanvas;

    [Header("Spin Settings")]
    [SerializeField] private float reelSpinTime = 3f;

    [Header("Bet Settings")]
    [SerializeField] private int[] betAmmountChoices;
    [SerializeField] private int winMultiplier=10;

    [Header("Item Settings")]
    [SerializeField] private int[] items;

    private int betAmmount;
    private int betAmmountPosition=0;
    private int currentMoney = 200;
    private int r1;
    private int r2;
    private int r3;
    private bool isSpinning = false;
    void Start()
    {
        betAmmount = betAmmountChoices[betAmmountPosition];
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
            currentMoney += winMultiplier*betAmmount;
            currentMoneyText.text = currentMoney.ToString();
        }
        else if(r1 == r2)
        {
            RecieveItem(r1);
            currentMoney -= betAmmount;
            currentMoneyText.text = currentMoney.ToString();
        }
        else if(r2 == r3)
        {
            RecieveItem(r2);
            currentMoney -= betAmmount;
            currentMoneyText.text = currentMoney.ToString();
        }
        else if(r1 == r3)
        {
            RecieveItem(r1);
            currentMoney -= betAmmount;
            currentMoneyText.text = currentMoney.ToString();
        }
        else
        {
            currentMoney -= betAmmount;
            currentMoneyText.text = currentMoney.ToString();
        }
    }
    private void BetChange()
    {
        betAmmount = betAmmountChoices[betAmmountPosition];
        betAmmountText.text = betAmmount.ToString();
    }
    public void BetIncrease(bool yes)
    {
        if (yes)
        {
            if (betAmmountPosition < betAmmountChoices.Length-1)
            {
                betAmmountPosition++;
                BetChange();
            }
        }
        if (!yes)
        {
            if (betAmmountPosition != 0)
            {
                betAmmountPosition--;
                BetChange();
            }
        }
    }
    private void RecieveItem(int reel)
    {
    }
    public void UsePeek()
    {   
        peekPrediction1.text = r1.ToString();
        peekPrediction2.text = r2.ToString();
    }
    public void UseLock()
    {
        LockCanvas.SetActive(true);
    }
    public void LockReelSet(int reel3)
    {
        r3 = reel3;
        LockCanvas.SetActive(false);
    }
}
