using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBehaviour : MonoBehaviour
{
    [Header("Movement")]

    [Range(0.0f, 100f)]
    public float slimeChaseRange = 5.0f;
    [Range(0.0f, 1.0f)]
    public float chaseSpeed = 0.5f;
    [Range(0.0f, 10.0f)]
    public float slimeMoveFrequency = 1.0f;

    [Header("Light Attack")]

    [Range(0.0f, 10f)]
    public float lightAttackDamage = 2.0f;
    [Range(0.0f, 5.0f)]
    public float lightAttackChargeTime = 1.0f;
    [Range(0.0f, 10.0f)]
    public float lightAttackCooldown = 5.0f;

    [Header("Heavy Attack")]

    [Range(0.0f, 10.0f)]
    public float heavyAttackDamage = 4.0f;
    [Range(0.0f, 100f)]
    public float slimeHeavyAttackRange = 2.0f;
    [Range(0.0f, 5.0f)]
    public float heavyAttackChargeTime = 1.0f;
    [Range(0.0f, 10.0f)]
    public float heavyAttackCooldown = 5.0f;

    [Header("Ranged Attack")]
    [Range(0.0f, 200f)]
    public float slimeRangedAttackRange = 2.0f;
    [Range(0.0f, 5.0f)]
    public float rangedAttackChargeTime = 1.0f;
    [Range(0.0f, 10.0f)]
    public float rangedAttackCooldown = 5.0f;

    private float attack_cooldown = 0.0f;

    [Header("Misc.")]

    [Range(0.0f, 30f)]
    public float attackRange = 10.0f;
    [Range(0.0f, 20f)]
    public float patrolRange = 5.0f;
    private Vector3 startPos;
    public int sideJumpChance = 3;

    public float deathTime = 4.1f;

    public float maxHealth = 10.0f;
    public float health = 10.0f;

    public enum SlimeType
    {
        Grande,
        Largo,
        Smolo,
        Kamikaze
    }

    public SlimeType size = SlimeType.Largo;

    public GameObject deathDrop;
    public GameObject projectile;

    [Range(0.0f, 20.0f)]
    public float explosionRadius = 10.0f;
    public float explosion_damage = 3.0f;

    private bool grounded = true;
    private bool attack_complete = true;
    public bool heavy_attack = false;

    private Vector3 current_direction;
    private float time_in_state = 0.0f;

    private GameObject player;
    private Vector3 target;
    private ParticleSystem fizz;
    private Material mat;
    private Vector3 original_color;

    private bool player_in_ranged_range;
    bool player_in_range;

    public bool canSeePlayer = false;

    public enum state
    {
        IDLE,
        PATROL,
        CHASE,
        JUMP,
        LIGHT_ATTACK,
        HEAVY_ATTACK,
        RANGED_ATTACK,
        DIE
    }

    public state current_state = state.PATROL;

    public void OnDestroy()
    {
        GetComponents<AudioSource>()[0].Stop();
        GetComponents<AudioSource>()[1].Stop();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        GetComponents<AudioSource>()[1].Play();

        if (health > 0.0f)
        {
            Vector3 dir = player.transform.position - transform.position;
            dir = new Vector3(-dir.x, 4.0f, -dir.z);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().AddForce(dir.normalized * damage, ForceMode.Impulse);
        }
        else if (current_state != state.DIE)
        {
            Vector3 dir = player.transform.position - transform.position;
            dir = new Vector3(-dir.x, 4.0f, -dir.z);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().AddForce(dir.normalized * damage, ForceMode.Impulse);

            current_state = state.DIE;
            time_in_state = 0.0f;
        }
        else
        {
            Vector3 dir = player.transform.position - transform.position;
            dir = new Vector3(-dir.x, 4.0f, -dir.z);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().AddForce(dir.normalized * damage * 2, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            if (current_state == state.JUMP)
            {
                Vector3 dir = player.transform.position - transform.position;
                dir = new Vector3(dir.x, 1, dir.z).normalized;

                collision.collider.GetComponent<PlayerMovement>().air_launch = dir * lightAttackDamage * 2f;
                collision.collider.GetComponent<Rigidbody>().velocity = Vector3.zero;
                collision.collider.GetComponent<PlayerMovement>().TakeDamage(heavy_attack ? heavyAttackDamage : lightAttackDamage);

                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        else if (collision.collider.tag != "Enemy")
        {
            GetComponents<AudioSource>()[0].Play();
        }

        if (heavy_attack)
        {
            heavy_attack = false;
        }
    }

    private void MakeNewTarget()
    {
        float theta = Mathf.Deg2Rad * Random.Range(1f, 89f);
        float hyp = Random.Range(0f, patrolRange);
        int quartile = Random.Range(1, 4);

        Vector3 targetOffset = new Vector3(hyp * Mathf.Cos(theta) * (quartile == 1 || quartile == 4 ? 1 : -1),
                                           0, 
                                           hyp * Mathf.Sin(theta) * (quartile == 1 || quartile == 2 ? 1 : -1));

        target = startPos + targetOffset;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        current_direction = transform.forward * 2;

        health = maxHealth;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Fizz")
                fizz = transform.GetChild(i).GetComponent<ParticleSystem>();
        }
        
        startPos = transform.position;
        mat = transform.GetChild(transform.childCount - 2).GetComponent<Renderer>().material;
        original_color = new Vector3(mat.color.r, mat.color.g, mat.color.b);
    }

    void FixedUpdate()
    {
        Debug.DrawLine(transform.position + Vector3.up, target);
        canSeePlayer = !Physics.Raycast(transform.position + new Vector3(0, 3f, 0), player.transform.position - transform.position, Vector3.Distance(transform.position, player.transform.position));
        Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), player.transform.position - transform.position);
        grounded = Physics.Raycast(transform.position + new Vector3(0, 0.01f, 0), Vector3.down, 0.1f);

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.2f);

        if (attack_cooldown <= 0.0f)
        {
            attack_cooldown = 0.0f;
        }
        else
        {
            if (grounded)
            {
                current_state = state.CHASE;
            }
            
            attack_cooldown -= Time.deltaTime;

            return;
        }


        current_state = StateCheck();
        
        switch (current_state)
        {
            case state.IDLE:
                {
                    time_in_state += Time.deltaTime;

                    if (player.GetComponent<PlayerMovement>().health <= 0.0f)
                    {
                        LookAt(player.transform.position,0.03f);
                        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 10 * Mathf.Sin(time_in_state * 2.5f), 0));
                    }

                    break;
                }
            case state.PATROL:
                {
                    if(time_in_state == 0.0f)
                    {
                        MakeNewTarget();
                    }

                    time_in_state += Time.deltaTime;
                    
                    {
                        transform.Translate(Vector3.forward * chaseSpeed * (-1 * Mathf.Cos(time_in_state * slimeMoveFrequency) + 1));
                    }


                    if (Vector3.Distance(target, transform.position) <= 3f)
                    {
                        MakeNewTarget();
                    }

                    if(Physics.Raycast(transform.position + new Vector3(0, 1f, 0), target - transform.position + new Vector3(0, 1f, 0), 100f))
                    {
                        MakeNewTarget();
                    }

                    LookAt(target,0.01f);
                    break;
                }
            case state.CHASE:
                {
                    time_in_state += Time.deltaTime;

                    LookAt(player.transform.position, 0.03f);
                    
                    {
                        transform.Translate(Vector3.forward * chaseSpeed * (-1 * Mathf.Cos(time_in_state * slimeMoveFrequency) + 1));
                    }
                    
                    if (player_in_ranged_range && 
                        player.GetComponent<PlayerMovement>().health > 0f)
                    {
                        time_in_state = 0.0f;
                        current_state = state.RANGED_ATTACK;
                        return;
                    }

                    if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
                    {
                        attack_complete = false;
                        time_in_state = 0.0f;

                        if (size == SlimeType.Grande)
                        {
                            if (Random.Range(0, 2) != 0)
                                current_state = state.LIGHT_ATTACK;
                            else
                                current_state = state.HEAVY_ATTACK;
                        }
                        else
                        {
                            current_state = state.LIGHT_ATTACK;
                        }

                        if (size == SlimeType.Kamikaze)
                        {
                            current_state = state.DIE;
                        }
                    }
                    
                    break;
                }
            case state.JUMP:
                {
                    time_in_state += Time.deltaTime;
                    if(size == SlimeType.Grande)
                        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.2f);

                    if (time_in_state > 4)
                        current_state = state.DIE;

                    break;
                }
            case state.LIGHT_ATTACK:
                {
                    time_in_state += Time.deltaTime;

                    LookAt(player.transform.position, 0.03f);

                    transform.localScale = new Vector3(1, Mathf.Lerp(transform.localScale.y, 0.9f, 0.03f), 1);

                    if (time_in_state > lightAttackChargeTime && attack_cooldown == 0.0f && grounded)
                    {
                        LaunchAtPlayer();
                        current_state = state.JUMP;
                        time_in_state = 0.0f;
                    }

                    break;
                }
            case state.HEAVY_ATTACK:
                {
                    time_in_state += Time.deltaTime;

                    transform.localScale = new Vector3(1, Mathf.Lerp(transform.localScale.y, 0.75f, 0.02f), 1);

                    LookAt(player.transform.position, 0.03f);

                    if (time_in_state > heavyAttackChargeTime && attack_cooldown == 0.0f && grounded)
                    {
                        heavy_attack = true;
                        LaunchAtPlayer();
                        current_state = state.JUMP;
                        time_in_state = 0.0f;
                    }

                    break;
                }
            case state.RANGED_ATTACK:
                {
                    time_in_state += Time.deltaTime;

                    LookAt(player.transform.position, 0.03f);

                    transform.localScale = new Vector3(1, Mathf.Lerp(transform.localScale.y, 0.75f, 0.02f), 1);

                    if (!player_in_ranged_range)
                    {
                        if(player_in_range)
                        {
                            current_state = state.CHASE;
                        }
                        else
                        {
                            current_state = state.PATROL;
                        }
                    }
                    else if (!canSeePlayer)
                    {
                        current_state = state.PATROL;
                    }

                    if (time_in_state > rangedAttackChargeTime && grounded)
                    {
                        attack_cooldown += rangedAttackCooldown;

                        FireProjectileAtPlayer();

                        time_in_state = 0;
                    }

                    break;
                }
            case state.DIE:
                {
                    time_in_state += Time.deltaTime;

                    float sin_a = 1f * Mathf.Sin(time_in_state * 10) + 0.5f;

                    mat.color = new Color(original_color.x + sin_a, original_color.y - 0.2f * sin_a, original_color.z - 0.2f * sin_a, mat.color.a);

                    if ((deathTime / 4) > time_in_state && (time_in_state > 0))
                    {
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            if (transform.GetChild(i).GetComponent<Wobble>() != null)
                                transform.GetChild(i).GetComponent<Wobble>().active = false;
                        }
                    }

                    if (time_in_state > 0)
                    {
                        if (!fizz.isPlaying)
                        {
                            fizz.Play();
                        }
                    }

                    if ((deathTime - 0.1f) > time_in_state && time_in_state > (deathTime / 2))
                    {
                        transform.localScale *= 1.003f;
                    }
                    
                    if (time_in_state > deathTime || (size == SlimeType.Kamikaze && Vector3.Distance(player.transform.position, transform.position) < attackRange))
                    {
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            if (transform.GetChild(i).name == "Fizz")
                            {
                                fizz.transform.parent = null;
                                fizz.transform.localScale = Vector3.one;
                            }
                        }

                        foreach (Collider col in Physics.OverlapSphere(transform.position, explosionRadius))
                        {
                            if (col.tag == "Player")
                            {
                                if (canSeePlayer)
                                {
                                    Vector3 dir = player.transform.position - transform.position;
                                    dir = new Vector3(dir.x, 1, dir.z).normalized;

                                    col.GetComponent<PlayerMovement>().air_launch = dir * 3;
                                    col.GetComponent<Rigidbody>().velocity = Vector3.zero;

                                    col.GetComponent<PlayerMovement>().TakeDamage(explosion_damage * (explosionRadius - Vector3.Distance(transform.position, player.transform.position)));
                                }
                            }
                        }

                        GetComponent<Collider>().enabled = false;

                        if(size == SlimeType.Largo || size == SlimeType.Grande)
                        {
                            Instantiate(deathDrop, transform.GetChild(0).transform.position, transform.rotation);
                            Instantiate(deathDrop, transform.GetChild(1).transform.position, transform.rotation);
                        }
                        if(size == SlimeType.Smolo || size == SlimeType.Kamikaze)
                        {
                            if(deathDrop != null)
                                Instantiate(deathDrop, transform.GetChild(0).transform.position, transform.rotation);
                        }

                        Destroy(fizz.gameObject, 5.0f);
                        Destroy(gameObject);
                    }

                    break;
                }
        }
    }
    
    private void FireProjectileAtPlayer()
    {
        Vector3 dir = player.transform.position - transform.position;
        dir += new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        
        GameObject p = Instantiate(projectile, transform.position + transform.forward * 5 + transform.up, transform.rotation);

        p.GetComponent<Rigidbody>().AddForce(dir.normalized * (30 + Random.Range(0, 5)) + 
                                             new Vector3(0, 0.13f * Vector3.Distance(player.transform.position, transform.position), 0), 
                                             ForceMode.Impulse);

        p.GetComponent<SlimeBehaviour>().current_state = state.DIE;

        attack_cooldown = rangedAttackCooldown;
    }

    public void LaunchAtPlayer()
    {
        bool side_attack = Random.Range(0, sideJumpChance) == 0;

        if (heavy_attack)
            side_attack = false;

        Vector3 dir = player.transform.position - transform.position;
        dir = side_attack ? new Vector3((Random.Range(0, 1) == 0 ? -1 : 1) * dir.z, 4.0f, (Random.Range(0, 1) == 0 ? -1 : 1) * dir.x) : new Vector3(dir.x, 4.0f, dir.z);

        GetComponent<Rigidbody>().AddForce(dir.normalized * 10 + new Vector3(0, heavy_attack ? 5 : 0, 0), ForceMode.Impulse);

        attack_cooldown = lightAttackCooldown;
    }

    private void LookAt(Vector3 point, float speed)
    {
        Vector3 dir = point - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);

        if (dir != Vector3.zero)
        {
            current_direction = Vector3.Lerp(current_direction, dir, speed);

            transform.rotation = Quaternion.LookRotation(current_direction);
        }
    }

    private state StateCheck()
    {
        player_in_range = Vector3.Distance(player.transform.position, transform.position) <= slimeChaseRange;
        player_in_ranged_range = Vector3.Distance(player.transform.position, transform.position) <= slimeRangedAttackRange && !player_in_range;

        if (player.GetComponent<PlayerMovement>().health <= 0.0f)
        {
            return state.IDLE;
        }

            switch (current_state)
        {
            case state.IDLE:
                {
                    if (!grounded)
                    {
                        time_in_state = 0.0f;
                        return state.JUMP;
                    }
                    else if (player_in_range && player.GetComponent<PlayerMovement>().health > 0f)
                    {
                        time_in_state = 0.0f;
                        return state.CHASE;
                    }
                    break;
                }
            case state.PATROL:
                {
                    if (player_in_ranged_range)
                    {
                        time_in_state = 0.0f;
                        return state.RANGED_ATTACK;
                    }
                    if (player_in_range && player.GetComponent<PlayerMovement>().health > 0f && canSeePlayer)
                    {
                        time_in_state = 0.0f;
                        return state.CHASE;
                    }

                    break;
                }
            case state.CHASE:
                {
                    if (!player_in_range || player.GetComponent<PlayerMovement>().health <= 0f || !canSeePlayer)
                    {
                        time_in_state = 0.0f;
                        return state.PATROL;
                    }

                    break;
                }
            case state.JUMP:
                {
                    if (grounded && time_in_state > 0.1f)
                    {
                        if (player_in_range && player.GetComponent<PlayerMovement>().health > 0f)
                        {
                            time_in_state = 0.0f;
                            return state.CHASE;
                        }
                        else
                        {
                            time_in_state = 0.0f;
                            return state.PATROL;
                        }
                    }

                    break;
                }
            case state.LIGHT_ATTACK:
                {
                    if (!grounded)
                    {
                        time_in_state = 0.0f;
                        return state.JUMP;
                    }

                    if (attack_complete)
                    {
                        if (!grounded)
                        {
                            time_in_state = 0.0f;
                            return state.JUMP;
                        }
                        else if (player_in_range)
                        {
                            time_in_state = 0.0f;
                            return state.CHASE;
                        }
                        else
                        {
                            time_in_state = 0.0f;
                            return state.PATROL;
                        }
                    }

                    break;
                }
            case state.HEAVY_ATTACK:
                {
                    if (!grounded)
                    {
                        time_in_state = 0.0f;
                        return state.JUMP;
                    }

                    if (attack_complete)
                    {
                        if (player_in_range)
                        {
                            time_in_state = 0.0f;
                            return state.CHASE;
                        }
                        else
                        {
                            time_in_state = 0.0f;
                            return state.PATROL;
                        }
                    }

                    break;
                }
        }
        return current_state;
    }
}