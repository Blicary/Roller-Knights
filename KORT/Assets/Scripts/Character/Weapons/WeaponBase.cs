﻿using UnityEngine;
using System.Collections;


public abstract class WeaponBase 
{
    // references
    protected Character owner;
    protected AttackInfoHub attack_info_hub;
    protected CharAimInfoHub aim_info_hub;
    

    // general
    public abstract string WeaponName { get; }


    // time between two attacks (time before another attack can be made) - defines weapon speed
    protected abstract float AttackDuration { get; }

    // timer for attack time (time left before the attack is over)
    private float attack_time_left = 0;
    protected float AttackTimeLeft 
    {
        get { return attack_time_left; }
        set
        {
            attack_time_left = value;
            tempanimator.SetTime(AttackDuration - attack_time_left);
        }
    }


    // animation
    public SpriteRenderer animation_renderer; // on info hub?...
    protected SpriteAnimator tempanimator;
    protected Animator animator;
    



    public WeaponBase()
    {
    }
    public void Initialize(Character owner, AttackInfoHub attack_info_hub, CharAimInfoHub aim_info_hub,
        SpriteRenderer weapon_renderer, Animator animator)
    {
        this.owner = owner;
        this.attack_info_hub = attack_info_hub;
        this.aim_info_hub = aim_info_hub;
        this.animation_renderer = weapon_renderer;
        this.animator = animator;

        tempanimator = new SpriteAnimator(animation_renderer, AttackDuration);
    }

    public virtual void Update()
    {
        if (attack_time_left > 0)
        {
            attack_time_left -= Time.deltaTime;
        }
        if (tempanimator.Update(Time.deltaTime))
        {
            OnAnimationEnd();
        }
    }

    /// <summary>
    /// Will attack if allowed.
    /// </summary>
    public void Attack()
    {
        if (CanAttack()) HandleAttack();
    }
    public virtual void InterruptAttack()
    {
        tempanimator.StopAnimation();
        OnAnimationEnd();
    }

    /// <summary>
    /// Called by WeaponBase when an attack is commanded and CanAttack.
    /// When overiding, be sure the call the original.
    /// </summary>
    protected virtual void HandleAttack()
    {
        attack_time_left = AttackDuration;
        SetAnimationAttacking(true);
    }

    /// <summary>
    /// Has there been enough time since the last attack, etc.
    /// If overiding, be sure to use the return of the original.
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanAttack()
    {
        return !IsAttacking();
    }

    private void SetAnimationAttacking(bool attacking)
    {
        if (!animator) return;
        animator.SetBool("Attacking", attacking);
    }

    protected virtual void OnAnimationEnd()
    {
        SetAnimationAttacking(false);
    }

    public bool IsAttacking()
    {
        return attack_time_left > 0;
    }

}
