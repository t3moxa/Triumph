using UnityEngine;

public class City : Building
{
    public override void ShowBuildingInfoPanel()
    {
        PlayerController.CityInfoPanel.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = Name;
        PlayerController.CityInfoPanel.SetActive(true);
    }

    public override void OnConquer(KingdomData Conqueror)
    {
        Owner = Conqueror;
        LiftSiege();
        ChangeBanner();
    }
    public override void HideAllPanels()
    {
        PlayerController.CityInfoPanel.SetActive(false);
    }
}