using System;
using TMPro;
using Unity.Mathematics;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class UIWorkManager : MonoBehaviour
{
    public static UIWorkManager Instance;
    // work overlay
    [Header("Overlay")]
    public GameObject workoverlayUI;
    public GameObject ingameOverlayUI;
    public TextMeshProUGUI scenarioText;
    public TextMeshProUGUI moneyEarned;
    public Image bonusFill;
    // question and ans
    [Header("QA")]
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI questionText;

    public GameObject numInputAnsObj;
    public GameObject multiChoiceAnsObj;
    public TMP_InputField inputTxt;
    public TextMeshProUGUI unitsTxt;

    public TextMeshProUGUI hintTxt;

    [SerializeField] private GameObject[] choiceButtons;
    // feedback
    [Header("Feedback")]
    public GameObject feedbackObj;
    public TextMeshProUGUI feedbackHeaderTxt;
    public TextMeshProUGUI feedbackTxt;
    public Image feedbackBg;
    public Color correctColor;
    public Color incorrectColor;

    void Awake() { Instance = this; }
    public void UpdateTimer(float timeRemaining) { bonusFill.fillAmount = Mathf.Clamp01(timeRemaining/WorkHandler.bonusTimePerScenario); }
    public void UpdateWorkStats(int to, int totalScenarios) { scenarioText.text = $"{to}/{totalScenarios}"; moneyEarned.text = $"${WorkHandler.Instance.totalEarned:F2}"; }
    public void ShowFeedback(bool isCorrect, FinancialScenario scenario) {
        feedbackObj.SetActive(true);
        feedbackHeaderTxt.text = isCorrect? "Correct" : "Incorrect...";
        feedbackBg.color = isCorrect? correctColor : incorrectColor;
        isMultiChoice = scenario.choices != null && scenario.choices.Length > 0;
        if (isMultiChoice)
            feedbackTxt.text = scenario.choices[scenario.correctChoiceIndex];
        else
            feedbackTxt.text = $"{scenario.correctAnswerFloat:F2}";
    }
    // displays the provided scenario
    public void DisplayScenario(FinancialScenario scenario)
    {
        descriptionText.text = scenario.description;
        questionText.text = scenario.question;
        feedbackObj.SetActive(false);
        // set the hint text
        unitsTxt.text = scenario.units;
        hintTxt.text = scenario.hintText;
        // determine if multichoice based on whether choices exist
        isMultiChoice = scenario.choices != null && scenario.choices.Length > 0;
        
        numInputAnsObj.SetActive(!isMultiChoice);
        multiChoiceAnsObj.SetActive(isMultiChoice);
        
        if (isMultiChoice)
        {
            // set the options
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < scenario.choices.Length)
                {
                    choiceButtons[i].SetActive(true);
                    choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = scenario.choices[i];
                }
                else
                    choiceButtons[i].SetActive(false);
            }
            // get children
            Transform parent = choiceButtons[0].transform.parent;
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < parent.childCount; i++)
                children.Add(parent.GetChild(i));

            // shuffle the choices
            int n = children.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1); // exclusive max
                Transform value = children[k];
                children[k] = children[n];
                children[n] = value;
            }
            // reorder in hierarchy
            for (int i = 0; i < children.Count; i++)
            {
                // also deactivate highlights
                children[i].SetSiblingIndex(i);
                children[i].GetChild(1).gameObject.SetActive(false);
            }
        }
        else
        {
            unitsTxt.text = scenario.units;
        }
    }
    int selectedChoiceIndex = -1;
    bool isMultiChoice = false;
    // called from the choice select buttons
    public void ChoiceButtonClicked(int choiceIndex) { selectedChoiceIndex = choiceIndex; }
    // called from the submit button
    public void SubmitButtonClicked()
    {
        if (WorkHandler.Instance.inReviewTime)
            return;
        // no answer inputted
        if (isMultiChoice && selectedChoiceIndex == -1)
            return;
        if (inputTxt.text == "" && !isMultiChoice)
            return;
        // answer the question
        if (isMultiChoice)
            WorkHandler.Instance.SubmitChoice(selectedChoiceIndex);
        else
            WorkHandler.Instance.SubmitAnswer(float.Parse(inputTxt.text));
        // reset
        selectedChoiceIndex = -1;
        inputTxt.text = "";
    }
    // cancels the work, called from a button
    public void CancelWork()
    {
        string header = "Stop working?";
        string body = "Do you want to stop working? You will lose any earned money.";
        UIPopups.Instance.PopupYN(header,body, () =>
        {
            WorkHandler.Instance.CancelShift();
            CameraHandler.Instance.ToggleScrollerBG(true);
            ingameOverlayUI.SetActive(true);
            workoverlayUI.SetActive(false);
        }, () => {});
    }
    // enters work, called from the working functionality when the go to work action is pressed
    public void EnterWork()
    {
        string header = "Start working";
        string body = "Do you want to start working?";
        UIPopups.Instance.PopupYN(header,body, () =>
        {
            CameraHandler.Instance.ToggleScrollerBG(false);
            workoverlayUI.SetActive(true);
            ingameOverlayUI.SetActive(false);

            WorkHandler.Instance.BeginShift();
        }, null, "Start", "Nevermind");
    }
    // ends the shift
    public void EndShift()
    {
        string body = $"Great work! You earned ${WorkHandler.Instance.totalEarned:F2} for your hard work! 8 hours have passed.";
        UIPopups.Instance.PopupInfo("Job well done!",body,"Yay!",() =>
        {
            WorkHandler.Instance.EndShift();
            CameraHandler.Instance.ToggleScrollerBG(true);
            ingameOverlayUI.SetActive(true);
            workoverlayUI.SetActive(false);
        });
    }
}