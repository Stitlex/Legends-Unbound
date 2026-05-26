using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private const string IS_RUNNING = "IsRunning";
    private const string IS_DIE = "IsDie";
    private const string ATTACK = "Attack";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Player.Instance.OnPlayerDeath += Instance_OnPlayerDeath;

        ActiveWeapon.Instance.OnWeaponAttack += ActiveWeapon_OnWeaponAttack;
    }

    private void OnDestroy()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnPlayerDeath -= Instance_OnPlayerDeath;
        }
        if (ActiveWeapon.Instance != null)
        {
            ActiveWeapon.Instance.OnWeaponAttack -= ActiveWeapon_OnWeaponAttack;
        }
    }

    private void Update()
    {
        animator.SetBool(IS_RUNNING, Player.Instance.IsRunning());

        if (Player.Instance.IsAlive()) AdjustPlayerFacingDirection();
    }

    private void Instance_OnPlayerDeath(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_DIE, true);
    }

    private void ActiveWeapon_OnWeaponAttack(object sender, EventArgs e)
    {
        animator.SetTrigger(ATTACK);
    }

    private void AdjustPlayerFacingDirection()
    {
        if (Time.timeScale == 0f) return;

        Vector3 mousePos = GameInput.Instance.GetMousePosition();
        Vector3 playerPosition = Player.Instance.GetPlayerScreenPosition();

        if (mousePos.x < playerPosition.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}
