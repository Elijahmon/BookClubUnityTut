using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour  
{
    public enum FireMode { SINGLE, BURST, RAPID };

    #region References
    [SerializeField]
    Rigidbody2D _rigidBody;
    [SerializeField]
    Collider2D _coll;
    [SerializeField]
    Animator _anim;
    [SerializeField]
    GameObject _bulletPrefab;
    [SerializeField]
    Transform _bulletPooler;
    [SerializeField]
    GameObject _bulletCasingPrefab;
    [SerializeField]
    Transform _bulletSpawnPointL;
    [SerializeField]
    Transform _bulletSpawnPointR;
    [SerializeField]
    Transform _bulletCasingSpawnPoint;
    [SerializeField]
    ChaseCamera _cam;
    [SerializeField]
    CameraShake _camShaker;
    [SerializeField]
    ParticleSystem smokeParticle;
    [SerializeField]
    float smokeSpawnX;
    #endregion

    #region Stats
    [SerializeField]
    int health;
    [SerializeField]
    int damage;
    [SerializeField]
    int moveSpeed;
    [SerializeField]
    int jumpHeight;
    [SerializeField]
    float groundedDistance;
    [SerializeField]
    float shootCooldownLength;
    [SerializeField]
    int bulletCap;
    [SerializeField]
    float burstSprayMaxAngle;
    [SerializeField]
    float RapidSprayMaxAngle;
    [SerializeField]
    float singleShakeMax;
    [SerializeField]
    float burstShakeMax;
    [SerializeField]
    float autoShakeMax;
    [SerializeField]
    float shakeDuration;
    #endregion

    #region Collision
    [SerializeField]
    LayerMask groundMask;
    [SerializeField]
    LayerMask enemyMask;
    #endregion

    #region PlayerState
    bool alive;
    bool grounded = true;
    float colliderHeight;
    bool shooting;
    int currentDirection = 1;
    List<BulletController> bulletPool;
    FireMode currentFireMode;
    bool changeFireOldState;
    #endregion

    public void Init()
    {
        colliderHeight = _coll.bounds.size.y;
        bulletPool = new List<BulletController>();

        for (int i = bulletCap; i > 0; i--)
        {
            BulletController b = Instantiate<GameObject>(_bulletPrefab, _bulletPooler).GetComponent<BulletController>();
            b.Init(this);
            bulletPool.Add(b);
        }
        UIController.instance.UpdateFireMode(currentFireMode);
        changeFireOldState = false;

        smokeParticle.Stop();
    }

    #region Input
    /// <summary>
    /// Handles movement input
    /// </summary>
    /// <param name="input"></param>
    public void ProcessMovementInput(float input)
    {
        if (shooting == false)
        {
            if (grounded)
            {
                if (Mathf.Abs(input) > 0)
                {
                    _rigidBody.velocity = new Vector2(input * moveSpeed, _rigidBody.velocity.y);
                    _anim.SetBool("Moving", true);
                    currentDirection = input > 0 ? 1 : -1;
                    _anim.SetInteger("Direction", currentDirection);
                    _cam.UpdateCamTarget(currentDirection);
                }
                else
                {
                    _anim.SetBool("Moving", false);
                }
            }
        }
        else
        {
            _anim.SetBool("Moving", false);
        }
    }

    /// <summary>
    /// Handles input for jumping
    /// </summary>
    /// <param name="input">true to jump</param>
    public void ProcessJumpInput(bool input)
    {
        
        if (grounded && input)
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _rigidBody.velocity.y + jumpHeight);
            grounded = false;
        }
    }

    /// <summary>
    /// Handles input for shooting
    /// </summary>
    /// <param name="input">true to shoot</param>
    public void ProcessShootInput(bool input)
    {
        if (input)
        {
            shooting = true;
        }
        else
        {
            shooting = false;
        }
        _anim.SetBool("Shooting", shooting);
    }

    public void ProcessChangeFireModeInput(bool input)
    {
        if(changeFireOldState != input && input) //Is the button pressed down without being previously pressed down
        {
            
            if (currentFireMode == FireMode.RAPID)
            {
                currentFireMode = FireMode.SINGLE;
            }
            else
            {
                currentFireMode += 1;
            }
            _anim.SetInteger("FireMode", (int)currentFireMode);
            UIController.instance.UpdateFireMode(currentFireMode);
            //Debug.Log("Changing Firemode");
        }
        changeFireOldState = input;
    }
    #endregion

    #region State
    /// <summary>
    /// Checks if the player is on the ground
    /// </summary>
    /// <returns>true if they are on the ground</returns>
    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - (colliderHeight/2)), Vector2.down, groundedDistance, groundMask);
        //Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - (colliderHeight / 2)), Vector2.down * groundedDistance, Color.blue, 5f);
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Fires a single bullet from the corresponding spawn point
    /// </summary>
    public void FireBullet()
    {
        Transform bulletSpawnPoint = currentDirection > 0 ? _bulletSpawnPointR : _bulletSpawnPointL;
        BulletController bullet;

        if (bulletPool.Count > 0)
        {
            bullet = bulletPool[0];
            bulletPool.Remove(bullet);
            bullet.transform.position = bulletSpawnPoint.transform.position;
        }
        else
        {
            bullet = Instantiate<GameObject>(_bulletPrefab).GetComponent<BulletController>();
            bullet.transform.position = bulletSpawnPoint.transform.position;
        }

        float sprayAngle = 0; //Single fire mode has no spray
        switch (currentFireMode)
        {
            case FireMode.SINGLE:
                _camShaker.ActivateScreenShake(singleShakeMax * currentDirection, singleShakeMax, shakeDuration); //Shake backwards from player direction
                break;
            case FireMode.BURST:
                sprayAngle = Random.Range(-burstSprayMaxAngle, burstSprayMaxAngle);
                _camShaker.ActivateScreenShake(burstShakeMax * currentDirection, (sprayAngle * burstShakeMax), shakeDuration); //Shake at the inverse of the spray angle
                break;
            case FireMode.RAPID:
                sprayAngle = Random.Range(-RapidSprayMaxAngle, RapidSprayMaxAngle);
                _camShaker.ActivateScreenShake(autoShakeMax * currentDirection, (sprayAngle * autoShakeMax), shakeDuration, true); //Shake at the inverse of the spray angle (allow down shake
                break;
        }

        bullet.Activate(_bulletPooler, bulletPool, new Vector2(currentDirection, sprayAngle));
        smokeParticle.Play();
    }

    public void HandleDeadFrame()
    {
        _anim.speed = 0;
        GameStateManager.instance.GameOver();
    }

    public void TakeDamage(EnemyController source)
    {
        int incomingDamage = source.GetDamage();

        if (incomingDamage >= health)
        {
            health = 0;
            Die();
        }
        else
        {
            health -= incomingDamage;
        }
    }

    void Die()
    {
        alive = false;
        _anim.SetBool("Dead", true);
        _anim.SetBool("Shooting", true);
        _anim.SetBool("Moving", true);
    }

    public void Despawn()
    {
        Destroy(_cam.gameObject);
        Destroy(this.gameObject);
    }
    #endregion

    #region Unity
    void Update()
    {
    }

    void FixedUpdate()
    {
        if(shooting)
        {
            smokeParticle.gameObject.transform.localPosition = new Vector3(smokeSpawnX * currentDirection, smokeParticle.transform.localPosition.y);

        }
        else
        {
            smokeParticle.Stop();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        grounded = IsGrounded();
        //Debug.Log(_coll.name);
    }
    #endregion

    public int GetDamage()
    {
        return damage;
    }

    virtual public bool IsAlive()
    {
        return alive;
    }

}
