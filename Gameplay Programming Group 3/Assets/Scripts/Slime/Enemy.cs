using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyPhase;

    public GameObject path;
    Transform[] pathNodes;

    GameObject nextSplitterEnemy;

    GameObject player;

    float dist;

    public float speed;

    public float bounceRate;

    float s;

    public float jumpForce;

    bool has_hit;

    float health;

    int attack_damage;

    GameObject healthBar;

    bool isChasing;
    float nodeDist;
    int currentNode;

    public float chase_range;
    public float hit_range;

    public float nodeRange;

    float colTimer;

    public Material start_mat;
    public Material hit_mat;
    bool hit;
    float hitTime;

    bool invunerable;
    float invTime;

    bool dead;
    float deadtime;

    Vector3 statScale;

    bool hasBouncedTowards;
    float bounceTime;

    bool grounded;

    float t;

    bool randBounceSet;

    float rcptTime;

    Vector3 e2;
    float groundTimer;

    public ParticleSystem jumpFX;
    float jfxT;
    bool gap;

    float stime;

    //public int[] near;

    // Use this for initialization
    void Start()
    {
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        jumpFX = Instantiate(jumpFX);
        jumpFX.transform.parent = gameObject.transform;
        jumpFX.transform.position = gameObject.transform.position + new Vector3(0, -0.5f, 0);
        this.jumpFX.Stop();

        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        statScale = transform.localScale;

        GetComponent<Renderer>().material = start_mat;

        player = GameObject.FindGameObjectWithTag("Player");

        healthBar = transform.Find("healthCanvas").gameObject;
        healthBar.GetComponent<Canvas>().enabled = false;

        nextSplitterEnemy = this.gameObject;
        //	nextSplitterEnemy = Resources.Load("enemySlime") as GameObject;

        //set variables based on phases
        if (enemyPhase == 1)
        {
            health = 3;
            GetComponent<enemyHealthBar>().setMaxValue(3);
        }
        if (enemyPhase == 2)
        {
            hit_range -= 0.2f;
            health = 2;
            GetComponent<enemyHealthBar>().setMaxValue(2);
        }
        if (enemyPhase == 3)
        {
            hit_range -= 0.3f;
            health = 1;
            GetComponent<enemyHealthBar>().setMaxValue(1);
        }

        //reduce enemy scale for each phase
        if (enemyPhase > 1)
        {
            gameObject.transform.localScale -= new Vector3(0.35f, 0.35f, 0.35f);
        }

        if (enemyPhase > 2)
        {
            gameObject.transform.localScale -= new Vector3(0.35f, 0.35f, 0.35f);
        }

        //gets and sets patrol path;
        if (path)
        {
            pathNodes = new Transform[path.GetComponent<patrolPath>().path.Length];

            for (int i = 0; i < path.GetComponent<patrolPath>().path.Length; i++)
            {
                pathNodes[i] = path.GetComponent<patrolPath>().path[i].transform;
            }
        }

        setNearestNode();
    }

    void Update()
    {
        Grounded();

        Vector3 down = transform.TransformDirection(Vector3.down);

        Debug.DrawRay(transform.position, down, Color.red);

        if (Physics.Raycast(transform.position + new Vector3(0, 0.01f, 0) + (transform.forward * 2), down, 5f))
        {
            //null
            gap = false;
        }
        else
        {
            //  GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            gap = true;
        }
    }

    void FixedUpdate()
    {
        if (jumpFX.isPlaying)
        {
            jfxT += Time.deltaTime;

            if (jfxT > 0.7f)
            {
                jumpFX.Stop();
            }
        }

        if (hasBouncedTowards)
        {

        }

        bounceTime += Time.deltaTime;

        if (dead)
        {
            GetComponent<Renderer>().material = hit_mat;
            invunerable = true;
            deadtime += Time.deltaTime;

            if (transform.localScale.y > 0f)
            {
                transform.localScale -= new Vector3(0, 1, 0) * deadtime / 7.5f;
            }
            else
            {
                die();
            }
        }

        if (invunerable)
        {
            invTime += Time.deltaTime;

            if (invTime > 0.7f)
            {
                invunerable = false;
                invTime = 0;
            }
        }

        if (hit)
        {
            hitTime += Time.deltaTime;

            if (hitTime > 0.2f)
            {
                GetComponent<Renderer>().material = start_mat;
                hitTime = 0;
                hit = false;
            }
        }

        GetComponent<enemyHealthBar>().health = health;

        s = speed * Time.deltaTime;
        dist = Vector3.Distance(transform.position, player.transform.position);

        if (dist < chase_range)
        {
            //take damage test

            isChasing = true;

            healthBar.GetComponent<Canvas>().enabled = true;

            Vector3 l = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.LookAt(l);


            //transform.position = Vector3.MoveTowards (transform.position, player.transform.position, s);

            t = Random.Range(1, 5);

            if (bounceTime > t && grounded == true && gap == false)
            {
                float f = Random.Range(2, 4);

                Vector3 dir = transform.position + player.transform.position;
                dir.Normalize();
                GetComponent<Rigidbody>().velocity = (transform.forward * f) + (Vector3.up * jumpForce);

                jumpFX.Play();

                bounceTime = 0;
            }


            if (dist < hit_range && grounded == true && gap == false)
            {
                //attack player
                if (!(player.GetComponent<PlayerMovement>().current_state == PlayerMovement.state.DEAD) && stime == 0)
                {

                    stime = 1;
                    if (enemyPhase == 1)
                    {
                        player.GetComponent<PlayerMovement>().TakeDamage(3);
                    }
                    else if (enemyPhase == 2)
                    {
                        player.GetComponent<PlayerMovement>().TakeDamage(2);
                    }
                    else if (enemyPhase == 3)
                    {
                        player.GetComponent<PlayerMovement>().TakeDamage(1);
                    }

                    Vector3 dir = transform.position - player.transform.position;
                    dir.Normalize();
                    GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + dir * 3.5f + Vector3.up * (jumpForce);

                    jumpFX.Play();
                    bounceTime = 0;

                }
            }
        }
        else
        {
            isChasing = false;
            healthBar.GetComponent<Canvas>().enabled = false;
        }

        if (stime > 0)
        {
            stime -= Time.deltaTime;

            if (stime <= 0)
            {
                stime = 0;
            }
        }

        //on death - depending on phase, split/instatiate accordingly
        if (health <= 0)
        {
            dead = true;
            //die ();
        }

        if (colCheck() == true && grounded == true && gap == false)
        {
            Vector3 dir = transform.position - player.transform.position;
            dir.Normalize();
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + dir + Vector3.up * (jumpForce / 10);
            jumpFX.Play();
        }

        //checks down and jumps random direction;
        if (downColCheck() == true && grounded == true && gap == false)
        {
            Vector3 dir = Random.insideUnitCircle.normalized;
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + dir + Vector3.up * (jumpForce / 10);
            jumpFX.Play();
        }


        if (isChasing == false)
        {
            pathing();
        }
        else
        {
            setNearestNode();
        }

        if (groundTimer > 7)
        {
            Vector3 dir = new Vector3(Random.Range(0, 1), 0, Random.Range(0, 1)).normalized;

            rcptTime = 0;
            GetComponent<Rigidbody>().velocity = -transform.forward * (jumpForce) + Vector3.up * (jumpForce / 3);
            jumpFX.Play();
            groundTimer = 0;
        }
    }

    void LateUpdate()
    {
        if (path)
            Debug.DrawLine(transform.position, pathNodes[currentNode].transform.position, Color.yellow);


    }

    bool colCheck()
    {
        //checks if front is hit and theres nothing directly behind and moves back - recuces bunching
        RaycastHit hit;
        Vector3 f = transform.TransformDirection(Vector3.forward);
        Vector3 b = transform.TransformDirection(-Vector3.forward);

        Debug.DrawRay(transform.position, f, Color.red);
        Debug.DrawRay(transform.position, b, Color.red);

        if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), f, out hit, 0.5f))
        {
            if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), b, out hit, 1f))
            {
                return false;
            }

            return true;
        }

        return false;
    }

    bool downColCheck()
    {
        //checks if down is hit as player or enemy - reduces getting stuck ontop of things
        RaycastHit hit;
        Vector3 d = transform.TransformDirection(-Vector3.up);
        Vector3 u = transform.TransformDirection(Vector3.up);

        Debug.DrawRay(transform.position, d, Color.red);
        Debug.DrawRay(transform.position, u, Color.red);

        if (Physics.Raycast(transform.position + new Vector3(0, 0.01f, 0), d, out hit, 1f))
        {
            if (hit.transform.tag == "Player" || hit.transform.tag == "Enemy")
            {
                if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), u, out hit, 1f))
                {
                    return false;
                }
                //	Debug.Log("down hit");
                return true;
            }
        }

        return false;

    }

    void die()
    {
        if (enemyPhase == 1)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject next = Instantiate(nextSplitterEnemy);
                next.GetComponent<Enemy>().enemyPhase = 2;
                next.GetComponent<Enemy>().path = path;

                next.GetComponent<Enemy>().setInvunrability();

                if (i == 0)
                    next.transform.position = transform.position + new Vector3(0.75f, 0, 0);

                if (i == 1)
                    next.transform.position = transform.position - new Vector3(0.75f, 0, 0);

            }
            Destroy(gameObject);
        }
        else if (enemyPhase == 2)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject next = Instantiate(nextSplitterEnemy);
                next.GetComponent<Enemy>().enemyPhase = 3;
                next.GetComponent<Enemy>().path = path;

                next.GetComponent<Enemy>().setInvunrability();

                if (i == 0)
                    next.transform.position = transform.position;

                if (i == 1)
                    next.transform.position = transform.position - new Vector3(1f, 0, 0);

                if (i == 2)
                    next.transform.position = transform.position + new Vector3(1f, 0, 0);
            }
            Destroy(gameObject);
            dead = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void pathing()
    {
        if (path)
        {

            if (currentNode >= pathNodes.Length)
            {
            }

            Vector3 e = new Vector3(pathNodes[currentNode].transform.position.x, transform.position.y, pathNodes[currentNode].transform.position.z);

            if (!returnToPatrolCheck())
            {
                rcptTime += Time.deltaTime;

                if (rcptTime > 4)
                {
                    e2.x = Random.Range(transform.position.x - 10, transform.position.x + 10);
                    e2.z = Random.Range(transform.position.z - 10, transform.position.z + 10);
                    rcptTime = 0;
                }

                e.x = e2.x;
                e.z = e2.z;
            }

            Debug.DrawLine(transform.position + new Vector3(0, 0.01f, 0), e, Color.magenta);

            if (transform.position != e && !nodeCollisionCheck())
            {

                Quaternion r = Quaternion.LookRotation((e - transform.position).normalized);
                transform.rotation = Quaternion.Lerp(transform.rotation, r, Time.deltaTime * 2);

                if (randBounceSet == false)
                {
                    t = Random.Range(0.1f, 2f);
                }

                randBounceSet = true;

                if (bounceTime > bounceRate - t && grounded == true && gap == false)
                {
                    float f = Random.Range(2, 4);

                    Vector3 dir = transform.position + player.transform.position;
                    dir.Normalize();
                    GetComponent<Rigidbody>().velocity = transform.forward * (speed - f) + Vector3.up * jumpForce;

                    jumpFX.Play();

                    bounceTime = 0;
                    randBounceSet = false;
                }
            }
            else
            {
                int[] near = new int[100];
                int l = 0;

                for (int i = 0; i < pathNodes.Length; i++)
                {
                    float d = Vector3.Distance(new Vector3(pathNodes[i].transform.position.x, transform.position.y, pathNodes[i].transform.position.z), transform.position);

                    if (d < nodeRange && l < near.Length)
                    {
                        near[l] = i;
                        l++;
                    }
                }
                int r = Random.Range(0, l);
                currentNode = near[r];
            }
        }
        else
        {

            Vector3 e = transform.position;

            rcptTime += Time.deltaTime;

            if (rcptTime > 4)
            {
                e.x = Random.Range(transform.position.x - 10, transform.position.x + 10);
                e2.z = Random.Range(transform.position.z - 10, transform.position.z + 10);
                rcptTime = 0;
            }

            e.x = e2.x;
            e.z = e2.z;


            Debug.DrawLine(transform.position + new Vector3(0, 0.01f, 0), e, Color.magenta);

            Quaternion r = Quaternion.LookRotation((e - transform.position).normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, r, Time.deltaTime * 2);

            if (randBounceSet == false)
            {
                t = Random.Range(0.1f, 2f);
            }

            randBounceSet = true;

            if (bounceTime > bounceRate - t && grounded == true && gap == false)
            {
                float f = Random.Range(2, 4);

                Vector3 dir = transform.position + player.transform.position;
                dir.Normalize();
                GetComponent<Rigidbody>().velocity = transform.forward * (speed - f) + Vector3.up * (jumpForce);

                jumpFX.Play();

                bounceTime = 0;
                randBounceSet = false;
            }
        }
    }

    void setNearestNode()
    {
        if (path)
        {
            float bestDist = Vector3.Distance(transform.position, new Vector3(pathNodes[0].transform.position.x, transform.position.y, pathNodes[0].transform.position.z));
            currentNode = 0;

            for (int i = 0; i < pathNodes.Length; i++)
            {
                float d = Vector3.Distance(new Vector3(pathNodes[i].transform.position.x, transform.position.y, pathNodes[i].transform.position.z), transform.position);

                if (d < bestDist)
                {
                    bestDist = d;
                    currentNode = i;
                }
            }
        }
    }

    bool nodeCollisionCheck()
    {
        colTimer += Time.deltaTime;

        RaycastHit hit;
        Vector3 f = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(transform.position + new Vector3(0, 0.01f, 0), f, out hit, 10f) && colTimer > 1f)
        {
            colTimer = 0;
            return true;
        }

        return false;
    }

    public void getHit(float damage)
    {
        Vector3 dir = transform.position - player.transform.position;
        dir.Normalize();
        GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + dir * 3 + Vector3.up * (jumpForce / 5);

        jumpFX.Play();
        bounceTime = 0;

        if (invunerable == false)
        {
            health -= damage;
            GetComponent<Renderer>().material = hit_mat;
            hit = true;
            hitTime = 0;

            setInvunrability();

        }
    }

    public void setInvunrability()
    {
        invunerable = true;
    }

    void moveBounce()
    {
        if (grounded)
        {
            Vector3 dir = transform.position + player.transform.position;
            dir.Normalize();
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + dir + Vector3.up * (jumpForce / 10);

            jumpFX.Play();

            bounceTime = 0;
        }

    }
    void Grounded()
    {

        Vector3 down = transform.TransformDirection(Vector3.down);

        Debug.DrawRay(transform.position, down, Color.red);

        if (Physics.Raycast(transform.position + new Vector3(0, 0.01f, 0), down, 0.85f))
        {
            grounded = true;

            groundTimer = 0;

        }
        else
        {
            grounded = false;
            groundTimer += Time.deltaTime;
        }
    }


    bool returnToPatrolCheck()
    {
        if (Physics.Linecast(transform.position + new Vector3(0, 0.01f, 0), pathNodes[currentNode].transform.position))
        {
            return false;
        }
        return true;
    }
}