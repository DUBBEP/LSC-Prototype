using UnityEngine.UI;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    GameObject spellHandUI;


    // instance
    public static GameUI instance;

    private void Awake()
    {
        instance = this;
    }


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
}
