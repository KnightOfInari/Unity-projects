using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour {

    const float locomotionAnimationSmoothTime = 0.1f;

    public AnimationClip replaceableAttackAnimation;
    public AnimationClip[] defaultAttackAnimSet;
    protected AnimationClip[] currentAttackAnimSet;

    NavMeshAgent agent;
    protected Animator animator;
    protected CharacterCombat combat;
    protected AnimatorOverrideController overrideController;


	protected virtual void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        combat = GetComponent<CharacterCombat>();

        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;

        currentAttackAnimSet = defaultAttackAnimSet;
        combat.OnAttack += OnAttack;
	}
	
	// Update is called once per frame
	protected virtual void Update ()
    {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("speedPercent", speedPercent, 0.1f, Time.deltaTime);

        animator.SetBool("inCombat", combat.InCombat);
	}

    protected virtual void OnAttack()
    {
        animator.SetTrigger("attack");
        int attackIndex = Random.Range(0, currentAttackAnimSet.Length);
        overrideController[replaceableAttackAnimation] = currentAttackAnimSet[attackIndex];
    }
}
