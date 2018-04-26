using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private Animator player_animator;
    private Rigidbody rigidbody_player;
    private GameObject main_camera;

    private bool on_ground = false;
    private bool run = true;
    private bool head_on_roof = false;
    public state current_state = state.GROUND;

    private float speed_limit = 1.0f;
    private float current_speed = 1.0f;
    public float previous_rotation = 0.0f;
    private float ground_angle = 0.0f;
    private float pause_time = 0.0f;

    public float spline_rotation = 0.0f;
    
    private bool double_jump = false;
    public float speed_boost = 0.0f;

    public bool hasDoubleJumped = false;

    public ParticleSystem pickup_particle;
    public ParticleSystem pickup_current;

    [Range(0.0f, 2.0f)]
    public float air_speed_modifier = 1.0f;
    private float air_speed = 1.0f;
    private bool slowed = false;
    public Vector3 air_launch;

    public bool in_spline = false;

    public float health = 0.0f;
    public float max_health = 10.0f;

    public Attack attackTargets;

    static public bool djump_unlocked = false;
    static public bool boost_unlocked = false;
    float boost_cool_time = 0;

    public enum state
    {
        AIR,
        GROUND,
        ATTACK,
        HEAVY_ATTACK,
        MENU,
        PAUSED,
        DEAD
    }

    public void unlockDoubleJump()
    {
        djump_unlocked = true;
    }

    public void unlockBoost()
    {
        boost_unlocked = true;
    }

    private void ResetScene()
    {
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);
    }

    public void Slow(float duration)
    {
        slowed = true;
        Invoke("Unslow", duration);
    }

    private void Unslow()
    {
        slowed = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(0.2f, damage);

        if (health <= 0.0f)
        {
            air_launch = new Vector3(air_launch.x, air_launch.y * 0.5f, air_launch.z);
        }
        else
        {
            player_animator.SetBool("Damaged", true);
        }
    }

    void HitLight()
    {
        foreach (GameObject enemy in attackTargets.enemies)
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(0.2f, 0.5f);

            if ((enemy.GetComponent("SlimeBehaviour") as SlimeBehaviour) != null)
            {
                enemy.GetComponent<SlimeBehaviour>().TakeDamage(2.0f);
            }
            if ((enemy.GetComponent("Enemy") as Enemy) != null)
            {
                enemy.GetComponent<Enemy>().getHit(1.0f);
            }
            return;
        }
    }

    void HitHeavy()
    {
        foreach (GameObject enemy in attackTargets.enemies)
        {
            if ((enemy.GetComponent("SlimeBehaviour") as SlimeBehaviour) != null)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(0.2f, 1.0f);
                enemy.GetComponent<SlimeBehaviour>().TakeDamage(4.0f);
            }
            if ((enemy.GetComponent("Enemy") as Enemy) != null)
            {
                enemy.GetComponent<Enemy>().getHit(2.0f);
            }
        }
    }

    public bool DoubleJump()
    {
        if(double_jump)
        {
            return false;
        }
        else
        {
            double_jump = true;

            return true;
        }
    }

    public void SpeedBoost()
    {
        speed_boost += 5.0f;

        GameObject[] par =  GameObject.FindGameObjectsWithTag("SpeedBoost");

        foreach(GameObject p in par)
        {
            p.GetComponent<ParticleSystem>().Play();
        }
    }
    
    void OnCollisionEnter(Collision col)
    {
        if (col.contacts.Length > 0)
        {
            ContactPoint contact = col.contacts[0];

            if (Vector3.Dot(contact.normal, Vector3.up) > 0.9f)
            {
                on_ground = true;
            }

            if (col.collider.tag != "Enemy")
            {
                air_launch = Vector3.zero;
            }
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.contacts.Length > 0)
        {
            ContactPoint contact = col.contacts[0];

            if (Vector3.Dot(contact.normal, Vector3.up) > 0.1f)
            {
                on_ground = true;

                ground_angle = Vector3.Angle(Vector3.up, contact.normal);
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (!Physics.Raycast(transform.position, Vector3.down, 0.4f))
        {
            on_ground = false;
        }
    }

    void Start ()
    {
        player_animator = GetComponent<Animator>();
        rigidbody_player = GetComponent<Rigidbody>();
        main_camera = GameObject.FindGameObjectWithTag("MainCamera");

        health = max_health;
    }

    void Update()
    {
        StateCheck();
        switch (current_state)
        {
            case state.GROUND:
                ProximityRaycastCheck();
                Jump();
                GroundMovement();
                Turning();
                break;

            case state.AIR:
                ProximityRaycastCheck();
                Turning();
                AirMovement();
                AirJump();
                break;

            case state.ATTACK:
                previous_rotation = transform.rotation.eulerAngles.y;
                Attack();
                break;

            case state.HEAVY_ATTACK:
                previous_rotation = transform.rotation.eulerAngles.y;
                Attack();
                break;

            case state.PAUSED:
                previous_rotation = transform.rotation.eulerAngles.y;
                current_speed = Mathf.Lerp(current_speed, 0, 0.1f);
                player_animator.SetFloat("Speed", current_speed);
                player_animator.SetBool("Ground Hit", true);
                player_animator.SetBool("Jump", false);
                break;
            case state.DEAD:
                Invoke("ResetScene", 5.0f);
                break;
        }

        if (health < max_health / 2)
        {
            health += Time.deltaTime * 0.2f;
        }
        else
        {
            health = max_health / 2;
        }
    }

    void AirJump()
    {
        if(double_jump || djump_unlocked && !hasDoubleJumped)
        {
            if (Input.GetButtonDown("Jump"))
            {
                double_jump = false;
                on_ground = false;

                hasDoubleJumped = true;

                player_animator.SetBool("Jump", true);

                rigidbody_player.velocity = new Vector3(0, 0, 0);
                rigidbody_player.AddForce(new Vector3(0, 7, 0), ForceMode.Impulse);
                transform.position += new Vector3(0, 0.1f, 0);

                air_speed = Mathf.Clamp(current_speed, 0.4f, 1.5f);

                GameObject[] par = GameObject.FindGameObjectsWithTag("DoubleJump");

                foreach (GameObject p in par)
                {
                    p.GetComponent<ParticleSystem>().Play();
                }

                pickup_current.Stop();
            }
        }
    }

    void ProximityRaycastCheck()
    {
        if (on_ground)
        {
            player_animator.SetBool("Ground Hit", true);
            hasDoubleJumped = false;
        }
        else
        {
            if (!Physics.Raycast(transform.position, Vector3.down, 0.4f))
            {
                player_animator.SetBool("Ground Hit", false);
            }
        }

        if (Physics.Raycast(transform.position + new Vector3(0, 2.6f, 0), Vector3.up, 1.0f))
        {
            head_on_roof = true;
        }
        else
        {
            head_on_roof = false;
        }
    }

    public void Pause(float time)
    {
        pause_time = time;
    }

    void StateCheck()
    {
        if (on_ground)
        {
            current_state = state.GROUND;
        }
        else
        {
            current_state = state.AIR;
        }

        if (player_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack-Alt") ||
           player_animator.GetAnimatorTransitionInfo(0).IsName("Idle -> Attack") ||
           player_animator.GetAnimatorTransitionInfo(0).IsName("Locomotion -> Attack")||
           player_animator.GetAnimatorTransitionInfo(0).IsName("Attack -> Attack-Alt"))
        {
            current_state = state.ATTACK;
        }

        if (player_animator.GetCurrentAnimatorStateInfo(0).IsName("Heavy Attack") ||
            player_animator.GetCurrentAnimatorStateInfo(0).IsName("Heavy Attack Alt") ||
           player_animator.GetAnimatorTransitionInfo(0).IsName("Idle -> Heavy Attack") ||
           player_animator.GetAnimatorTransitionInfo(0).IsName("Locomotion -> Heavy Attack") ||
           player_animator.GetAnimatorTransitionInfo(0).IsName("Attack -> Heavy Attack Alt"))
        {
            current_state = state.ATTACK;
        }

        player_animator.SetBool("Attack", Input.GetButtonDown("Attack"));
        
        player_animator.SetBool("Heavy Attack", Input.GetButtonDown("Heavy Attack"));

        if (Mathf.Round(air_launch.y * 10) != 0)
        {
            current_state = state.AIR;
            on_ground = false;
        }
        
        if (pause_time > 0.0f)
        {
            pause_time -= Time.deltaTime;

            if (pause_time < 0.0f)
                pause_time = 0.0f;

            current_state = state.PAUSED;
        }

        if(health <= 0.0f && current_state == state.GROUND)
        {
            current_state = state.DEAD;
            player_animator.SetBool("Dead", true);
        }

        if(player_animator.GetCurrentAnimatorStateInfo(1).IsName("DamageTaken"))
        {
            player_animator.SetBool("Damaged", false);
        }
    }

    void Attack()
    {        
        rigidbody_player.AddForce(air_launch * 200);
        air_launch = air_launch * 0.99f;
        air_launch = new Vector3(air_launch.x, air_launch.y * 0.5f, air_launch.z);
    }

    void Jump()
    {
        if (Input.GetButton("Jump"))
        {
            if(on_ground)
            {
                if (!head_on_roof)
                {
                    if ((player_animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion") ||
                         player_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) &&
                       !(player_animator.GetAnimatorTransitionInfo(0).IsName("Idle -> Jumping") ||
                         player_animator.GetAnimatorTransitionInfo(0).IsName("Locomotion -> Jumping")))
                    {
                        on_ground = false;
                        player_animator.SetBool("Jump", true);

                        rigidbody_player.velocity = new Vector3(0, 0, 0);

                        rigidbody_player.AddForce(new Vector3(0, 7, 0), ForceMode.Impulse);

                        transform.position += new Vector3(0, 0.1f, 0);

                        air_speed = Mathf.Clamp(current_speed, 0.4f, 1.5f);
                    }
                }
            }
            
        }
        else
        {
            player_animator.SetBool("Jump", false);
        }
    }

    void AirMovement()
    {
        float speed = Mathf.Sqrt(Input.GetAxis("Vertical") * Input.GetAxis("Vertical") + Input.GetAxis("Horizontal") * Input.GetAxis("Horizontal"));

        float s = 0;

        if (speed_boost > 0)
        {
            s = air_speed * 1.5f;

            speed_boost -= Time.deltaTime;
        }
        else
        {
            s = air_speed;
        }

        player_animator.speed = 1.0f;

        rigidbody_player.AddRelativeForce(Vector3.forward * speed * 260 * s * air_speed_modifier);

        rigidbody_player.AddForce(air_launch * 200);
        air_launch = air_launch * 0.99f;
        air_launch = new Vector3(air_launch.x, air_launch.y * 0.5f, air_launch.z);
    }

    void GroundMovement()
    {
        float speed = Mathf.Sqrt(Input.GetAxis("Vertical") * Input.GetAxis("Vertical") + Input.GetAxis("Horizontal") * Input.GetAxis("Horizontal"));

        //boost
        if (Input.GetButton("Boost") && speed_boost <= 0 && boost_cool_time <= 0)
        {
            if (boost_cool_time <= 0)
            {
                SpeedBoost();
                boost_cool_time = 10;
            }
        }

        //boost cool down
        if(boost_cool_time > 0)
        {
            boost_cool_time -= Time.deltaTime;
        }

        if (in_spline)
        {
            speed = Mathf.Abs(Input.GetAxis("Horizontal"));
        }

        if(Input.GetButtonDown("Run"))
        {
            run = !run;
        }

        if(run)
        {
            speed_limit = Mathf.Lerp(speed_limit, 1.0f, 0.1f);
        }
        else
        {
            speed_limit = Mathf.Lerp(speed_limit, 0.3f, 0.1f);
        }

        if(ground_angle >= 10.0f)
        {
            speed_limit = Mathf.Lerp(speed_limit, 0.5f, 0.1f);
        }
        if (ground_angle >= 15.0f)
        {
            speed_limit = Mathf.Lerp(speed_limit, 0.3f, 0.1f);
        }

        speed = Mathf.Clamp(speed, 0, speed_limit);

        if (speed_boost > 0.0f)
        {
            player_animator.speed = 1.5f;

            speed_boost -= Time.deltaTime;

            if(speed_boost <= 0.0f)
            {
                GameObject[] par = GameObject.FindGameObjectsWithTag("SpeedBoost");

                foreach (GameObject p in par)
                {
                    p.GetComponent<ParticleSystem>().Stop();
                }
            }
        }
        else
        {
            player_animator.speed = 1.0f;
        }

        current_speed = speed;

        player_animator.SetFloat("Speed", speed);

        air_launch = Vector3.zero;
    }
     
    void Turning()
    {
        float angle = Mathf.Rad2Deg * Mathf.Atan(Mathf.Abs(Input.GetAxis("Horizontal") / Input.GetAxis("Vertical")));
        
        if (in_spline)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                angle = 90;
                angle += spline_rotation;
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                angle = -90;
                angle += spline_rotation;
            }
            else
            {
                angle = previous_rotation;
            }
        }
        else
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                if (Input.GetAxis("Horizontal") <= 0)
                {
                    angle = -angle;
                }
            }
            else
            {
                if (Input.GetAxis("Horizontal") > 0)
                {
                    angle = 180 - angle;
                }
                else if(Input.GetAxis("Horizontal") <= 0)
                {
                    angle = angle - 180;
                }
            }

            angle += GameObject.FindGameObjectWithTag("MainCamera").transform.rotation.eulerAngles.y;

            angle = float.IsNaN(angle) ? previous_rotation : angle;
        }
        
        transform.rotation = Quaternion.Euler(0, angle, 0);

        previous_rotation = angle;
    }
}