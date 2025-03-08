using UnityEngine;

public class Menu_Main : MonoBehaviour
{
    public GameObject profileActive;
    public GameObject profileDisable;
    public GameObject profilePanel;
    public GameObject missionActive;
    public GameObject missionDisable;
    public GameObject missionPanel;
    public GameObject inventoryActive;
    public GameObject inventoryDisable;
    public GameObject inventoryPanel;
    public GameObject homeActive;
    public GameObject homeDisable;
    public GameObject homePanel;
    private GameObject currentActive;
    private GameObject currentDisable;
    private GameObject currentPanel;
    private bool isProfileActive;
    private bool isMissionActive;
    private bool isHomeActive;
    private bool isInventoryActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnProfileClick()
    {
        ToggleMenu(profileActive, profileDisable, profilePanel);
    }

    public void OnMissionClick()
    {
        ToggleMenu(missionActive, missionDisable, missionPanel);
    }

    public void OnHomeClick()
    {
        ToggleMenu(homeActive, homeDisable, homePanel);
    }

    public void OnInventoryClick()
    {
        ToggleMenu(inventoryActive, inventoryDisable, inventoryPanel);
    }

    private void ToggleMenu(GameObject active, GameObject disable, GameObject panel)
    {
        if (currentActive == active)
            return;

        if (currentActive != null)
        {
            currentActive.SetActive(false);
            currentDisable.SetActive(true);
            currentPanel.SetActive(false);
        }

        active.SetActive(true);
        disable.SetActive(false);
        panel.SetActive(true);

        currentActive = active;
        currentDisable = disable;
        currentPanel = panel;
    }

}
