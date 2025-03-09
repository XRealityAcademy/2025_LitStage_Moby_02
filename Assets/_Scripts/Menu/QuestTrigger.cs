using UnityEngine;
using Meta.XR.MRUtilityKit;

public class QuestTrigger : MonoBehaviour
{
    public QuestUiToggle questUiToggle; // Reference to the QuestManager script
    private int currentQuestIndex = -1; // Tracks the current quest to toggle

    bool aButtonPressed = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(OVRInput.GetDown(OVRInput.RawButton.B)) 
       {    
            IncrementQuestIndex();
       }
        
    }
    void IncrementQuestIndex()
    {
        currentQuestIndex++;
        if (currentQuestIndex >= questUiToggle.quests.Length)
        {
            currentQuestIndex = questUiToggle.quests.Length; // Cap index at quests length
            return;
        }

        questUiToggle.ToggleQuestCompletion(currentQuestIndex); // Toggle current quest completion
    }
}
