using UnityEngine;
using Meta.XR.MRUtilityKit;

public class QuestTrigger : MonoBehaviour
{
    public QuestUiToggle questUiToggle; // Reference to the QuestManager script
    private int currentQuestIndex = 0; // Tracks the current quest to toggle
    bool aButtonPressed = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(OVRInput.GetDown(OVRInput.Button.Two)) 
       {    
            ToggleNextQuest();
       }
        
    }
    void ToggleNextQuest()
    {

        if (currentQuestIndex >= questUiToggle.quests.Length) return; // Stop if all quests are toggled

        questUiToggle.ToggleQuestCompletion(currentQuestIndex); // Toggle current quest completion
        currentQuestIndex++; // Move to the next quest
    }

}
