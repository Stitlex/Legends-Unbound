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
        bow.OnBowAttack += BowOnBowAttack;
    }

    private void OnDestroy()
    {
        if (bow != null)
        {
            bow.OnBowAttack -= BowOnBowAttack;
        }
    }

    public string IsShoot() => SHOOT;

    private void BowOnBowAttack(object sender, System.EventArgs e)
    {
        animator.SetTrigger(SHOOT);
    }

    public void TriggerFireArrow()
    {
        bow.FireArrow();
    }
}