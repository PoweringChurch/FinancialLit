using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor.PackageManager;

public enum ScenarioType 
{ 
    Budgeting, 
    Savings, 
    Comparison, 
    DebtPayoff,
    EmergencyFund,
    NeedsVsWants,
    CreditCardChoice,
    Insurance,
    OpportunityCost,
    Inflation,
    RentVsBuy
}

[Serializable]
public class FinancialScenario
{
    public ScenarioType type;
    public string description;
    public Dictionary<string, float> data; // flexible data for different scenario types
    public string question;
    public float correctAnswerFloat;
    public string correctAnswerTxt;
    public float tolerance; // how close they need to be
    public string[] choices; // for multiple choice scenarios
    public int correctChoiceIndex; // for multiple choice
    public string units;
    public bool multiChoice;
    public FinancialScenario(ScenarioType type, string description, string question)
    {
        this.type = type;
        this.description = description;
        this.question = question;
        this.data = new Dictionary<string, float>();
        this.tolerance = 0.01f;
    }
}

public class WorkHandler : MonoBehaviour
{
    public static WorkHandler Instance;
    public AudioClip nextScenario;
    public float totalEarned = 0f;

    private FinancialScenario currentScenario;
    private int completedScenarioCount = 0;
    private int totalScenarios = 5;
    private float bonusTimer;
    public const float bonusTimePerScenario = 30f;
    private float countdown = 0f;
    private bool shiftActive = false;
    private int difficultyLevel = 1; // scales with completed shifts
    
    public bool ShiftActive
    {
        get { return shiftActive; }
    }
    
    public event Action OnWorkStarted;
    public event Action OnWorkEnded;
    
    void Start()
    {
        Instance = this;
    }
    
    void Update()
    {
        // handle active scenarios
        if (shiftActive && currentScenario != null && !inReviewTime)
        {
            bonusTimer -= Time.deltaTime;
            UIWorkManager.Instance.UpdateTimer(bonusTimer);
        }
    }
    
    public void BeginShift()
    {
        completedScenarioCount = 0;
        totalEarned = 0f;

        shiftActive = true;
        NextScenario();

        OnWorkStarted?.Invoke();
        UIWorkManager.Instance.UpdateWorkStats(0, totalScenarios);
    }
    
    public void NextScenario()
    {
        if (completedScenarioCount > 0)
            UISFXPlayer.Instance.Play(nextScenario);
        
        // Check if shift is complete
        if (completedScenarioCount >= totalScenarios)
        {
            UIWorkManager.Instance.EndShift();
            return;
        }
        
        // Generate new scenario
        currentScenario = GenerateScenario();
        bonusTimer = bonusTimePerScenario;
        inReviewTime = false;
        UIWorkManager.Instance.DisplayScenario(currentScenario);
        UIWorkManager.Instance.UpdateTimer(bonusTimer);
    }
    
