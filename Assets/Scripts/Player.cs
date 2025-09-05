using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
   
    //public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private Game game;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    private WeaponControl weaponControl;
    private bool IsFiring = false;
    private Vector2 FiringDirection = Vector2.zero;

    private AudioSource audioScr;
    private Pause pause;
    private PlayerStats ps = PlayerStats.Instance;

    private bool DebugKillEnemy = false;
    private float DebugProjectileDamage = 0;
    private float DebugFireRate=0;
    private float DebugProjectileSpeed=0;

    // Start is called before the first frame update
    void Start()
    {        
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        weaponControl = GetComponentInChildren<WeaponControl>();
        audioScr = GetComponent<AudioSource>();
        var pauseMenu = GameObject.Find("PauseMenu");
        pause = pauseMenu.GetComponent<Pause>();
        pauseMenu.SetActive(false);
        var gameOverMenu = GameObject.Find("Game Over");
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(false);
            PlayerStats.Instance.gameOver = gameOverMenu.GetComponent<Pause>();
        }

        game = Game.Instance;
        //game.StartLevel();
    }

    private void Update()
    {
        game.Update();
    }

    public void FixedUpdate()
    {
        if (moveInput != Vector2.zero)
        {
            bool success = MovePlayer(moveInput);

            if (!success)
            {
                success = MovePlayer(new Vector2(moveInput.x, 0));
                if (!success) MovePlayer(new Vector2(0, moveInput.y));
            }

            if (success)
            {
                if (!audioScr.isPlaying)
                    audioScr.Play();
            }
            else
            {
                audioScr.Stop();
            }

            animator.SetBool("IsMoving", success);
        }
        else 
        { 
            animator.SetBool("IsMoving", false);
            audioScr.Stop();
        }

        if (IsFiring) 
        {             
            weaponControl.Shoot(gameObject, FiringDirection);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            DebugGotoLevel(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            DebugGotoLevel(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            DebugGotoLevel(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            DebugGotoLevel(4);
    }

    public bool MovePlayer(Vector2 direction)
    {
        // Check for potential collisions
        int count = rb.Cast(
            direction, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
            movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
            castCollisions, // List of collisions to store the found collisions into after the Cast is finished
            ps.MoveSpeed * Time.fixedDeltaTime + collisionOffset); // The amount to cast equal to the movement plus an offset

        if (count == 0)
        {
            Vector2 moveVector = direction * ps.MoveSpeed * Time.fixedDeltaTime;

            // No collisions
            rb.MovePosition(rb.position + moveVector);
            return true;
        }
        else
        {
            // Print collisions
           /* foreach (RaycastHit2D hit in castCollisions)
            {
                print(hit.ToString());
            }*/

            return false;
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        // Only set the animation direction if the upgrade is trying to move
        if (moveInput != Vector2.zero)
        {
            animator.SetFloat("XInput", moveInput.x);
            animator.SetFloat("YInput", moveInput.y);
           /* if (!audioScr.isPlaying) 
            {
                audioScr.Play();
            }*/

            //print($"{moveInput.x}, {moveInput.y}");

        }
        
    }

    public void OnFire(InputValue value)
    {       
        if (value == null)
        {
            IsFiring = false;
            return;
        }

        FiringDirection = value.Get<Vector2>();
        if (FiringDirection == Vector2.zero) 
        {
            IsFiring = false;            
        }
        else
        {
            IsFiring = true;             
        }                                       
    }

    public void OnPause(InputValue value)
    {
        //Debug.Log(value);
        pause.PauseGame();
    }

    public void OnKillme(InputValue value)
    {
        PlayerStats.Instance.TakeDamage(1);
    }

    public void OnKillEnemy(InputValue value)
    {
        if (DebugKillEnemy)
        {
            ps.ProjectileDamage = DebugProjectileDamage;
            ps.FireRate = DebugFireRate;
            ps.ProjectileSpeed = DebugProjectileSpeed;
            DebugKillEnemy = false;

        }
        else
        {
            DebugProjectileDamage = ps.ProjectileDamage;
            DebugFireRate = ps.FireRate;
            DebugProjectileSpeed = ps.ProjectileSpeed;
            ps.ProjectileDamage = 100;
            DebugKillEnemy = true;
        }
            
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var go = other.gameObject;

        if (go.CompareTag("Upgrade"))
        {
            var upgrade = go.GetComponent<Upgrade>();
            ApplyUpgrade(upgrade);
            Destroy(go);
        }
        else if (go.CompareTag("EnemyBullet"))
        {
            var bullet = go.GetComponent<Bullet>();
            PlayerStats.Instance.TakeDamage(bullet.damage);
            Destroy(go);
        }
        else if (go.CompareTag("Trap"))
        {
            //var trap = go.GetComponent<Trap>();
            //trap.ActivateTrap();
            game.StartWaves();
            Destroy(go);
        }
        else if (go.CompareTag("Portal"))
        {
            var portal = go.GetComponent<Portal>();
            if (portal.isActive)
            {
                game.GoToScene(portal.SceneName);
            }


        }
    }

    public void ApplyUpgrade(Upgrade upgrade)
    {
        ps.MoveSpeed *= upgrade.PlayerSpeedMultiplier;
        ps.ProjectileSpeed *= upgrade.ProjectileSpeedMultiplier;
        ps.FireRate *= upgrade.FireRateMultiplier;
        ps.ProjectileDamage *= upgrade.ProjectileDamageMultiplier;
        ps.ProjectileRange *= upgrade.ProjectileRangeMultiplier;
        ps.ProjectileScale *= upgrade.ProjectileScaleMultiplier;

        ps.onStatsChangedCallback?.Invoke(); 
    }

    public void OnChangeMode(InputValue input)
    {
        Game.Instance.SwitchGameMode();
    }

    public void OnChangeAim(InputValue value)
    {
        Game.Instance.SwitchAimMode();
    }

    private void DebugGotoLevel(int level) 
    {
        PlayerStats.Instance.PlayerName = "Debug";
        PlayerStats.Instance.PlayerDBId = 0;
        Game.Instance.GoToScene(level.ToString());

        Game.Instance.UpdatePortals();
    }

    private void OnDebug(InputValue value) //InputAction.CallbackContext context)
    {
        //if (Input.GetKey(KeyCode.LeftControl))
        //{
           
        //}
    }

    


    // Update is called once per frame
    //void Update()
    //{
    //    moveInput.x = Input.GetAxisRaw("Horizontal");
    //    moveInput.y = Input.GetAxisRaw("Vertical");

    //    moveInput.Normalize();

    //    rb.velocity = moveInput * moveSpeed;

    //    if (moveInput != Vector2.zero) 
    //    {
    //        animator.SetFloat("xImput", moveInput.x);
    //        animator.SetFloat("yImput", moveInput.y);
    //    }


    //}
}
