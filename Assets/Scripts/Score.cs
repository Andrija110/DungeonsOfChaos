using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;

public class Score : MonoBehaviour
{
    private TableUI table;

    // Start is called before the first frame update
    void Start()
    {
        table = GameObject.Find("ScoreTable").GetComponent<TableUI>();

        var go = GameObject.FindGameObjectWithTag("DBHelper");
        if (go != null)
        {
            var dbhelper = go.GetComponent<DBHelper>();
            if (dbhelper != null)
            {
                var scores = dbhelper.GetAllScores();
                var nScores = scores.Count; 
                table.Rows = nScores + 1;
                for (int i = 0; i < nScores; i++)
                {
                    table.GetCell(i + 1, 0).text = scores[i].Name;
                    table.GetCell(i + 1, 1).text = scores[i].HighScore.ToString();
                }

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBackButton()
    {
        Game.Instance.GoToScene("Menu");
    }
}
