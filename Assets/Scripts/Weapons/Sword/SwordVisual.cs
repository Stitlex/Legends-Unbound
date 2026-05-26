using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class SwordVisual : MonoBehaviour
{
    [SerializeField] private Sword sword;
    private Animator animator;
    private const string ATTACK = "Attack";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        sword.OnSwordSwing += Sword_OnSwordSwing;
    }

    private void OnDestroy()
    {
        if (sword != null)
        {
            sword.OnSwordSwing -= Sword_OnSwordSwing;
        }
    }

    public string IsAttack() => ATTACK;

    private void Sword_OnSwordSwing(object sender, System.EventArgs e) => animator.SetTrigger(ATTACK);

    public void TriggerStartAttackAnimation() => sword.AttackColliderTurnOn();
    public void TriggerEndAttackAnimation() => sword.AttackColliderTurnOff();
}
