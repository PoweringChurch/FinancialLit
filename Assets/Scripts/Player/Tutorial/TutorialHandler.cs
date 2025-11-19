using UnityEngine;
public class TutorialHandler : MonoBehaviour
{
    public static TutorialHandler Instance;

    void Awake()
    {
        Instance = this;
    }
    public void AskTutorial()
    {
        UIHandler.Instance.PopupManager.PopupYN("Tutorial", 
        "Would you like to learn how to play?", 
        () => StartTutorial(), 
        null, 
        "Teach me!", 
        "Skip");
    }
    public void StartTutorial()
    {
        // Welcome
        UIHandler.Instance.PopupManager.PopupInfo("Welcome!", 
            "Let's learn how to take care of your new pet!", 
            "Next"
        );
    }

}