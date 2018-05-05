using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DropType
{
    Health,
    Armor
}

public class Drop : MonoBehaviour
{
    private PlayerController playerController;
    private GameModel gameModel;
    public float Points = 10;
    public float Lifetime = 20;
    public DropType dropType;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        gameModel = GameObject.Find("GameModel").GetComponent<GameModel>();
        gameObject.transform.localScale *= Points / 10;
        Destroy(gameObject, Lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var isHeal = dropType == DropType.Health;
            var isArmor = dropType == DropType.Armor;
            if (isArmor && gameModel.MainPlayer.Armor != gameModel.MainPlayer.MaxArmor)
            {
                playerController.TakeArmor(Points);
                Destroy(gameObject);
            }
            else if (isHeal && gameModel.MainPlayer.Health != gameModel.MainPlayer.MaxHealth)
            {
                playerController.TakeHealth(Points);
                Destroy(gameObject);
            }
        }
    }
}
