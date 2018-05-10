using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Shooting))]
public class PlayerController : MonoBehaviour
{
    public float Velocity;
    public event Action<float> GetDamage;
    public event Action<float> GetHealth;
    public event Action<float> GetArmor;
    public bool AllowShooting { get; private set; }
    public float Regeneration = 0.1f;
    public GameObject lvlupEffect;
    public float LvlupTime = 3f;

    private float lvlupTimer;
    private Rigidbody playerRigidbody;
    private int floorMask;
    private float camRayLength = 100f;
    private Vector3 movement;
    private Shooting playerShooting;
    private GameModel gameModel;

    void Start()
    {
        AllowShooting = false;
        floorMask = LayerMask.GetMask("Terrain");
        playerRigidbody = GetComponent<Rigidbody>();
        playerShooting = GetComponent<Shooting>();
        gameModel = GameObject.Find("GameModel").GetComponent<GameModel>();
    }

    public void UpLevel() => lvlupTimer = LvlupTime;

    public void TakeDamage(Vector3 enemyPos, float damage)
    {
        GetDamage?.Invoke(damage);
        var force = transform.position - enemyPos;
        //playerRigidbody.AddForce(force * damage * 10 / gameModel.MainPlayer.Level, ForceMode.VelocityChange);
    }

    public void TakeHealth(float health)
        => GetHealth?.Invoke(health);

    public void TakeArmor(float armor)
        => GetArmor?.Invoke(armor);

    void FixedUpdate()
    {
        if (lvlupTimer > 0)
        {
            lvlupTimer -= Time.deltaTime;
            if (!lvlupEffect.activeInHierarchy)
                lvlupEffect.SetActive(true);
        }
        else
        {
            if (lvlupEffect.activeInHierarchy)
                lvlupEffect.SetActive(false);
        }
        TakeHealth(Regeneration / 100);
        Move();
        LookAtCursor();
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            if (AllowShooting == false)
                AllowShooting = true;
        }
        else
        {
            if (AllowShooting == true)
                AllowShooting = false;
        }
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movement.Set(horizontal, 0, vertical);
        movement = movement.normalized * Velocity * Time.deltaTime;
        playerRigidbody.MovePosition(transform.position + movement);  
    }

    private void LookAtCursor()
    {
        var camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(camRay, out RaycastHit floorHit, camRayLength, floorMask))
        {
            var playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;
            var newRotation = Quaternion.LookRotation(playerToMouse);
            playerRigidbody.MoveRotation(newRotation);
        }
    }
}
