using UnityEngine;

public enum CardType
{
    Shoot,
    DoubleJump,
    Burst,
    Spin,
    //Add more
}

[System.Serializable]
public class Card
{
    public CardType cardType;
    public int uses;

    public float cooldown = 0f;
    private float lastUsedTime = -Mathf.Infinity;

    public float bulletSpeed = 0f;
    public int bulletDamage = 0;

    public Card(CardType type, int uses = -1, float cooldown = 0f)
    {
        cardType = type;

        switch (cardType)
        {
            case CardType.Shoot:
                this.uses = 20;
                this.cooldown = 0.2f;
                bulletSpeed = 25f;
                break;

            case CardType.Burst:
                this.uses = 5;
                this.cooldown = 0.7f;
                bulletSpeed = 35f;
                break;

            case CardType.DoubleJump:
                this.uses = 1;
                this.cooldown = 0f;
                break;

            case CardType.Spin:
                this.uses = 3;
                this.cooldown = 1.3f;
                bulletSpeed = 40f;
                break;


            default:
                uses = 1;
                this.cooldown = 0f;
                break;
        }

        if (uses > 0)
        {
            this.uses = uses;
        }

        if (cooldown > 0)
        {
            this.cooldown = cooldown;
        }
    }

    public bool use()
    {
        if (uses > 0 && !IsOnCooldown())
        {
            uses--;
            lastUsedTime = Time.time;
            return true;
        }
        return false;
    }

    public bool isUsed()
    {
        return uses <= 0;
    }

    public bool IsOnCooldown()
    {
        return Time.time < lastUsedTime + cooldown;
    }
}
