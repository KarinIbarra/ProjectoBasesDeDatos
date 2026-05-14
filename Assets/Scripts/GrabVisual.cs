using UnityEngine;
using System.Collections;

public class GrabVisual : MonoBehaviour
{
    public Transform leftArm;
    public Transform rightArm;

    public float extendSpeed = 8f;
    public float maxLength = 10f;
    public float grabRadius = 1.2f;

    private bool extending = false;
    private float currentLength = 1f;

    private Transform target;
    private GrabEnemy owner;

    void Awake()
    {
        owner = GetComponent<GrabEnemy>();
    }

    public void StartExtend(Transform player)
    {
        target = player;
        extending = true;
        currentLength = 1f;

        leftArm.gameObject.SetActive(true);
        rightArm.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!extending || target == null) return;

        currentLength += extendSpeed * Time.deltaTime;

        ExtendArm(leftArm, currentLength);
        ExtendArm(rightArm, currentLength);

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        Vector3 armTip = transform.position + transform.forward * currentLength;

        float tipDistance = Vector3.Distance(armTip, target.position);

        if (tipDistance <= grabRadius)
        {
            extending = false;
            StartCoroutine(DragPlayer());
        }

       
        if (currentLength >= maxLength)
        {
            StopExtend();
        }
    }

    void ExtendArm(Transform arm, float length)
    {
        Vector3 scale = arm.localScale;
        scale.z = length;
        arm.localScale = scale;

        
        arm.localPosition = new Vector3(
            arm.localPosition.x,
            arm.localPosition.y,
            length / 2f
        );
    }

    IEnumerator DragPlayer()
    {
        float dragTime = 0.6f;
        float timer = 0f;

        Vector3 startPos = target.position;
        Vector3 endPos = owner.transform.position;

        while (timer < dragTime)
        {
            timer += Time.deltaTime;
            target.position = Vector3.Lerp(startPos, endPos, timer / dragTime);
            yield return null;
        }

        PlayerHealth ph = target.GetComponent<PlayerHealth>();
        if (ph != null)
            ph.TakeDamage(1);

        
        owner.ResetGrabCooldown();

        
        leftArm.gameObject.SetActive(false);
        rightArm.gameObject.SetActive(false);

        
        owner.ChangeState(new EnemyChaseState());
    }

    public void StopExtend()
    {
        extending = false;

        leftArm.gameObject.SetActive(false);
        rightArm.gameObject.SetActive(false);
    }
}