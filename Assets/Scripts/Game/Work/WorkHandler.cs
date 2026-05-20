using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;

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
    RentVsBuy,
    Taxes,
    NetWorth,
    CompoundInterest,
    InvestingRisk,
    Retirement,
    SubscriptionAudit,
    PricePerUnit,
    LifestyleInflation,
    CreditScoreImpact,
    AssetVsLiability
}

[Serializable]
public class FinancialScenario
{
    public ScenarioType type;
    public string description;
    public string question;
    public float correctAnswerFloat;
    public float tolerance; // how close they need to be to the answer in input questions
    public string[] choices; // for multiple choice scenarios
    public int correctChoiceIndex; // for multiple choice
    public string units;
    public bool multiChoice;

    public string hintText;
    public FinancialScenario(ScenarioType type, string description, string question)
    {
        this.type = type;
        this.description = description;
        this.question = question;
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
    private Dictionary<ScenarioType, Func<FinancialScenario>> _scenarioGenerators;
    public void NextScenario()
    {
        if (completedScenarioCount > 0)
            UISFXPlayer.Instance.Play(nextScenario);
        
        // check if shift is complete
        if (completedScenarioCount >= totalScenarios)
        {
            UIWorkManager.Instance.EndShift();
            return;
        }
        
        // generate new scenario
        currentScenario = GenerateScenario();
        bonusTimer = bonusTimePerScenario;
        inReviewTime = false;
        UIWorkManager.Instance.DisplayScenario(currentScenario);
        UIWorkManager.Instance.UpdateTimer(bonusTimer);
    }
    // generates & returns a scenario
    private FinancialScenario GenerateScenario()
    {
        bool useChoice = UnityEngine.Random.value > 0.5f; // 50/50 split
        
        ScenarioType type = (ScenarioType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ScenarioType)).Length);
        return type switch
        {
            ScenarioType.Budgeting => GenerateBudgetingScenario(),
            ScenarioType.Savings => GenerateSavingsScenario(),
            ScenarioType.Comparison => GenerateComparisonScenario(),
            ScenarioType.DebtPayoff => GenerateDebtScenario(),
            ScenarioType.EmergencyFund => GenerateEmergencyFundScenario(),
            ScenarioType.NeedsVsWants => GenerateNeedsVsWantsScenario(),
            ScenarioType.CreditCardChoice => GenerateCreditCardScenario(),
            ScenarioType.Insurance => GenerateInsuranceScenario(),
            ScenarioType.OpportunityCost => GenerateOpportunityCostScenario(),
            ScenarioType.Inflation => GenerateInflationScenario(),
            ScenarioType.RentVsBuy => GenerateRentVsBuyScenario(),

            ScenarioType.Taxes => GenerateTaxesScenario(),
            ScenarioType.NetWorth => GenerateNetWorthScenario(),
            ScenarioType.CompoundInterest => GenerateCompoundInterestScenario(),
            ScenarioType.InvestingRisk => GenerateInvestingRiskScenario(),
            ScenarioType.Retirement => GenerateRetirementMatchScenario(),
            ScenarioType.SubscriptionAudit => GenerateSubscriptionAuditScenario(),
            ScenarioType.PricePerUnit => GeneratePricePerUnitScenario(),
            ScenarioType.LifestyleInflation => GenerateLifestyleInflationScenario(),
            ScenarioType.CreditScoreImpact => GenerateCreditScoreImpactScenario(),
            ScenarioType.AssetVsLiability => GenerateAssetVsLiabilityScenario(),

            _ => GenerateBudgetingScenario(),
        };
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
    
