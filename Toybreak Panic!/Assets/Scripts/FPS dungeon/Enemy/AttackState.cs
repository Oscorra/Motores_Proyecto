using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer;
    private float losePlayerTimer;
    private float shotTimer;
    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer())
        {
            losePlayerTimer = 0f;
            moveTimer += Time.deltaTime;
            shotTimer += Time.deltaTime;
            enemy.transform.LookAt(enemy.Player.transform);

            if (shotTimer > enemy.fireRate)
            {
                Shoot();
            }
            // move enemy randomly around player every 3-7 seconds
            if (moveTimer > Random.Range(3, 7))
            {
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));

                moveTimer = 0f;
            }
        }
        else // loses sight of player
        {
            losePlayerTimer += Time.deltaTime;
            if (losePlayerTimer > 6f)
            {
                stateMachine.ChangeState(new PatrolState());
            }
        }
    }

    public void Shoot()
    {
        //reference to arrow head
        Transform arrowHead = enemy.arrowHead;
        // instantiate arrow
        GameObject arrow = GameObject.Instantiate(Resources.Load("Prefabs/Arrow") as GameObject, arrowHead.position, enemy.transform.rotation);
        // direction to player
        Vector3 shootDirection = (enemy.Player.transform.position - arrowHead.transform.position).normalized;
        // add force to rb
        arrow.GetComponent<Rigidbody>().linearVelocity = Quaternion.AngleAxis(Random.Range(-3, 3f), Vector3.up) * shootDirection * 25;
        Debug.Log("Shoot");
        shotTimer = 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
