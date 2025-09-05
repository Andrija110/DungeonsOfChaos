using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Wave = System.Collections.Generic.List<System.Tuple<int, float, float>>;

public enum GameMode
{
    Normal,
    Hard
}

public enum AimType
{
    Manual,
    AutomaticNearest,
    AutomaticWeakest,
    AutomaticStrongest
}
public class Game 
{
    const int START_LEVEL_COUNT = 200;
    const float VALUE_STEP_SECONDS = 2;

    #region Sigleton
    private static Game instance;
    public static Game Instance
    {
        get
        {
            if (instance == null)
                instance = new Game(); 
            return instance;
        }
    }
    #endregion


    public Level level;
    public Portal[] portals;
    public Portal exit;
    public GameMode gameMode = GameMode.Hard;
    public AimType aimType = AimType.Manual;

    bool startWaves = false;
    int numberOfEnemies = 0;
    int waveNumber = 0;
    bool waveFlag = false;
    float timer;
    bool sceneOver = false;
    int currentLevel;    

    List<int> usedUpgrades = new();

    //public Game()
    //{
    //    Resources.Load("Assets/New audio/Shoot_sound.wav");
    //}
    public void StartWaves()
    {
        startWaves = true;
    }

    public void StopWaves()
    {
        startWaves = false;
    }

    
    // Ovo zovemo kad završimo sa jednim nivoom
    public void FinishLevel()
    {
        var timer = GameObject.FindWithTag("Timer").GetComponent<Timer>();
        timer.StopCountDown();
        PlayerStats.Instance.LevelFinished();

        sceneOver = true;
        currentLevel++;
        GameObject.Find("Portal").GetComponent<Portal>().isActive = true;

        SaveGame();
    }

    public void StartLevel()
    {       
        PlayerStats.Instance.HealFull();
        startWaves = false;
        waveFlag = false;
        numberOfEnemies = 0;
        waveNumber = 0;
        var levelObject = GameObject.Find("Level");
        if (levelObject != null)
        {
            level = levelObject.GetComponent<Level>();
            currentLevel = level.levelNumber;            
            //usedUpgrades.Clear();
            sceneOver = false;

            var go = GameObject.FindWithTag("Timer");
            var timer = go.GetComponent<Timer>();
            if (timer != null)
                timer.onValueChangedCallback += UpdateScore;
            else Debug.Log("Nema timer-a!");

            
            timer.StartCountDown(START_LEVEL_COUNT, VALUE_STEP_SECONDS);

        }
        else
        {
            //currentLevel = 1;
            sceneOver = true;
        }   
    }

    public void UpdateScore(int score)
    {
        PlayerStats.Instance.SetLevelScore(score);
    }

    public void UpdatePortals()
    {
        if (currentLevel == 0) currentLevel = 1;
        var portalsObject = GameObject.Find("Portals");
        if (portalsObject != null)
        {
            portals = portalsObject.GetComponentsInChildren<Portal>();
            var tilemap = GameObject.Find("Ground001").GetComponent<Tilemap>();

            foreach (var portal in portals)
            {
                var tilePos = tilemap.WorldToCell(portal.transform.position);
                var tile = tilemap.GetTile<AnimatedTile>(tilePos);
                if (portal.SceneName == currentLevel.ToString())
                {
                    portal.isActive = true;
                    //tile.hideFlags = HideFlags.None; // Pokažemo tile ako je to trenutni nivo
                    tilemap.SetTileAnimationFlags(tilePos, TileAnimationFlags.None);
                }
                else
                {
                    portal.isActive = false;
                    //tile.hideFlags = HideFlags.HideAndDontSave; // Sakrijemo tile da ne bude vidljiv
                    tilemap.SetTileAnimationFlags(tilePos, TileAnimationFlags.PauseAnimation);
                }
            }
        }
    }

    public void GoToScene(string sceneName)
    {
        // Učitati scenu
        var sceneLoader = SceneManager.LoadSceneAsync(sceneName);
        sceneLoader.completed += (x) =>
            {
                if (int.TryParse(sceneName, out _))
                {
                    var portal = GameObject.Find("Portal").GetComponent<Portal>();
                    portal.isActive = false;
                }               

                PlayerStats.Instance.onHealthChangedCallback = null;
                PlayerStats.Instance.onStatsChangedCallback = null;
                StartLevel();
            };
        

    }

       public void SaveGame()
    {        
        var go = GameObject.FindGameObjectWithTag("DBHelper");
        if (go != null)
        {
            var dbhelper = go.GetComponent<DBHelper>();
            if (dbhelper != null)
                dbhelper.SaveCurentPlayer();
        }
        else Debug.Log("Nema dbhelper-a!");
    }

