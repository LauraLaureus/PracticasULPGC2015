using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    // Use this for initialization
    private float health;
    public HealthBar healthBar;
    private bool alive = true;
    public GameObject keyPrefab;

    void Start () {
        health = 100.0f;
    }
	
	// Update is called once per frame
	void Update () {
        updateHealthBar();
    }

    public void TakeDamage (float amountDamage)
    {
        if (alive)
        {
            health -= amountDamage;
            if (health < 0)
            {
                health = 0;
                alive = false;
                if (this.gameObject.CompareTag("Player"))
                {
                    Application.LoadLevel(0);
                }
                else
                {
                    Instantiate(keyPrefab, transform.position + transform.forward, transform.rotation);
                    Destroy(this.gameObject);
                }
            }
        }
        
    }

    public void AddHealth(float amountHealth)
    {
        if (alive)
        {
            health += amountHealth;
            if (health > 100)
            {
                health = 100;
            }
        }

    }

    private void updateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(health);
        }
    }

}
