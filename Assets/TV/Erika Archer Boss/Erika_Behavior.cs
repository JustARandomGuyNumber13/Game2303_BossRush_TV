using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Erika_Behavior : MonoBehaviour
{
    [Header("Base components")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileLaunchPos;
    [SerializeField] private GameObject KickBox;
    [SerializeField] private ParticleSystem phaseChangeExplosion;

    [Header("Others")]
    [SerializeField] private GameObject Phase1Mobs;
    [SerializeField] private GameObject Phase2Mobs;

    [Header("Stats")]
    [Range(0, 100)][SerializeField] private float rollRate;
    [SerializeField] private float meleeAttackRange;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float rollSpeed;

    [Header("Animator parameters")]
    [SerializeField] private readonly int hashKick = Animator.StringToHash("Kick");
    [SerializeField] private readonly int hashShoot = Animator.StringToHash("Shoot");
    [SerializeField] private readonly int hashSmoke = Animator.StringToHash("Smoke");
    [SerializeField] private readonly int hashIsRoll = Animator.StringToHash("isRoll");
    [SerializeField] private readonly int hashIsDead = Animator.StringToHash("isDead");

    private Transform _transform;
    private Adjusted_Damageable health;
    private Animator _anim;
    private bool isCanAim = true;
    private int phase;
    private int phase1MobsCount = 4;

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////* Monobehavior methods */
    private void Start()
    {
        _transform = transform;
        _anim = GetComponent<Animator>();
        health = GetComponent<Adjusted_Damageable>();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////* Other handlers */
    public void SetIsCanAim(bool value)
    {
        isCanAim = value;
    }
    public void LookAtPlayer()
    {
        if (!isCanAim) return; 
        Vector3 targetPos = player.position;
        targetPos.y = _transform.position.y;
        _transform.LookAt(targetPos);
    }
    public void Rotate()
    {
        _transform.eulerAngles += Vector3.down * Time.deltaTime * rotateSpeed;
    }
    public void Roll()
    {
        _transform.position += _transform.forward * Time.deltaTime * rollSpeed;
    }
    public void DeactivateRolling()
    {
        _anim.SetBool(hashIsRoll, false);
    }
    private void ResetAnimatorParameters()
    { 
        _anim.ResetTrigger(hashShoot);
        _anim.ResetTrigger(hashKick);
        _anim.ResetTrigger(hashSmoke);
        _anim.SetBool(hashIsRoll, false);
    }

    public void Die(bool value)
    {
        _anim.SetBool(hashIsDead, value);
    }
    public void EndGame()
    {
        GameManager.instance.GoToNextLevel();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// /* Phase Change handlers */
    public void SetIsInvulnerable(bool value)
    {
        health.isInvulnerable = value;
    }
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
    public void ChangePhase(int value)
    {
        if (health.GetHealth() == value)
        {
            phase++;
            SetIsInvulnerable(true);
            ResetAnimatorParameters();
            _anim.SetTrigger(hashSmoke);
        }
    }
    public void SpawnMob()
    {
        if(phase == 1)
            Phase1Mobs.SetActive(true);
        else
            Phase2Mobs.SetActive(true);
    }
    public void Phase1Check()
    {
        phase1MobsCount--;
        if (phase1MobsCount == 0)
        {
            SetIsInvulnerable(false);
            SetActive(true);
        }
    }
    public void PlayerPhaseChangeExplosion()
    {
        phaseChangeExplosion.Play();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////* Attack handlers */
    public IEnumerator AttackCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (Vector3.Distance(player.position, _transform.position) < meleeAttackRange)
        {
            _anim.SetTrigger(hashKick);
            yield return new WaitForSeconds(0.6f);
            if (Kick())
                _anim.SetBool(hashIsRoll, true);
        }
        else
        {
            _anim.SetTrigger(hashShoot);
        }
        yield return null;
    }
    public void Shoot()
    {
        GenerateProjectile(false);
        int randomArrowAmount = (int)Random.Range(0, 10);
        for(int i = 0; i < randomArrowAmount; i++) 
            GenerateProjectile(true);
    }
    private void GenerateProjectile(bool isRandom)
    {
        GameObject arrow = Instantiate(projectile, projectileLaunchPos.position, projectileLaunchPos.rotation); // Original projectile generation
        Transform arrowTranform = arrow.transform;
        arrow.SetActive(true);

        if (!isRandom)
        {
            arrowTranform.forward = transform.forward;
            return;
        }

        Vector3 arrowRot = arrowTranform.eulerAngles;   // Other projectiles generation (Same thing but change rotation)
        float randomY = Random.Range(-10, 10);
        arrowRot.y += randomY;
        arrowTranform.eulerAngles = arrowRot;
    }
    private bool Kick() // Return is can roll
    {
        KickBox.SetActive(true);
        Invoke("DisableKickBox", 0.1f);
        float rollChance = Random.Range(0, 101);
        return rollChance < rollRate;
    }
    private void DisableKickBox()
    {
        KickBox.SetActive(false);
    }

    /*Testing area*/
    public void Test()
    {
        //Debug.Log(gameObject.name + " is dead");
    }
}