    public void Update()
    {
        if (sceneOver) return;

        if (Time.frameCount % 10 != 0) return;

        if (!startWaves) return;

        if (numberOfEnemies == 0 && !waveFlag)
        {
            timer = Time.time;
            waveFlag = true;
        }

        if (numberOfEnemies == 0 && Time.time - timer > level.wavePause)
        {            
            if (waveNumber < waves[currentLevel-1].Count)
            {
                CreateWave(waves[currentLevel-1][waveNumber++]);
                if (waveNumber % 5 == 0) SpawnRandomUpgrade();  // Svakih 5 valova stvaramo random upgrade
                waveFlag = false;
            }
            else
            {                
                FinishLevel();
            }
        }
    }

    private void SpawnRandomUpgrade()
    {
        bool found = false;
        while (!found)
        {
            var randint = UnityEngine.Random.Range(0, level.upgradePrefabs.Count());
            if (!usedUpgrades.Contains(randint))
            {
                found = true;
                usedUpgrades.Add(randint);              
                level.SpawnUpgrade(randint);
            }
        }
    }


    void CreateWave(Wave wave)
    {
        waveFlag = true;
        foreach (var subWave in wave)
        {
            CreateNewEnemy(subWave);
        }
    }

    void CreateNewEnemy(Tuple<int, float, float> subWave)
    {   
        numberOfEnemies++;
        level.CreateNewEnemy(subWave);
    }
    
        
    public void EnemyDestroyed()
    {
        numberOfEnemies--;
    }

    public void IgnoreAllTraps(Collider2D collider)
    {
        var trap = GameObject.FindWithTag("Trap");
        if (trap != null) {
            Physics2D.IgnoreCollision(collider, trap.GetComponent<Collider2D>());
        }
    }

    internal void RestartLevel()
    {
        GoToScene(currentLevel.ToString());
    }

    internal void SwitchGameMode()
    {
        if (gameMode == GameMode.Hard) gameMode = GameMode.Normal;
        PlayerStats.Instance.onStatsChangedCallback?.Invoke();

    }

    internal void SwitchAimMode()
    {
        if (gameMode == GameMode.Normal)
        {
            switch (aimType)
            {
                case AimType.Manual: 
                    aimType = AimType.AutomaticNearest;
                    break;
                case AimType.AutomaticNearest:
                    aimType = AimType.AutomaticWeakest;
                    break;
                case AimType.AutomaticWeakest:
                    aimType = AimType.AutomaticStrongest;
                    break;
                case AimType.AutomaticStrongest:
                    aimType = AimType.Manual;
                    break;
                default:
                    break;
            }
            PlayerStats.Instance.onStatsChangedCallback?.Invoke();
        }
    }

