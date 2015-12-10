using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    // Use this for initialization
    private float health;
    public HealthBar healthBar;
    private bool alive = true;

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
                    Destroy(this.gameObject);
                }
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