    private FinancialScenario GenerateTaxesScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        int salary = UnityEngine.Random.Range(30000, 90000);
        float taxRate = UnityEngine.Random.Range(15f, 30f);
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.Taxes,
            $"{clientName} earns ${salary} per year and pays {taxRate:f2}% in taxes.",
            "What is their annual take-home pay?"
        );
        scenario.units = "Dollars";
        scenario.correctAnswerFloat = salary * (1 - taxRate / 100f);
        scenario.tolerance = 2f;
        scenario.hintText = "Annual take-home pay is calculated as your salary after taxes.";
        return scenario;
    }
    private FinancialScenario GenerateNetWorthScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        int assets = UnityEngine.Random.Range(20000, 100000);
        int debt = UnityEngine.Random.Range(5000, 50000);
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.NetWorth,
            $"{clientName} owns assets worth ${assets} and owes ${debt} in debt.",
            "What is their net worth?"
        );
        scenario.units = "Dollars";
        scenario.correctAnswerFloat = assets - debt;
        scenario.tolerance = 2f;
        scenario.hintText = "Net worth is calculated as your assets minus debts.";
        return scenario;
    }
    private FinancialScenario GenerateCompoundInterestScenario()
    {
        int principal = UnityEngine.Random.Range(1000, 5000);
        float rate = UnityEngine.Random.Range(4f, 10f);
        int years = UnityEngine.Random.Range(2, 6);
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.CompoundInterest,
            $"${principal} is invested at {rate:F2}% annual compound interest for {years} years.",
            "What is the approximate final value?"
        );
        scenario.units = "Dollars";
        scenario.correctAnswerFloat = principal * Mathf.Pow(1 + rate / 100f, years);
        scenario.tolerance = 3f;
        scenario.hintText = "Compound interest means you earn interest on your interest each year. Use the formula: Principal × (1 + rate)^years.";
        return scenario;
    }
    private FinancialScenario GenerateInvestingRiskScenario()
    {
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.InvestingRisk,
            "An investment has higher potential returns but also larger price swings.",
            "What characteristic does this describe?"
        );
        scenario.choices = new string[]
        {
            "Higher risk",
            "Lower volatility",
            "Guaranteed return",
            "Liquidity"
        };
        scenario.correctChoiceIndex = 0;
        scenario.hintText = "Larger price swings and uncertainty mean greater risk. Higher returns come with higher risk.";
        return scenario;
    }
    private FinancialScenario GenerateRetirementMatchScenario()
    {
        int salary = UnityEngine.Random.Range(40000, 80000);
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.Retirement,
            $"An employer matches 5% of salary. Salary is ${salary}.",
            "How much free money is the match per year?"
        );
        scenario.units = "Dollars";
        scenario.correctAnswerFloat = salary * (5 / 100f);
        scenario.hintText = "Employer match is free money added to your retirement. Calculate 5% of the annual salary.";
        scenario.tolerance = 3f;
        return scenario;
    }
    private FinancialScenario GenerateSubscriptionAuditScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        int monthlyTotal = UnityEngine.Random.Range(40, 120);
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.SubscriptionAudit,
            $"{clientName} spends ${monthlyTotal} per month on subscriptions.",
            "How much do they spend per year?"
        );
        scenario.units = "Dollars";
        scenario.correctAnswerFloat = monthlyTotal * 12f;
        scenario.tolerance = 5f;
        scenario.hintText = "Small monthly costs add up over a year. Multiply the monthly amount by 12 months.";
        return scenario;
    }
    private FinancialScenario GeneratePricePerUnitScenario()
    {
        float price = UnityEngine.Random.Range(4f, 12f);
        int ounces = UnityEngine.Random.Range(10, 30);
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.PricePerUnit,
            $"A package costs ${price:F2} and contains {ounces} ounces.",
            "What is the cost per ounce?"
        );
        scenario.units = "Dollars per ounce";
        scenario.correctAnswerFloat = price / ounces;
        scenario.tolerance = 0.05f;
        scenario.hintText = "Price per unit helps compare products of different sizes. Divide total price by the number of ounces.";
        return scenario;
    }
    //
    private FinancialScenario GenerateLifestyleInflationScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.LifestyleInflation,
            $"After getting a raise, {clientName} increases their spendings instead of saving more.",
            "What is this behavior called?"
        );
        scenario.choices = new string[]
        {
            "Lifestyle inflation",
            "Diversification",
            "Debt consolidation",
            "Compound growth"
        };
        scenario.correctChoiceIndex = 0;
        scenario.hintText = "When income increases but savings don't, spending habits have inflated. This is called lifestyle inflation.";
        return scenario;
    }
    private FinancialScenario GenerateCreditScoreImpactScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.CreditScoreImpact,
            $"{clientName} has recently gotten a credit card and is looking for advice.",
            "Which action would most likely LOWER a credit score?"
        );
        scenario.choices = new string[]
        {
            "Missing a payment",
            "Paying off a loan",
            "Keeping credit utilization low",
            "Checking your own credit report"
        };
        scenario.correctChoiceIndex = 0;
        scenario.hintText = "Payment history is the biggest factor in credit scores. Missing payments damages your credit score significantly.";
        return scenario;
    }
    private FinancialScenario GenerateAssetVsLiabilityScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.AssetVsLiability,
            $"{clientName} is concerned if they have any potential financial liabilities.",
            "Which item is typically considered a liability?"
        );
        string[] liabilities = new string[]
        {
            "Car loan",
            "Credit card debt",
            "Student loan",
            "Mortgage balance",
            "Personal loan",
            "Medical debt",
            "Payday loan",
            "Auto lease obligation",
            "Home equity loan",
            "Business loan",
            "Unpaid tax bill",
            "Lines of credit balance",
            "Installment loan",
            "Boat loan",
            "Furniture financing balance"
        };
        string[] nonliabilities = new string[]
        {
            "Savings account",
            "Checking account balance",
            "Cash on hand",
            "Emergency fund",
            "Dividend-paying stock",
            "Index fund",
            "Mutual fund investment",
            "Certificate of deposit",
            "Treasury bonds",
            "Corporate bonds owned",
            "Retirement account (401k)",
            "Roth IRA",
            "Traditional IRA",
            "Health Savings Account balance",
            "Rental property producing income",
            "Primary home equity",
            "Paid-off vehicle",
            "Collectible artwork",
            "Precious metals",
            "Cryptocurrency holdings",
            "Business ownership stake",
            "Intellectual property royalties",
            "Side business inventory",
            "Equipment owned outright",
            "Land owned",
            "Vacation property owned",
            "Trust fund balance",
            "Education savings account",
            "529 college fund",
            "High-yield savings account",
            "Brokerage account",
            "Money market account",
            "Peer-to-peer investment",
            "Crowdfunded real estate investment",
            "Dividend reinvestment plan",
            "Pension value",
            "Annuity value",
            "Stock options (vested)",
            "Treasury bills",
            "Commodities investment",
            "Farmland ownership",
            "Royalties from a book",
            "Paid-in-full motorcycle",
            "Valuable jewelry collection"
        };
        string correct = liabilities[UnityEngine.Random.Range(0, liabilities.Length)];
        List<string> wrong = nonliabilities
            .OrderBy(x => UnityEngine.Random.value)
            .Take(3)
            .ToList();
        scenario.choices = new string[]
        {
            correct,
            wrong[0],
            wrong[1],
            wrong[2]
        };
        scenario.hintText = "A liability is money you owe to others. Assets are things you own that have value.";
        scenario.correctChoiceIndex = 0;
        return scenario;
    }
    private FinancialScenario GenerateBudgetingScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        
        int income = UnityEngine.Random.Range(2000, 5000);
        int rent = (int)(income * UnityEngine.Random.Range(0.25f, 0.35f));
        int groceries = UnityEngine.Random.Range(300, 600);
        int utilities = UnityEngine.Random.Range(100, 250);
        int transport = UnityEngine.Random.Range(150, 400);
        
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.Budgeting,
            $"{clientName} earns ${income} each month. They must pay ${rent} for rent, spend about ${groceries} on groceries, use ${utilities} for utilities, and pay ${transport} for transportation.",
            "After covering these costs, how much money do they have left?"
        );
        scenario.units = "Dollars";
        scenario.correctAnswerFloat = income - (rent + groceries + utilities + transport);
        scenario.hintText = "Subtract all expenses from total income to find what's left. This is your remaining budget or surplus.";
        scenario.tolerance = 2f;
        return scenario;
    }
    private FinancialScenario GenerateNeedsVsWantsScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        string[] needs = new string[]
        {
            "Internet", "Work boots", "Car insurance", "Groceries", "Clothing", "Rent payments", "Medical bills"
        };
        string[] wants = new string[]
        {
            "Designer sneakers", "Luxury slippers", "Streaming subscription", "Gaming console", "Gourmet meal kit",
            "Restaurant dining", "Concert tickets", "New smartphone", "Car Smartwatch"
        };
        string[] itemSet = new string[] {
            needs[UnityEngine.Random.Range(0, needs.Length-1)],
            wants[UnityEngine.Random.Range(0, needs.Length-1)],
            wants[UnityEngine.Random.Range(0, needs.Length-1)],
            wants[UnityEngine.Random.Range(0, needs.Length-1)]
        };
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.NeedsVsWants,
            $"{clientName} has limited budget this month and must prioritize how they spend their money.",
            "Which expense is a need, and not a want?"
        );
        scenario.choices = itemSet;
        scenario.correctChoiceIndex = 0;
        scenario.hintText = "Needs are essential for survival and basic functioning. Wants are nice to have but not required.";
        return scenario;
    }
    private FinancialScenario GenerateCreditCardScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.CreditCardChoice,
            $"{clientName} is choosing a credit card. They plan to pay off their balance monthly.",
            "Which matters least if they pay in full each month?"
        );
        scenario.choices = new string[] 
        { 
            "Interest rate", 
            "Annual fee amount",
            "Rewards percentage",
            "Credit limit"
        };
        scenario.correctChoiceIndex = 0;
        scenario.hintText = "Interest only applies to balances carried over month-to-month. Paying in full means no interest charged.";
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
        float planACost = (lowPremium * 12) + lowDeductible; // assumes one incident
        float planBCost = (highPremium * 12) + highDeductible;
        scenario.choices = new string[] 
        { 
            $"Plan A (${lowPremium:F2}/mo, ${lowDeductible:F2} deductible)",
            $"Plan B (${highPremium:F2}/mo, ${highDeductible:F2} deductible)",
            "Both cost the same"
        };
        scenario.hintText = "For low healthcare usage, compare total premiums paid annually plus one deductible. Higher deductible plans have lower monthly costs.";
        scenario.correctChoiceIndex = planBCost < planACost ? 1 : 0;
        return scenario;
    }
    private FinancialScenario GenerateOpportunityCostScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        int amount = UnityEngine.Random.Range(1000, 3000);
        float investmentReturn = UnityEngine.Random.Range(5f, 10f);
        string[][] options = new string[][]
        {
            new string[] { $"Invest ${amount} (expecting {investmentReturn}% annual return)", $"Buy a TV for ${amount}", $"Take a vacation for ${amount}", "Save it in a 0% checking account" },
            new string[] { "Work overtime for extra $500", $"Spend $500 on a course that could increase salary", "Spend $500 on entertainment", "Save $500 under the mattress" }
        };
        string[] optionSet = options[UnityEngine.Random.Range(0, options.Length)];
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.OpportunityCost,
            $"{clientName} wants to make the most of their money long-term.",
            "Which choice has the best opportunity cost?"
        );
        scenario.choices = optionSet;
        scenario.correctChoiceIndex = optionSet == options[0] ? 0 : 1; // investment or course
        scenario.hintText = "Opportunity cost is what you give up by choosing one option over another. Choose investments that grow your wealth or skills long-term.";
        return scenario;
    }
    private FinancialScenario GenerateInflationScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        float inflationRate = UnityEngine.Random.Range(2f, 5f);
        float interestRate = UnityEngine.Random.Range(0f, 6f);
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.Inflation,
            $"Inflation is {inflationRate:F2}% this year. {clientName} has $10,000 in savings earning {interestRate:F2}% interest.",
            "What happens to their purchasing power?"
        );
        scenario.choices = new string[] 
        { 
            "It increases",
            "It decreases", 
            "It stays the same"
        };
        if (interestRate > inflationRate)
            scenario.correctChoiceIndex = 0; // interest beats inflation
        else if (interestRate < inflationRate)
            scenario.correctChoiceIndex = 1; // inflation beats interest
        else
            scenario.correctChoiceIndex = 2; // same equal rates
        scenario.hintText = "Compare the interest rate to inflation rate. If interest beats inflation, purchasing power grows; otherwise it shrinks.";
        return scenario;
    }
    private FinancialScenario GenerateRentVsBuyScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        
        int rent = UnityEngine.Random.Range(1200, 2000);
        int mortgage = (int)(rent * UnityEngine.Random.Range(1.1f, 1.4f));
        int yearsPlanning = UnityEngine.Random.Range(1, 8);
        
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.RentVsBuy,
            $"{clientName} is relocating for a job that will last {yearsPlanning} years. Renting would cost ${rent} per month, while buying a home with a mortgage would cost ${mortgage} per month but would build equity over time.",
            "Considering the length of the job, which option is the better financial choice?"
        );
        scenario.choices = new string[] 
        { 
            "Buying, because building equity is always better",
            "Renting, because short stay means buying costs more", 
            "Buying, because interest rates are low",
            "Renting, because home ownership is never worth the costs"
        };
        scenario.correctChoiceIndex = yearsPlanning <= 2 ? 1 : 0; // rent for short term
        scenario.hintText = "Buying has high upfront costs like closing fees and realtor commissions. Short-term stays typically favor renting to avoid these costs.";
        return scenario;
    }
    private FinancialScenario GenerateSavingsScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        int goalAmount = UnityEngine.Random.Range(1000, 5000);
        int monthlySavings = UnityEngine.Random.Range(100, 500);
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.Savings,
            $"{clientName} wants to save ${goalAmount} for a vacation. They can save ${monthlySavings} per month.",
            $"How many months until {clientName} reaches their goal?"
        );
        scenario.units = "Months";
        scenario.correctAnswerFloat = Mathf.Ceil((float)goalAmount / monthlySavings);
        scenario.tolerance = 0.5f;
        scenario.hintText = "Divide the total goal amount by monthly savings to find the number of months needed. Round up to the nearest whole month.";
        return scenario;
    }
    private FinancialScenario GenerateComparisonScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        string[] items = { "a laptop", "a phone", "a bike", "a TV", "headphones" };
        string item = items[UnityEngine.Random.Range(0, items.Length)];
        int priceA = UnityEngine.Random.Range(200, 800);
        int priceB = (int)(priceA * UnityEngine.Random.Range(0.85f, 1.15f));
        int qualityA = UnityEngine.Random.Range(6, 10);
        int qualityB = UnityEngine.Random.Range(6, 10);
        FinancialScenario scenario = new(
            ScenarioType.Comparison,
            $"{clientName} wants to buy {item}. At Store A, it costs ${priceA} and has a quality rating of {qualityA} out of 10. At Store B, it costs ${priceB} with a quality rating of {qualityB} out of 10.",
            "Which store gives the better value for the money?"
        );
        float valueA = priceA / qualityA;
        float valueB = priceB / qualityB;
        scenario.choices = new string[] { "Store A", "Store B" };
        scenario.correctChoiceIndex = valueA < valueB ? 0 : 1;
        scenario.hintText = "Calculate price per quality point by dividing price by quality rating. Lower cost per quality point means better value.";
        return scenario;
    }
    private FinancialScenario GenerateDebtScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        int debt = UnityEngine.Random.Range(1000, 5000);
        int monthlyPayment = UnityEngine.Random.Range(100, 500);
        float interestRate = UnityEngine.Random.Range(5f, 20f);
        // simplified interest calculation
        float monthlyInterest = debt * (interestRate / 100f) / 12f;
        float principalPayment = monthlyPayment - monthlyInterest;
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.DebtPayoff,
            $"{clientName} owes ${debt} on a loan with a simple annual interest rate of {interestRate:F2}%. Each month, they make a payment of ${monthlyPayment}.",
            $"In the first month, how much of that payment goes toward paying down the original amount {clientName} borrowed?"
        );
        scenario.units = "Dollars";
        scenario.correctAnswerFloat = principalPayment;
        scenario.tolerance = 3f;
        scenario.hintText = "Calculate monthly interest charged, then subtract it from the payment. What remains goes toward reducing the principal balance.";
        return scenario;
    }
    private FinancialScenario GenerateEmergencyFundScenario()
    {
        string clientName = names[UnityEngine.Random.Range(0, names.Length)];
        int monthlyExpenses = UnityEngine.Random.Range(1500, 4000);
        int targetMonths = UnityEngine.Random.Range(3, 6);
        FinancialScenario scenario = new FinancialScenario(
            ScenarioType.EmergencyFund,
            $"{clientName} spends about ${monthlyExpenses} each month on living expenses. {clientName} wants to have enough in emergency funds to cover {targetMonths} months of expenses.",
            $"About how much money should {clientName} aim to have in an emergency fund?"
        );
        scenario.units = "Dollars";
        scenario.correctAnswerFloat = monthlyExpenses * targetMonths;
        scenario.tolerance = 3f;
        scenario.hintText = "Emergency funds should cover multiple months of expenses. Multiply monthly expenses by the target number of months.";
        return scenario;
    }
    // submits the answer
    public void SubmitAnswer(float answer)
    {
        if (currentScenario == null || !shiftActive) return;
        
        bool correct = Mathf.Abs(answer - currentScenario.correctAnswerFloat) <= currentScenario.tolerance;
        UIWorkManager.Instance.ShowFeedback(correct, currentScenario);
        CompleteScenario(correct);
    }
    // submits a choice
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
            // calculate payment based on time remaining and difficulty
            float timeBonus = Mathf.Clamp01(bonusTimer / bonusTimePerScenario);
            float basePay = 20f + (difficultyLevel * 10f);
            float payment = basePay + ((timeBonus*20) + basePay * 0.5f);
            totalEarned += payment;
        }
        
        UIWorkManager.Instance.UpdateWorkStats(completedScenarioCount, totalScenarios);
        inReviewTime = true;
        // small delay before next scenario
        Invoke(nameof(NextScenario), 4f);
    }
    // cancels the shift, making time elapse, but no pay
    public void CancelShift()
    {
        if (shiftActive)
            GameTime.Instance.ElapseTime(480, true);
        
        shiftActive = false;
        totalEarned = 0f;
        currentScenario = null;

        OnWorkEnded?.Invoke();
    }
    // ends the shift
    public void EndShift()
    {
        shiftActive = false;
        FinancialSpending.Instance.Earn(totalEarned, "Work");
        
        // increase difficulty every 3 successful shifts
        if (completedScenarioCount >= totalScenarios)
            difficultyLevel = Mathf.Min(difficultyLevel + 1, 5);
        
        totalEarned = 0f;
        
        GameTime.Instance.ElapseTime(480, true);
        OnWorkEnded?.Invoke();
    }
}