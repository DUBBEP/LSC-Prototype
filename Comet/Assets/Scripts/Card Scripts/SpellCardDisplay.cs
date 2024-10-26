using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellCardDisplay : MonoBehaviour
{
    public SpellCard spellCard;

    [Header("DisplayComponents")]
    public TextMeshProUGUI spellNameText;
    public TextMeshProUGUI cardDelayText;
    public TextMeshProUGUI coolDownText;
    public TextMeshProUGUI useCountText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI cardTypeText;
    public TextMeshProUGUI cardRarityText;
    public TextMeshProUGUI descriptionText;

    void Start()
    {
        spellNameText.text = spellCard.spellName;
        cardDelayText.text = spellCard.castDelay.ToString();
        coolDownText.text = spellCard.coolDown.ToString();
        useCountText.text = spellCard.numberOfUses.ToString();
        powerText.text = spellCard.power.ToString();
        cardTypeText.text = spellCard.cardType.ToString()[0].ToString().ToUpper();
        cardRarityText.text = spellCard.cardRarity.ToString()[0].ToString().ToUpper();
        descriptionText.text = spellCard.description.ToString();
    }
}
