using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class BowVisual : MonoBehaviour
{
    [SerializeField] private Bow bow;
    private Animator animator;
    private const string SHOOT = "Shoot";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        bow.OnBowAttack += Bow_OnBowAttack;
    }

    private void OnDestroy()
    {
        if (bow != null)
        {
            bow.OnBowAttack -= Bow_OnBowAttack;
        }
    }

    public string IsShoot() => SHOOT;

    private void Bow_OnBowAttack(object sender, System.EventArgs e)
    {
        animator.SetTrigger(SHOOT);
    }

    public void TriggerFireArrow()
    {
        bow.FireArrow();
    }
}