using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cuboneHealth : MonoBehaviour
{
    // Start is called before the first frame update
    public float health;
    public float maxHealth;
    //public Image Healthbar;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
    //    Healthbar.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1);
    }

    public void TakeDamage(int dmg) {
        health -= dmg;

        //play hurt animation
        anim.SetTrigger("Hurt");
        //die
        if (health <= 0) {
            Die();
        }
    }

    void Die() {
        anim.SetBool("IsDead",true);
    }
}
