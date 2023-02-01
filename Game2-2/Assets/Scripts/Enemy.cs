using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour,TakeDamage<float>
{
    [SerializeField] float hp;
    public float damage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void TestDodamage()
    {
        TakeDamage<float> damage = GetComponent<TakeDamage<float>>();
        damage.TakeDamage(this.damage);
    }

    void TakeDamage<float>.TakeDamage(float damage)
    {
        hp -= damage;
    }
}
