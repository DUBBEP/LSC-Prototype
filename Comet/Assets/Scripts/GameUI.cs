using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public GameObject spellHandUI;
    public GameObject submitCast;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleHandUI()
    {
        spellHandUI.SetActive(!spellHandUI.activeSelf);
    }

    public void SetConfirmCastButton(bool toggle)
    {
        submitCast.SetActive(toggle);
    }


}
