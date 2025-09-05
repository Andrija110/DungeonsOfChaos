using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStats : MonoBehaviour
{

    private PlayerStats ps;
    private TMP_Text txtSpeed;
    private TMP_Text txtDamage;
    private TMP_Text txtFireRate;
    private TMP_Text txtRange;
    private TMP_Text txtBulletSize;
    private TMP_Text txtBulletSpeed;
    private TMP_Text txtGameMode;
    private TMP_Text txtAimMode;
    private TMP_Text txtScoreLevel;
    private TMP_Text txtTotalScore;

    // Start is called before the first frame update
    void Start()
    {
        ps = PlayerStats.Instance;
        ps.onStatsChangedCallback += UpdateText;       

        GameObject canvas = transform.Find("Canvas").gameObject;

        GameObject panel = canvas.transform.Find("Panel").gameObject;
        txtSpeed = panel.transform.Find("Text_speed").GetComponent<TMP_Text>();
        txtDamage = panel.transform.Find("Text_damage").GetComponent<TMP_Text>();
        txtFireRate = panel.transform.Find("Text_firerate").GetComponent<TMP_Text>();
        txtRange = panel.transform.Find("Text_range").GetComponent<TMP_Text>();
        txtBulletSize = panel.transform.Find("Text_bulletsize").GetComponent<TMP_Text>();
        txtBulletSpeed = panel.transform.Find("Text_bulletspeed").GetComponent<TMP_Text>();

        GameObject panelGame = canvas.transform.Find("PanelGame").gameObject;
        txtGameMode = panelGame.transform.Find("Text_Mode").GetComponent<TMP_Text>();
        txtAimMode = panelGame.transform.Find("Text_Aim").GetComponent<TMP_Text>();

        GameObject panelTime = canvas.transform.Find("PanelTime").gameObject;
        txtScoreLevel = panelTime.transform.Find("Text_Score_Level").GetComponent<TMP_Text>();
        txtTotalScore = panelTime.transform.Find("Text_Total_Score").GetComponent<TMP_Text>();

        UpdateText();
    }

    private void SetStat(TMP_Text text, float value)
    {
        var label = text.text.Split(':')[0].Trim();
        text.text = $"{label}: {value:F2}";
    }

    private void SetStatString(TMP_Text text, string value)
    {
        var label = text.text.Split(':')[0].Trim();
        text.text = $"{label}: {value}";
    }

    private void UpdateText()
    {
        SetStat(txtSpeed, ps.ProjectileSpeed);
        SetStat(txtDamage, ps.ProjectileDamage);
        SetStat(txtFireRate, ps.FireRate);
        SetStat(txtRange, ps.ProjectileRange);
        SetStat(txtBulletSize, ps.ProjectileScale);
        SetStat(txtBulletSpeed, ps.ProjectileSpeed);

        SetStatString(txtScoreLevel, ps.LevelScore.ToString());
        SetStatString(txtTotalScore, ps.Score.ToString());


        var g = Game.Instance;
        if (g.gameMode == GameMode.Hard)
            SetStatString(txtGameMode, "Hard");
        else
            SetStatString(txtGameMode, "Normal");

        switch (g.aimType)
        {
            case AimType.Manual:
                SetStatString(txtAimMode, "Manual");
                break;
            case AimType.AutomaticNearest:
                SetStatString(txtAimMode, "Automatic Nearest");
                break;
            case AimType.AutomaticWeakest:
                SetStatString(txtAimMode, "Automatic Weakest");
                break;
            case AimType.AutomaticStrongest:
                SetStatString(txtAimMode, "Automatic Strongest");
                break;
            default:
                SetStatString(txtAimMode, "Unknown");
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
