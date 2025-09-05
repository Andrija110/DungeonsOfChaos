using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Name : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayButton()
    {
        var nameText = GameObject.Find("NameText").GetComponent<TMP_InputField>();
        if (!string.IsNullOrEmpty(nameText.text))
        {
            PlayerStats.Instance.PlayerName = nameText.text;
            PlayerStats.Instance.PlayerDBId = 0;
            Game.Instance.GoToScene("MainArea");
        }
        
    }

    public void OnBackButton()
    {
        Game.Instance.GoToScene("Menu");
    }
}
