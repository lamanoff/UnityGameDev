using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Velocity;
    public event Action<float> GetDamage;
    public event Action<float> GetHealth;
    public event Action<float> GetArmor;
    public bool AllowShooting { get; private set; }
    public float Regeneration = 0.1f;

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

    public void TakeDamage(Vector3 enemyPos, float damage)
    {
        GetDamage(damage);
        var force = transform.position - enemyPos;
        playerRigidbody.AddForce(force * damage * 10 / gameModel.MainPlayer.Level, ForceMode.VelocityChange);
    }

    public void TakeHealth(float health)
        => GetHealth(health);

    public void TakeArmor(float armor)
        => GetArmor(armor);

    void FixedUpdate()
    {
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