    private List<List<Wave>> waves = new List<List<Wave>>()
    {
        // Scena 1
        new List<Wave> {
            new Wave{
                new (0, -1f, -6.5f),
                new (0, 2f, -7f)
            },
            new Wave {
                new (0, -1f, -6.5f),
                new (0, 2f, -7f),
                new (0, 2f, -6.5f),
                new (0, -1f, -7f)
            },
            new Wave {
                new (1, -1f, -6.5f),
            },
            new Wave {
                new (1, -1f,-6.5f),
                new (1, 2f, -6.5f)
            },
            new Wave {
                new (1, -1f,-6.5f),
                new (1, 2f,-6.5f),
                new (0, 2f,-7f),
                new (0, -1f,-7f)
            },
            new Wave {
                new (2, -1f,-6.5f),
                new (1, 2f,-6.5f)
            },
            new Wave {
                new (2, -1f,-6.5f),
                new (2, 2f, -6.5f),
                new (0, 2f, -6.5f),
                new (0, 2f, -6.5f)
            },
             new Wave {
                new (2, -1f,-6.5f),
                new (2, 2f, -6.5f),
                new (1, -1f,-6.5f),
                new (1, 2f, -6.5f),
                new (0, 2f, -6.5f),
                new (0, 2f, -6.5f)
            },
              new Wave {
                new (1, -1f,-6.5f),
                new (1, 2f, -6.5f),
                new (1, -1f,-6.5f),
                new (1, 2f, -6.5f),
                new (0, 2f, -6.5f),
                new (0, 2f, -6.5f),
                new (0, 2f, -6.5f),
                new (0, 2f, -6.5f)
            },
              new Wave{
                new (3, -1f, -6.5f)
            },



        },
        // Scena 2
        new List<Wave> {
           new Wave{
            new (0, 0f, -5f),
            new (0, 0f, -7f)
        },
           new Wave {
            new (1, 0f, -5f)
        },
           new Wave{
            new (0, 0f, -5f),
            new (0, 0f, -7f),
            new (1, 0f, -5f),
            new (1, 0f, -7f)
        },
           new Wave{
            new (1, 0f, -5f),
            new (1, 0f, -5.5f),
            new (1, 0f, -7.5f),
            new (1, 0f, -7f)
        },
           new Wave{
            new (0, 0f, -5.5f),
            new (0, 0f, -7f),
            new (0, 0f, -7.5f),
            new (0, 0f, -7f),
            new (0, 0f, -6.5f),
            new (0, 0f, -6f)
        },
           new Wave {
            new (2, 0f, -5f)
        },
           new Wave {
            new (1, 0f, -5f),
            new (1, 0f, -7f),
            new (2, 0f, -5f),
        },
           new Wave {
            new (2, 0f, -5f),
            new (2, 0f, -7f),
        },
           new Wave {
            new (1, 0f, -5f),
            new (1, 0f, -7f),
            new (2, 0f, -5f),
            new (2, 0f, -7f),
            new (0, 0f, -5f),
            new (0, 0f, -7f),
        },
           new Wave {
            new (3, 0f, -5f),
        },

    },
        // Scena 3
        new List<Wave> {
            new Wave{
                new (0, -6.5f, 2.5f),
                new (0, 6.5f, 2.5f)
            },
            new Wave{
                new (0, -6.5f, 2.5f),
                new (0, 6.5f, 2.5f),
                new (0, -6.5f,-12.5f),
                new (0, 6.5f, -12.5f)
            },
            new Wave{
                new (0, -6.5f, 2.5f),
                new (1, 6.5f, 2.5f)
            },
            new Wave{
                new (1, -6.5f, 2.5f),
                new (1, 6.5f, 2.5f),
                new (0, -6.5f,-12.5f),
                new (0, 6.5f, -12.5f)
            },
            new Wave{
                new (2, -6.5f, 2.5f),
                new (2, 6.5f, 2.5f),
            },
            new Wave{
                new (1, -6.5f, 2.5f),
                new (2, 6.5f, 2.5f),
                new (0, -6.5f,-12.5f),
                new (0, 6.5f, -12.5f)
            },
            new Wave{
                new (0, -6.5f, 2.5f),
                new (0, 6.5f, 2.5f),
                new (0, -6.5f,-12.5f),
                new (0, 6.5f, -12.5f),
                new (1, -6.5f, 2.5f),
                new (1, 6.5f, 2.5f),
                new (1, -6.5f,-12.5f),
                new (1, 6.5f, -12.5f)
            },
             new Wave{
                new (2, -6.5f, 2.5f),
                new (2, 6.5f, 2.5f),
                new (2, -6.5f,-12.5f),
                new (2, 6.5f, -12.5f)
            },
             new Wave{
                new (1, -6.5f, 2.5f),
                new (1, 6.5f, 2.5f),
                new (1, -6.5f,-12.5f),
                new (1, 6.5f, -12.5f),
                new (2, -6.5f, 2.5f),
                new (2, 6.5f, 2.5f),
                new (2, -6.5f,-12.5f),
                new (2, 6.5f, -12.5f)
            },
              new Wave{
                new (3, 0f, -5f),
            },
          },

        // Scena 4
        new List<Wave> {
            new Wave{
                new (0, -6.5f, 2.5f),
                new (0, 6.5f, 2.5f)
            },
            new Wave{
                new (0, -6.5f, 2.5f),
                new (0, 6.5f, 2.5f),
                new (0, -6.5f,-12.5f),
                new (0, 6.5f, -12.5f)
            },
            new Wave{
                new (0, -6.5f, 2.5f),
                new (1, 6.5f, 2.5f)
            },
            new Wave{
                new (1, -6.5f, 2.5f),
                new (1, 6.5f, 2.5f),
                new (0, -6.5f,-12.5f),
                new (0, 6.5f, -12.5f)
            },
            new Wave{
                new (2, -6.5f, 2.5f),
                new (2, 6.5f, 2.5f),
            },
            new Wave{
                new (1, -6.5f, 2.5f),
                new (2, 6.5f, 2.5f),
                new (0, -6.5f,-12.5f),
                new (0, 6.5f, -12.5f)
            },
            new Wave{
                new (0, -6.5f, 2.5f),
                new (0, 6.5f, 2.5f),
                new (0, -6.5f,-12.5f),
                new (0, 6.5f, -12.5f),
                new (1, -6.5f, 2.5f),
                new (1, 6.5f, 2.5f),
                new (1, -6.5f,-12.5f),
                new (1, 6.5f, -12.5f)
            },
             new Wave{
                new (2, -6.5f, 2.5f),
                new (2, 6.5f, 2.5f),
                new (2, -6.5f,-12.5f),
                new (2, 6.5f, -12.5f)
            },
             new Wave{
                new (1, -6.5f, 2.5f),
                new (1, 6.5f, 2.5f),
                new (1, -6.5f,-12.5f),
                new (1, 6.5f, -12.5f),
                new (2, -6.5f, 2.5f),
                new (2, 6.5f, 2.5f),
                new (2, -6.5f,-12.5f),
                new (2, 6.5f, -12.5f)
            },
              new Wave{
                new (3, 0.5f, -8f),
            },
          }

    };
}

    


