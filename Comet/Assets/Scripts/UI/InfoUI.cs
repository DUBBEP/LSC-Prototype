using UnityEngine;
using TMPro;

public class InfoUI : MonoBehaviour
{
    [SerializeField]
    private GameObject useCountDescription;
    [SerializeField]
    private GameObject powerDescription;
    [SerializeField]
    private GameObject delayDescription;
    [SerializeField]
    private GameObject cooldownDescription;
    [SerializeField]
    private GameObject cardTypeDescription;
    [SerializeField]
    private GameObject cardRarityDescritpion;

    [SerializeField]
    private GameObject cardInfoScreen;
    [SerializeField]
    private GameObject generalInfoScreen;

    public void OnSetElementDescription(GameObject description)
    {
        useCountDescription.SetActive(false);
        powerDescription.SetActive(false);
        delayDescription.SetActive(false);
        cooldownDescription.SetActive(false);
        cardTypeDescription.SetActive(false);
        cardRarityDescritpion.SetActive(false);

        description.SetActive(true);
    }

    public void ToggleInfoScreen()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void SwitchInfoScreens()
    {
        if (cardInfoScreen.activeSelf)
        {
            cardInfoScreen.SetActive(false);
            generalInfoScreen.SetActive(true);
        }
        else if (generalInfoScreen.activeSelf)
        {
            cardInfoScreen.SetActive(true);
            generalInfoScreen.SetActive(false);
        }
    }
}