    private FinancialScenario GenerateScenario()
    {
        ScenarioType[] numericTypes = { ScenarioType.Budgeting, ScenarioType.Savings, ScenarioType.DebtPayoff, ScenarioType.EmergencyFund };
        ScenarioType[] choiceTypes = { ScenarioType.Comparison, ScenarioType.NeedsVsWants, ScenarioType.CreditCardChoice, ScenarioType.Insurance, ScenarioType.OpportunityCost, ScenarioType.Inflation, ScenarioType.RentVsBuy };
        
        bool useChoice = UnityEngine.Random.value > 0.5f; // 50/50 split
        
        ScenarioType type;
        if (useChoice)
            type = choiceTypes[UnityEngine.Random.Range(0, choiceTypes.Length)];
        else
            type = numericTypes[UnityEngine.Random.Range(0, numericTypes.Length)];
        
        switch (type)
        {
            case ScenarioType.Budgeting: return GenerateBudgetingScenario();
            case ScenarioType.Savings: return GenerateSavingsScenario();
            case ScenarioType.Comparison: return GenerateComparisonScenario();
            case ScenarioType.DebtPayoff: return GenerateDebtScenario();
            case ScenarioType.EmergencyFund: return GenerateEmergencyFundScenario();
            case ScenarioType.NeedsVsWants: return GenerateNeedsVsWantsScenario();
            case ScenarioType.CreditCardChoice: return GenerateCreditCardScenario();
            case ScenarioType.Insurance: return GenerateInsuranceScenario();
            case ScenarioType.OpportunityCost: return GenerateOpportunityCostScenario();
            case ScenarioType.Inflation: return GenerateInflationScenario();
            case ScenarioType.RentVsBuy: return GenerateRentVsBuyScenario();
            default: return GenerateBudgetingScenario();
        }
    }
    string[] names = {
    "Alex", "Avery", "Blake", "Cameron", "Casey", "Dakota", "Dylan", "Emerson", "Finley", "Harper",
    "Hayden", "Jamie", "Jordan", "Kendall", "Logan", "Marley", "Morgan", "Noah", "Parker", "Quinn",
    "Reese", "Riley", "Rowan", "Sawyer", "Skyler", "Spencer", "Taylor", "Toby", "Wyatt", "Zion",
    "Elliot", "Frankie", "Jesse", "Kai", "Lane", "Micah", "Nico", "Oakley", "Remy", "Sage",
    "Theo", "Wesley", "Arlo", "Brooks", "Cody", "Felix", "Leo", "Miles", "Otis", "River",
    "Quineshia", "Ackerman", "Helix", "Seth", "Zimmerman", "Zoe", "Xander", "Mikhael", "Franklin", "Trenton",
    "Brody", "Howard", "August", "Gene", "Brooks"
    };
    private FinancialScenario GenerateBudgetingScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        
        float income = UnityEngine.Random.Range(2000f, 5000f);
        float rent = income * UnityEngine.Random.Range(0.25f, 0.35f);
        float groceries = UnityEngine.Random.Range(300f, 600f);
        float utilities = UnityEngine.Random.Range(100f, 250f);
        float transport = UnityEngine.Random.Range(150f, 400f);
        
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.Budgeting,
            $"{clientName} earns ${income:F0} each month. They must pay ${rent:F0} for rent, spend about ${groceries:F0} on groceries, use ${utilities:F0} for utilities, and pay ${transport:F0} for transportation.",
            "After covering these costs, how much money do they have left?"
        );
        scenario.units = "Dollars";
        scenario.data["income"] = income;
        scenario.data["rent"] = rent;
        scenario.data["groceries"] = groceries;
        scenario.data["utilities"] = utilities;
        scenario.data["transport"] = transport;
        
        scenario.correctAnswerFloat = income - (rent + groceries + utilities + transport);
        scenario.correctAnswerTxt = $"{scenario.correctAnswerFloat:F0}";
        scenario.tolerance = 2f;
        
        return scenario;
    }
    private FinancialScenario GenerateNeedsVsWantsScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        
        string[][] items = new string[][] 
        {
            new string[] { "Designer sneakers", "Basic running shoes", "Work boots (job requirement)", "Luxury slippers" },
            new string[] { "Premium streaming subscriptions", "Basic internet", "Cable TV package", "Gaming console" },
            new string[] { "Gym membership", "Gourmet meal kit", "Groceries", "Restaurant dining" },
            new string[] { "New smartphone (current works fine)", "Car insurance", "Smartwatch", "Concert tickets" }
        };
        
        string[] itemSet = items[UnityEngine.Random.Range(0, items.Length)];
        int needIndex = UnityEngine.Random.Range(0, itemSet.Length);
        
        // Ensure there's actually a clear "need" in the set
        if (itemSet == items[0]) needIndex = 2; // Work boots
        else if (itemSet == items[1]) needIndex = 1; // Basic internet
        else if (itemSet == items[2]) needIndex = 2; // Groceries
        else if (itemSet == items[3]) needIndex = 1; // Car insurance
        
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.NeedsVsWants,
            $"{clientName} has limited budget this month and must prioritize how they spend their money.",
            "Which expense is a need, and not a want?"
        );
        
        scenario.choices = itemSet;
        scenario.correctChoiceIndex = needIndex;
        
        return scenario;
    }

    private FinancialScenario GenerateCreditCardScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        
        float cardA_APR = UnityEngine.Random.Range(15f, 25f);
        float cardB_APR = UnityEngine.Random.Range(15f, 25f);
        int cardA_fee = UnityEngine.Random.Range(0, 100);
        int cardB_fee = UnityEngine.Random.Range(0, 100);
        int cardA_rewards = UnityEngine.Random.Range(1, 3);
        int cardB_rewards = UnityEngine.Random.Range(1, 3);
        
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.CreditCardChoice,
            $"{clientName} is choosing a credit card. They plan to pay off their balance monthly.",
            "Which matters least if they pay in full each month?"
        );
        
        scenario.choices = new string[] 
        { 
            "Annual fee amount",
            "Interest rate (APR)", 
            "Rewards percentage",
            "Credit limit"
        };
        scenario.correctChoiceIndex = 1; // Interest rate doesn't matter if paying in full
        
        return scenario;
    }

    private FinancialScenario GenerateInsuranceScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        
        float lowDeductible = 500f;
        float highDeductible = 2000f;
        float lowPremium = 200f;
        float highPremium = 100f;
        
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.Insurance,
            $"{clientName} rarely gets sick and is choosing a health insurance plan. Plan A costs ${lowPremium}/month and has a ${lowDeductible} deductible. Plan B costs ${highPremium}/month with a ${highDeductible} deductible.",
            "Based on how often they use healthcare, which plan is likely to save them more money over a year?"
        );
        
        float planACost = (lowPremium * 12) + lowDeductible; // Assume one incident
        float planBCost = (highPremium * 12) + highDeductible;
        
        scenario.choices = new string[] 
        { 
            $"Plan A (${lowPremium:F2}/mo, ${lowDeductible:F2} deductible)",
            $"Plan B (${highPremium:F2}/mo, ${highDeductible:F2} deductible)",
            "Both cost the same",
            "Not enough information"
        };
        scenario.correctChoiceIndex = planBCost < planACost ? 1 : 0;
        
        return scenario;
    }

    private FinancialScenario GenerateOpportunityCostScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        
        float amount = UnityEngine.Random.Range(1000f, 3000f);
        float investmentReturn = UnityEngine.Random.Range(5f, 10f);
        
        string[][] options = new string[][]
        {
            new string[] { $"Invest ${amount:F0} (expecting {investmentReturn}% annual return)", $"Buy a TV for ${amount:F0}", $"Take a vacation for ${amount:F0}", "Save it in a 0% checking account" },
            new string[] { "Work overtime for extra $500", $"Spend $500 on a course that could increase salary", "Spend $500 on entertainment", "Save $500 under the mattress" }
        };
        
        string[] optionSet = options[UnityEngine.Random.Range(0, options.Length)];
        
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.OpportunityCost,
            $"{clientName} wants to make the most of their money long-term.",
            "Which choice has the best opportunity cost?"
        );
        
        scenario.choices = optionSet;
        scenario.correctChoiceIndex = optionSet == options[0] ? 0 : 1; // Investment or course
        
        return scenario;
    }

    private FinancialScenario GenerateInflationScenario()
    {
        string[] names = { "Emerson", "Lennon", "Sutton", "Ellis", "Marlowe" };
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        
        float inflationRate = UnityEngine.Random.Range(2f, 5f);
        
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.Inflation,
            $"Inflation is {inflationRate}% this year. {clientName} has $10,000 in savings.",
            "What happens to their purchasing power if money earns 0% interest?"
        );
        
        scenario.choices = new string[] 
        { 
            "It increases",
            "It decreases", 
            "It stays the same",
            "It depends on the stock market"
        };
        scenario.correctChoiceIndex = 1; // Decreases
        
        return scenario;
    }

    private FinancialScenario GenerateRentVsBuyScenario()
    {
        string[] names = { "Hayden", "Sloane", "Jules", "Palmer", "Monroe" };
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        
        float rent = UnityEngine.Random.Range(1200f, 2000f);
        float mortgage = rent * UnityEngine.Random.Range(1.1f, 1.4f);
        int yearsPlanning = UnityEngine.Random.Range(1, 3);
        
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.RentVsBuy,
            $"{clientName} is relocating for a job that will last {yearsPlanning} years. Renting would cost ${rent:F0} per month, while buying a home with a mortgage would cost ${mortgage:F0} per month but would build equity over time.",
            "Considering the length of the job, which option is the smarter financial choice?"
        );
        
        scenario.choices = new string[] 
        { 
            "Buy - building equity is always better",
            "Rent - short stay means buying costs more", 
            "Buy - interest rates are low",
            "Rent - homeownership is never worth the money"
        };
        scenario.correctChoiceIndex = yearsPlanning <= 2 ? 1 : 0; // Rent for short term
        
        return scenario;
    }
    private FinancialScenario GenerateSavingsScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        
        float goalAmount = UnityEngine.Random.Range(1000f, 5000f);
        float monthlySavings = UnityEngine.Random.Range(100f, 500f);
        
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.Savings,
            $"{clientName} wants to save ${goalAmount:F0} for a vacation. They can save ${monthlySavings:F0} per month.",
            $"How many months until {clientName} reaches their goal?"
        );
        scenario.units = "Months";

        scenario.data["goal"] = goalAmount;
        scenario.data["monthly"] = monthlySavings;
        
        scenario.correctAnswerFloat = Mathf.Ceil(goalAmount / monthlySavings);
        scenario.tolerance = 0.5f;
        
        return scenario;
    }
    
    private FinancialScenario GenerateComparisonScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];

        string[] items = { "a laptop", "a phone", "a bike", "a TV", "headphones" };
        string item = items[UnityEngine.Random.Range(0, items.Length)];
        float priceA = UnityEngine.Random.Range(200f, 800f);
        float priceB = priceA * UnityEngine.Random.Range(0.85f, 1.15f);
        int qualityA = UnityEngine.Random.Range(6, 10);
        int qualityB = UnityEngine.Random.Range(6, 10);
        
        FinancialScenario scenario = new(
            ScenarioType.Comparison,
            $"{clientName} wants to buy {item}. At Store A, it costs ${priceA:F0} and has a quality rating of {qualityA} out of 10. At Store B, it costs ${priceB:F0} with a quality rating of {qualityB} out of 10.",
            "Which store gives the better value for the money?"
        );
        
        scenario.data["priceA"] = priceA;
        scenario.data["priceB"] = priceB;
        scenario.data["qualityA"] = qualityA;
        scenario.data["qualityB"] = qualityB;
        
        float valueA = priceA / qualityA;
        float valueB = priceB / qualityB;
        
        scenario.choices = new string[] { "Store A", "Store B" };
        scenario.correctChoiceIndex = valueA < valueB ? 0 : 1;
        
        return scenario;
    }
    
    private FinancialScenario GenerateDebtScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        
        float debt = UnityEngine.Random.Range(1000f, 5000f);
        float interestRate = UnityEngine.Random.Range(5f, 20f);
        float monthlyPayment = UnityEngine.Random.Range(100f, 500f);
        
        // Simplified interest calculation (not compound for easier math)
        float monthlyInterest = (debt * (interestRate / 100f)) / 12f;
        float principalPayment = monthlyPayment - monthlyInterest;
        
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.DebtPayoff,
            $"{clientName} owes ${debt:F0} on a loan with an annual interest rate of {interestRate:F1}%. Each month, they make a payment of ${monthlyPayment:F0}.",
            $"In the first month, how much of that payment goes toward paying down the original amount {clientName} borrowed?"
        );
        scenario.units = "Dollars";
        scenario.data["debt"] = debt;
        scenario.data["rate"] = interestRate;
        scenario.data["payment"] = monthlyPayment;
        
        scenario.correctAnswerFloat = principalPayment;
        scenario.tolerance = 3f;
        
        return scenario;
    }
    
    private FinancialScenario GenerateEmergencyFundScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        
        float monthlyExpenses = UnityEngine.Random.Range(1500f, 4000f);
        int targetMonths = UnityEngine.Random.Range(3, 6);
        
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.EmergencyFund,
            $"{clientName} spends about ${monthlyExpenses:F0} each month on living expenses. {clientName} wants to have enough in emergency funds to cover {targetMonths} months of expenses.",
            $"About how much money should {clientName} aim to have in an emergency fund?"
        );
        scenario.units = "Dollars";
        scenario.data["expenses"] = monthlyExpenses;
        scenario.data["months"] = targetMonths;
        
        scenario.correctAnswerFloat = monthlyExpenses * targetMonths;
        scenario.tolerance = 10f;
        
        return scenario;
    }
    
    public void SubmitAnswer(float answer)
    {
        if (currentScenario == null || !shiftActive) return;
        
        bool correct = Mathf.Abs(answer - currentScenario.correctAnswerFloat) <= currentScenario.tolerance;
        UIWorkManager.Instance.ShowFeedback(correct, currentScenario);
        CompleteScenario(correct);
    }
    
    public void SubmitChoice(int choiceIndex)
    {
        if (currentScenario == null || !shiftActive) return;
        
        bool correct = choiceIndex == currentScenario.correctChoiceIndex;
        UIWorkManager.Instance.ShowFeedback(correct, currentScenario);
        CompleteScenario(correct);
    }
    // completes the scenario. accepts "correct" bool parameter
    public bool inReviewTime = false;
    private void CompleteScenario(bool correct)
    {
        completedScenarioCount++;
        
        if (correct)
        {
            // Calculate payment based on time remaining and difficulty
            float timeBonus = Mathf.Clamp01(bonusTimer / bonusTimePerScenario);
            float basePay = 20f + (difficultyLevel * 10f);
            float payment = basePay + ((timeBonus*20) + basePay * 0.5f);
            totalEarned += payment;
        }
        
        UIWorkManager.Instance.UpdateWorkStats(completedScenarioCount, totalScenarios);
        inReviewTime = true;
        // Small delay before next scenario
        Invoke(nameof(NextScenario), 4f);
    }
    // cancels the shift, making time elapse, but no pay
    public void CancelShift()
    {
        if (shiftActive)
            GameTime.Instance.ElapseTime(480, true);
        
        shiftActive = false;
        countdown = 0f;
        totalEarned = 0f;
        currentScenario = null;

        OnWorkEnded?.Invoke();
    }
    // ends the shift
    public void EndShift()
    {
        shiftActive = false;
        FinancialSpending.Instance.Earn(totalEarned);
        
        // increase difficulty every 3 successful shifts
        if (completedScenarioCount >= totalScenarios)
            difficultyLevel = Mathf.Min(difficultyLevel + 1, 5);
        
        totalEarned = 0f;
        
        GameTime.Instance.ElapseTime(480, true);
        OnWorkEnded?.Invoke();
    }
}