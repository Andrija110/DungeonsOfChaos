using UnityEngine;

public class PlayerStats 
{
    public delegate void OnHealthChangedDelegate();
    public OnHealthChangedDelegate onHealthChangedCallback;

    public delegate void OnStatsChangedDelegate();
    public OnStatsChangedDelegate onStatsChangedCallback;

    #region Singleton
    private static PlayerStats instance;
    public static PlayerStats Instance
    {
        get
        {
            if (instance == null)
                instance = new PlayerStats(); 
            return instance;
        }
    }
    #endregion
    public string PlayerName { get; set; }
    public int PlayerDBId { get; set; }
    public int LevelScore { get; set; } = 0;
    public int Score { get; set; } = 0;
    public int HighScore { get; set; }
    public float Health { get; set; } = 3;
    public float MaxHealth { get; set; } = 3;
    public float MaxTotalHealth { get; set; } = 5;
    public float MoveSpeed { get; set; } = 2;
    

    public float ProjectileSpeed = 200f;
    public float FireRate = 1f;   // Koliko puta u sekundi
    public float ProjectileDamage = 1f;
    public float ProjectileRange = 10f;
    public float ProjectileScale = 0.4f;

    public Pause gameOver = null;

    public void SetLevelScore(int score)
    {
        this.LevelScore = score;        
        onStatsChangedCallback?.Invoke();
    }

    public void LevelFinished()
    {
        Score += LevelScore;
        if (Score > HighScore) HighScore = Score;
        LevelScore = 0;
    }

    public void HealFull()
    {
        this.Health = this.MaxHealth;
        ClampHealth();
    }
    public void Heal(float health)
    {
        this.Health += health;
        ClampHealth();
    }

    public void TakeDamage(float dmg)
    {
        Health -= dmg;
        ClampHealth();
    }

    public void AddHealth()
    {
        if (MaxHealth < MaxTotalHealth)
        {
            MaxHealth += 1;
            Health = MaxHealth;

            onHealthChangedCallback?.Invoke();
        }   
    }

    void ClampHealth()
    {
        if (Health <=0 )
        {
            gameOver.PauseGame();
        }
        Health = Mathf.Clamp(Health, 0, MaxHealth);

        onHealthChangedCallback?.Invoke();

    }

    public void MakeScore()
    {
        Score += LevelScore;
    }
}
