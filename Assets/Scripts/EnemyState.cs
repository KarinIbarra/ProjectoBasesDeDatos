using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState
{
    public abstract void EnterState(Enemy enemy);
    public abstract void UpdateState(Enemy enemy);
    public abstract void ExitState(Enemy enemy);
}

#region PATROL
public class EnemyPatrolState : EnemyState
{
    private Transform targetPoint;

    public override void EnterState(Enemy enemy)
    {
        Debug.Log(enemy.name + " entrando en estado PATROL");

        targetPoint = enemy.GetNextPatrolPoint();

        if (targetPoint == null)
        {
            Debug.LogWarning(enemy.name + " NO tiene puntos de patrulla asignados.");
            return;
        }

        Debug.Log(enemy.name + " moviéndose hacia: " + targetPoint.name);

        enemy.MoveTo(targetPoint.position, enemy.patrolSpeed);
    }

    public override void UpdateState(Enemy enemy)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

       
        if (!agent.hasPath && targetPoint != null)
        {
            enemy.MoveTo(targetPoint.position, enemy.patrolSpeed);
        }

        if (enemy.CanSeePlayer())
        {
            if (enemy is RangedEnemy)
                enemy.ChangeState(new RangedChaseState());
            else
                enemy.ChangeState(new EnemyChaseState());
            return;
        }

        if (targetPoint != null &&
            Vector3.Distance(enemy.transform.position, targetPoint.position) < 1f)
        {
            targetPoint = enemy.GetNextPatrolPoint();
            if (targetPoint != null)
                enemy.MoveTo(targetPoint.position, enemy.patrolSpeed);
        }
    }

    public override void ExitState(Enemy enemy) { }

}
#endregion

#region CHASE
public class EnemyChaseState : EnemyState
{
    public override void EnterState(Enemy enemy)
    {
        enemy.StartMemory();
    }

    public override void UpdateState(Enemy enemy)
    {
        enemy.MoveTo(enemy.player.position, enemy.chaseSpeed);

        if (enemy.CanSeePlayer())
        {
            enemy.ResetMemory();
        }
        else
        {
            enemy.UpdateMemory();
            if (enemy.MemoryExpired())
            {
                enemy.ChangeState(new EnemyPatrolState());
                return;
            }
        }
        GrabEnemy ge = enemy as GrabEnemy;
        if (ge != null && ge.IsReadyToGrab())
        {
            ge.ChangeState(new GrabWindupState());
            return;
        }

        if (Vector3.Distance(enemy.transform.position, enemy.player.position)
            <= enemy.attackRange)
        {
            enemy.ChangeState(new EnemyAttackState());
        }
    }

    public override void ExitState(Enemy enemy) { }
}
#endregion

#region ATTACK
public class EnemyAttackState : EnemyState
{
    public override void EnterState(Enemy enemy)
    {
        enemy.StopMoving();
        if (enemy.player != null)
        {
            PlayerHealth playerHealth = enemy.player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(1);
            else
                Debug.LogWarning("El jugador no tiene PlayerHealth");
        }
        else
        {
            Debug.LogWarning("Enemy.player no está asignado");
        }
        enemy.ChangeState(new EnemyAttackCooldownState());
    }

    public override void UpdateState(Enemy enemy) { }
    public override void ExitState(Enemy enemy) { }
}
#endregion

#region COOLDOWN
public class EnemyAttackCooldownState : EnemyState
{
    private float timer;

    public override void EnterState(Enemy enemy)
    {
        timer = enemy.attackCooldown;
        enemy.StopMoving();
    }

    public override void UpdateState(Enemy enemy)
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
            enemy.ChangeState(new EnemyChaseState());
    }

    public override void ExitState(Enemy enemy) { }
}
#endregion


#region RANGED CHASE
public class RangedChaseState : EnemyState
{
    public override void EnterState(Enemy enemy)
    {
        enemy.StartMemory();
    }

    public override void UpdateState(Enemy enemy)
    {
        RangedEnemy re = enemy as RangedEnemy;
        if (re == null) return;

        if (!re.CanSeePlayer())
        {
            re.UpdateMemory();
            if (re.MemoryExpired())
            {
                re.ChangeState(new EnemyPatrolState());
                return;
            }
        }
        else
        {
            re.ResetMemory();
        }

        float dist = Vector3.Distance(re.transform.position, re.player.position);

     
        if (dist > re.preferredDistance)
        {
            re.MoveTo(re.player.position, re.chaseSpeed);
        }
        else
        {
           
            re.StopMoving();

            if (re.IsReadyToShoot())
                re.ChangeState(new RangedWindupState());
        }

        
        Vector3 lookDir = re.player.position - re.transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
        {
            re.transform.rotation = Quaternion.Slerp(
                re.transform.rotation,
                Quaternion.LookRotation(lookDir),
                10f * Time.deltaTime);
        }
    }

    public override void ExitState(Enemy enemy) { }
}
#endregion

#region RANGED WINDUP
public class RangedWindupState : EnemyState
{
    private float timer;

    public override void EnterState(Enemy enemy)
    {
        RangedEnemy re = enemy as RangedEnemy;
        timer = re != null ? re.shootWindup : 1f;
        enemy.StopMoving();
    }

    public override void UpdateState(Enemy enemy)
    {
        RangedEnemy re = enemy as RangedEnemy;
        if (re == null) return;

        
        Vector3 lookDir = re.player.position - re.transform.position;
        lookDir.y = 0f;
        if (lookDir != Vector3.zero)
            re.transform.rotation = Quaternion.Slerp(
                re.transform.rotation,
                Quaternion.LookRotation(lookDir),
                10f * Time.deltaTime);

        timer -= Time.deltaTime;
        if (timer <= 0f)
            re.ChangeState(new RangedShootState());
    }

    public override void ExitState(Enemy enemy) { }
}
#endregion

#region RANGED SHOOT
public class RangedShootState : EnemyState
{
    public override void EnterState(Enemy enemy)
    {
        RangedEnemy re = enemy as RangedEnemy;
        if (re == null) return;
        re.FireProjectiles();
        re.ChangeState(new RangedChaseState());
    }

    public override void UpdateState(Enemy enemy) { }
    public override void ExitState(Enemy enemy) { }
}
#endregion


public class GrabWindupState : EnemyState
{
    private float timer;

    public override void EnterState(Enemy enemy)
    {
        GrabEnemy ge = enemy as GrabEnemy;
        if (ge == null) return;

        ge.StopMoving();
        timer = 1.5f; 
    }

    public override void UpdateState(Enemy enemy)
    {
        GrabEnemy ge = enemy as GrabEnemy;
        if (ge == null) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            GrabVisual visual = ge.GetComponent<GrabVisual>();
            if (visual != null)
                visual.StartExtend(ge.player);

            ge.ResetGrabCooldown();

            ge.ChangeState(new EnemyChaseState());
        }
    }

    public override void ExitState(Enemy enemy) { }
}