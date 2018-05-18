using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour
{
    private Image healthBar;
    private Image armorBar;
    private Image expBar;
    private GameModel gameModel;
    private PlayerModel playerModel;
    private Text ammoView;
    private Text level;
    private Shooting shootingWrapper;
    private bool toUpdateBars = true;

    private const float maxFillAmountHealth = 0.7f;
    private const float maxFillAmountArmor = 0.3f; 

    void Start()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<Image>();
        armorBar = GameObject.Find("ArmorBar").GetComponent<Image>();
        expBar = GameObject.Find("ExpBar").GetComponent<Image>();
        ammoView = GameObject.Find("Ammo").GetComponent<Text>();
        level = GameObject.Find("Level").GetComponent<Text>();
        gameModel = GameObject.Find("GameModel").GetComponent<GameModel>();
        shootingWrapper = GameObject.Find("Weapon").GetComponent<Shooting>();
        playerModel = gameModel.MainPlayer;
        playerModel.OnStateChanged += UpdatePlayer;
        shootingWrapper.OnStateChanged += UpdateAmmo;
        UpdatePlayer();
    }

    void FixedUpdate()
    {
        if (toUpdateBars)
        {
            var healthTo = (maxFillAmountHealth / 100) / (playerModel.MaxHealth / 100) * playerModel.Health;
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, healthTo, 0.1f);
            var armorTo = (maxFillAmountArmor / 100) / (playerModel.MaxArmor / 100) * playerModel.Armor;
            armorBar.fillAmount = Mathf.Lerp(armorBar.fillAmount, armorTo, 0.1f);

            var differenceHealth = Mathf.Abs(healthBar.fillAmount - healthTo);
            var differenceArmor = Mathf.Abs(armorBar.fillAmount - armorTo);
            if (differenceHealth < 1e-3 && differenceArmor < 1e-3)
                toUpdateBars = false;
        }
        var expTo = 1 / (gameModel.ExpToLevelUp - gameModel.LastExpMargin) * (gameModel.Experience - gameModel.LastExpMargin);
        expBar.fillAmount = Mathf.Lerp(expBar.fillAmount, expTo, 0.1f);
    }

    private void UpdateAmmo(float magazine, float ammo)
    {
        if (float.IsInfinity(ammo))
            ammoView.text = $"{magazine}/" + '\u221E';
        else
            ammoView.text = $"{magazine}/{ammo}";
    }

    private void UpdatePlayer()
    {
        toUpdateBars = true;
        level.text = $"Level {playerModel.Level}";
    }  
}
