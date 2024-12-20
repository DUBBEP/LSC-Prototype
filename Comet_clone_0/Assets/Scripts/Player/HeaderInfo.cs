using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;


public class HeaderInfo : MonoBehaviourPun
{
    [SerializeField]
    private Color fullHealthColor;
    [SerializeField]
    private Color lowHealthColor;

    public TextMeshProUGUI nameText;
    public Image bar;
    private float maxValue;

    public void Initialize (string text, int maxVal)
    {
        if (text.Length > 8)
            text = text.Substring (0, 8);
        nameText.text = text;
        maxValue = maxVal;
        bar.fillAmount = 1.0f;
    }

    [PunRPC]
    void UpdateHealthBar (int value)
    {
        bar.fillAmount = (float)value / maxValue;

        if (bar.fillAmount > 0.4f)
            bar.color = fullHealthColor;
        else
            bar.color = lowHealthColor;
    }
}
