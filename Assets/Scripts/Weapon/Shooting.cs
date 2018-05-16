using System;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    private float damagePerShot;
    private float timeBetweenBullets;
    private float range;
    private float magazineCapacity;
    private float currentMagazine;
    private float ammo;
    private float reloadTime;
    private float reloadTimer;
    private bool isReloading = false;
    private PlayerController playerController;
    private WeaponProperties gunProperties;
    private float timer;
    private Ray shootRay = new Ray();
    private RaycastHit shootHit;
    private ParticleSystem gunParticles;
    private LineRenderer gunLine;
    private Light gunLight;
    private float effectsDisplayTime = 0.2f;
    private AudioSource gunShootSound;
    private AudioSource gunReloadSound;

    public event Action<float, float> OnStateChanged;

    void Start()
    {
        gunProperties = GetComponentInChildren<WeaponProperties>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        gunParticles = GetComponentInChildren<ParticleSystem>();
        gunLine = GetComponentInChildren<LineRenderer>();
        gunLight = GetComponentInChildren<Light>();
        magazineCapacity = gunProperties.Magazine;
        gunShootSound = GetComponentsInChildren<AudioSource>()[0];
        gunReloadSound = GetComponentsInChildren<AudioSource>()[1];
        ammo = gunProperties.Ammo;
        currentMagazine = magazineCapacity;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && reloadTimer <= 0)
            ReloadAmmo();
        damagePerShot = gunProperties.Damage;
        timeBetweenBullets = gunProperties.Frequency;
        range = gunProperties.Range;
        reloadTime = gunProperties.ReloadTime;
        timer += Time.deltaTime;
        if (reloadTimer > 0)
        {
            reloadTimer -= Time.deltaTime;
        }
        else if (isReloading)
        {
            isReloading = false;
            OnStateChanged?.Invoke(currentMagazine, ammo);
        }
        else if (playerController.AllowShooting && timer >= timeBetweenBullets && Time.timeScale != 0)
        {
            timer = 0f;
            Shoot();
        }

        if (timer >= timeBetweenBullets * effectsDisplayTime)
            DisableEffects();
    }

    public void DisableEffects()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
    }

    private void ReloadAmmo()
    {
        if (ammo == 0 || reloadTimer > 0)
            return;
        var delta = magazineCapacity - currentMagazine;
        if (delta == 0)
            return;
        if (ammo >= delta)
        {
            currentMagazine = magazineCapacity;
            ammo -= delta;
        }
        else
        {
            currentMagazine += ammo;
            ammo = 0;
        }
        isReloading = true;
        reloadTimer = reloadTime;
        gunReloadSound.Play();
        OnStateChanged?.Invoke(currentMagazine, ammo);
    }

    private bool AllowShooting()
    {
        if (currentMagazine > 0)
        {
            currentMagazine--;
            return true;
        }
        ReloadAmmo();
        return false;
    }

    private void Shoot()
    {
        gunParticles.Stop();
        if (!AllowShooting())
            return;
        gunShootSound.Play();
        OnStateChanged?.Invoke(currentMagazine, ammo);
        gunLight.enabled = true;
        gunParticles.Play();
        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;
        var camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        var direction = transform.forward;
        if (Physics.Raycast(camRay, out RaycastHit floorHit, range))
            direction = floorHit.point - transform.position;
        if (Physics.Raycast(transform.position, direction, out shootHit, range))
        {
            var collider = shootHit.collider;
            if (collider != null && collider.CompareTag("Enemy"))
                collider.GetComponent<EnemyController>().TakeDamage(damagePerShot);
            gunLine.SetPosition(1, shootHit.point);
        }
        else
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        if (currentMagazine <= 0)
            ReloadAmmo();
    }
}
