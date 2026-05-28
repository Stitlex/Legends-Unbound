using UnityEngine;
using UnityEngine.AI;
using LegendsUnbound.Utils;
using System;

public class EnemyAI : MonoBehaviour
{
    [Header("Roaming")]
    [SerializeField] private State startingState;
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 2f;

    [Header("Chasing")]
    [SerializeField] private bool isChasingEnemy = false;
    [SerializeField] private float chasingDistance = 4f;
    [SerializeField] private float chasingSpeedMultiplier = 2f;

    [Header("Attacking")]
    [SerializeField] private bool isAttackingEnemy = false;
    [SerializeField] private float attackingDistance = 2f;
    [SerializeField] private float attackRate = 2f;


    private NavMeshAgent navMeshAgent;
    private State currentState;
    private Vector3 roamPosition;
    private Vector3 startingPosition;
    private Vector3 lastPosition;

    private float roamingTimer;
    private float nextAttackTime;
    private float chasingSpeed;
    private float roamingSpeed;
    private float nextCheckDirectionTime = 0f;
    private float checkDirectionDuration = 0.1f;

    public event EventHandler OnEnemyAttack;
    public bool IsRunning => navMeshAgent.velocity != Vector3.zero;

    private enum State
    {
        Idle,
        Roaming,
        Chasing,
        Attacking,
        Death
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent not found on " + gameObject.name);
            return;
        }
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        currentState = startingState;
        roamingSpeed = navMeshAgent.speed;
        chasingSpeed = navMeshAgent.speed * chasingSpeedMultiplier;

        startingPosition = transform.position;

    }

    private void Update()
    {
        StateHandler();
        MovementDirectonHandler();
    }

    public void SetDeathState()
    {
        navMeshAgent.ResetPath();
        currentState = State.Death;
    }

    public float GetRoamingAnimationSpeed() => navMeshAgent.speed / roamingSpeed;

    private void StateHandler()
    {
        switch (currentState)
        {
            case State.Roaming:
                RoamingState();
                break;
            case State.Chasing:
                ChasingTarget();
                break;
            case State.Attacking:
                AttackingTarget();
                break;
            case State.Death:
                break;
            default:
            case State.Idle:
                break;

        }
    }

    private void RoamingState()
    {
        roamingTimer -= Time.deltaTime;
        if (roamingTimer < 0)
        {
            Roaming();
            roamingTimer = roamingTimerMax;
        }
        CheckCurrentState();
    }

    private void ChasingTarget()
    {
        navMeshAgent.SetDestination(Player.Instance.transform.position);
        CheckCurrentState();
    }

    private void AttackingTarget()
    {
        if (Time.time > nextAttackTime)
        {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);

            nextAttackTime = Time.time + attackRate;
        }
        CheckCurrentState();
    }

    private void CheckCurrentState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        State newState = State.Roaming;

        if (isChasingEnemy)
        {
            if (distanceToPlayer <= chasingDistance)
            {
                newState = State.Chasing;
            }
        }

        if (isAttackingEnemy)
        {
            if (distanceToPlayer <= attackingDistance)
            {
                newState = State.Attacking;
            }
        }

        if (newState != currentState)
        {
            if (newState == State.Chasing)
            {
                navMeshAgent.ResetPath();
                navMeshAgent.speed = chasingSpeed;
            }
            else if (newState == State.Roaming)
            {
                roamingTimer = 0f;
                navMeshAgent.speed = roamingSpeed;
            }
            else if (newState == State.Attacking)
            {
                navMeshAgent.ResetPath();
            }

            currentState = newState;
        }
    }

    private Vector3 GetRoamingPosition() => startingPosition + Utils.GetRandomDir() * UnityEngine.Random.Range(roamingDistanceMin, roamingDistanceMax);

    private void Roaming()
    {
        roamPosition = GetRoamingPosition();
        navMeshAgent.SetDestination(roamPosition);
    }

    private void MovementDirectonHandler()
    {
        if (Time.time > nextCheckDirectionTime)
        {
            if (IsRunning)
            {
                ChangeFacingDirection(lastPosition, transform.position);
            }
            else if (currentState == State.Attacking)
            {
                ChangeFacingDirection(transform.position, Player.Instance.transform.position);
            }

            lastPosition = transform.position;
            nextCheckDirectionTime = Time.time + checkDirectionDuration;
        }
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
