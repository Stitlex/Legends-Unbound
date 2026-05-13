using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class SkeletonVisual : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private EnemyEntity enemyEntity;
    [SerializeField] private GameObject enemyShadow;

    private Animator animator;

    private const string IS_RUNNING = "IsRunning";
    private const string TAKE_HIT = "TakeHit";
    private const string IS_DIE = "IsDie";
    private const string CHASING_SPEED_MULTIPLIER = "ChasingSpeedMultiplier";
    private const string ATTACK = "Attack";

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        enemyAI.OnEnemyAttack += enemyAIOnEnemyAttack;
        enemyEntity.OnTakeHit += enemyEntityOnTakeHit;
        enemyEntity.OnDeath += enemyEntityOnDeath;
    }

    private void Update()
    {
        animator.SetBool(IS_RUNNING, enemyAI.IsRunning);
        animator.SetFloat(CHASING_SPEED_MULTIPLIER, enemyAI.GetRoamingAnimationSpeed());
    }

    private void enemyEntityOnDeath(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_DIE, true);
        spriteRenderer.sortingOrder = -1;
        enemyShadow.SetActive(false);
    }

    private void enemyEntityOnTakeHit(object sender, System.EventArgs e) => animator.SetTrigger(TAKE_HIT);

    private void OnDestroy()
    {
        enemyAI.OnEnemyAttack -= enemyAIOnEnemyAttack;
        enemyEntity.OnTakeHit -= enemyEntityOnTakeHit;
        enemyEntity.OnDeath -= enemyEntityOnDeath;
    }

    

    public void TriggerAttackAnimationTurnOff() => enemyEntity.PolygonColliderTurnOff();
    public void TriggerAttackAnimationTurnOn() => enemyEntity.PolygonColliderTurnOn();

    private void enemyAIOnEnemyAttack(object sender, System.EventArgs e) => animator.SetTrigger(ATTACK);
}
