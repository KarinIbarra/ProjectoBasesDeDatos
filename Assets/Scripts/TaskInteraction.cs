using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TaskInteraction : MonoBehaviour
{
    [Header("Configuración")]
    public float taskDuration = 6f;
    public float qteChanceTimeMin = 0.3f;
    public float qteChanceTimeMax = 0.7f;
    public float boostAmount = 2f;

    [Header("QTE")]
    public float qteDuration = 2f;
    public float barSpeed = 0.5f;

    [Header("Referencias")]
    public Slider progressBar;
    public GameObject qteUI;
    public RectTransform movingBar;
    public RectTransform successZone;
    public GameObject interactText;
    public Enemy enemy;

    private bool playerInRange;
    private bool doingTask;
    private float currentTime;
    private PlayerController playerController;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();

        progressBar.gameObject.SetActive(false);
        qteUI.SetActive(false);
        interactText.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !doingTask)
        {
            StartCoroutine(DoTask());
        }
    }

    IEnumerator DoTask()
    {
        doingTask = true;
        currentTime = taskDuration;

        progressBar.value = 0;
        progressBar.gameObject.SetActive(true);
        interactText.SetActive(false);

        playerController.canMove = false;

        float qteCooldown = 1f; 
        float lastQteTime = -qteCooldown;

        while (currentTime > 0)
        {
            if (enemy.CanSeePlayer())
            {
                CancelTask();
                yield break;
            }

            currentTime -= Time.deltaTime;
            progressBar.value = 1 - (currentTime / taskDuration);

            if (Time.time >= lastQteTime + qteCooldown)
            {
                float chance = 0.5f * Time.deltaTime;

                if (Random.value < chance)
                {
                    lastQteTime = Time.time;
                    yield return StartCoroutine(QuickTimeEvent());
                }
            }

            yield return null;
        }

        CompleteTask();
    }

    IEnumerator QuickTimeEvent()
    {
        qteUI.SetActive(true);

        RectTransform container = movingBar.parent as RectTransform;
        float containerWidth = container.rect.width;

        float zoneWidthPx = containerWidth * 0.15f;
        float barWidthPx = containerWidth * 0.10f;

        float zoneStartPx = Random.Range(0f, containerWidth - zoneWidthPx);

        successZone.anchorMin = new Vector2(0, 0.5f);
        successZone.anchorMax = new Vector2(0, 0.5f);
        successZone.pivot = new Vector2(0, 0.5f);
        successZone.sizeDelta = new Vector2(zoneWidthPx, 40f);
        successZone.anchoredPosition = new Vector2(zoneStartPx, 0);

        movingBar.anchorMin = new Vector2(0, 0.5f);
        movingBar.anchorMax = new Vector2(0, 0.5f);
        movingBar.pivot = new Vector2(0, 0.5f);
        movingBar.sizeDelta = new Vector2(barWidthPx, 40f);

        float timer = 0f;
        bool success = false;
        float speedPx = containerWidth * barSpeed;

        while (timer < qteDuration)
        {
            timer += Time.deltaTime;

            float maxPos = containerWidth - barWidthPx;
            float barPx = Mathf.PingPong(timer * speedPx, maxPos);
            movingBar.anchoredPosition = new Vector2(barPx, 0);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                float barLeft = barPx;
                float barRight = barPx + barWidthPx;
                float zoneLeft = zoneStartPx;
                float zoneRight = zoneStartPx + zoneWidthPx;

                
                if (barLeft <= zoneRight && barRight >= zoneLeft)
                {
                    success = true;
                }
                break;
            }

            yield return null;
        }

        qteUI.SetActive(false);

        if (success)
        {
            Debug.Log("QTE PERFECTO");
            currentTime -= boostAmount;
        }
        else
        {
            Debug.Log("QTE FALLADO");
            enemy.AlertToPosition(transform.position);
            CancelTask();
        }
    }

    void CompleteTask()
    {
        Debug.Log("Tarea completada");

        progressBar.gameObject.SetActive(false);
        qteUI.SetActive(false);
        playerController.canMove = true;
        doingTask = false;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnTaskCompleted();
        }

        gameObject.SetActive(false); 
    }

    void CancelTask()
    {
        StopAllCoroutines();

        progressBar.gameObject.SetActive(false);
        qteUI.SetActive(false);

        playerController.canMove = true;
        doingTask = false;

        Debug.Log("Tarea cancelada");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactText.SetActive(false);
        }
    }
}