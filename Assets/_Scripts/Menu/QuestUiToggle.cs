using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro for UI text rendering // Import Unity's UI system

public class QuestUiToggle : MonoBehaviour
{
    [System.Serializable] // Allows Quest class to be serialized in Unity's Inspector
    public class Quest
    {
        public string questName; // The name of the quest
        public bool isCompleted; // Tracks whether the quest is completed 
        public TMP_Text questText;     
        public Image checkmark; // UI image component for displaying the checkmark
    }

    public Quest[] quests = new Quest[7]; // Array to store 7 quests

    void Start()
    {
        UpdateUI(); // Call to initialize the UI at the start
    }

    public void ToggleQuestCompletion(int index) // Function to toggle quest completion
    {
        if (index < 0 || index >= quests.Length) return; // Ensure index is within bounds

        quests[index].isCompleted = !quests[index].isCompleted; // Toggle quest completion status
        UpdateUI(); // Update the UI after toggling
    }

    void UpdateUI() // Function to update the UI based on quest status
    {
        foreach (var quest in quests) // Loop through all quests
        {
            if (quest.isCompleted) // If quest is completed
            {
                Color textColor = quest.questText.color; // Get current text color
                textColor.a = 0.5f; // Reduce opacity to 50%
                quest.questText.color = textColor; // Apply new color
                quest.checkmark.gameObject.SetActive(true); // Show the checkmark
            }
            else // If quest is not completed
            {
                Color textColor = quest.questText.color; // Get current text color
                textColor.a = 1f; // Set opacity to 100%
                quest.questText.color = textColor; // Apply new color
                quest.checkmark.gameObject.SetActive(false); // Hide the checkmark
            }
        }
    }
}
