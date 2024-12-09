using brolive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_Animator_For_Enemy : MonoBehaviour
{
    private Animator _anim;
    private readonly int hashAttack = Animator.StringToHash("Attack");
    private readonly int hashIsChase = Animator.StringToHash("isChasing");
    private readonly int hashAttackPattern = Animator.StringToHash("attackPattern");
    private readonly int hashDie = Animator.StringToHash("isDie");
    private readonly float[] attackPatternDuration = {1.5f, 1.5f, 1f};
    private readonly float attackAnimationSpeedScale = 4;

    private Actor _actor;
    private Adjusted_Damageable _damageable;
    private Adjusted_EnemyTest _enemyTest;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _actor = GetComponent<Actor>();
        _damageable = GetComponent<Adjusted_Damageable>();
        _enemyTest = GetComponent<Adjusted_EnemyTest>();
    }

    public float Attack()
    {
        int randomAttackPattern = (int)Random.Range(0, 3);
        _anim.SetInteger(hashAttackPattern, randomAttackPattern);
        _anim.SetTrigger(hashAttack);
        return(attackPatternDuration[randomAttackPattern] / attackAnimationSpeedScale);
    }
    public void SetChase(bool value)
    { 
        _anim.SetBool(hashIsChase, value);
    }
    public void Die(bool value)
    { 
        _anim.SetBool(hashDie, value);

        if (value)
        {
            _actor.enabled = false;
            _damageable.enabled = false;
            _enemyTest.enabled = false;
        }
    }
}
