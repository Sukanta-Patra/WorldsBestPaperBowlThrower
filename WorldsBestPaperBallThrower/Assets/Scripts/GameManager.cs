using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LevelStartPosition
{
    public GameObject levelParent;
    public Transform ballStartTransform;
    public Transform bucketStartTransform;
}


public class GameManager : MonoBehaviour
{

    [Header("Values")]
    [SerializeField] private float releaseForce;

    [Header("References")]
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject bucket;
    [SerializeField] private Collider2D innerCollider;

    [SerializeField] private LineRenderer pullLineRenderer;

    [SerializeField] private List<LevelStartPosition> levelStartPositions;

    private bool isAiming = false;
    private Rigidbody2D ballRigidBody;

    private Vector2 startPoint; // Starting point of the drag
    private Vector2 endPoint; // Ending point of the drag
    private Vector2 direction; // Direction of the force
    private float distance; // Distance the drag traveled

    /// <summary>
    /// isGameRunning controls if the entire gameplay can be played or not 
    /// </summary>
    private bool isGameRunning = false;
    /// <summary>
    /// isGameComplete flags if the game completed after player pushed the ball inside the bucket
    /// </summary>
    private bool isGameComplete = false;
    /// <summary>
    /// 0 - Level Setting has not started
    /// 1 - Level Setting has started
    /// 2 - Level Setting has finished
    /// </summary>
    private int levelSetStatus = 0;

    private int currentLevel = 0;

    // Start is called before the first frame update
    void Start()
    {
        ballRigidBody = ball.GetComponent<Rigidbody2D>();
        pullLineRenderer.gameObject.SetActive(false);
        StartCoroutine(UpdateLevel(-1, 1f));
        isGameRunning = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (isGameRunning)
        {
            if (!isGameComplete)
            {
                #region COLLISION_DETECTION
                if (ball.GetComponent<Collider2D>().IsTouching(innerCollider)) isGameComplete = true;
                #endregion

                #region BALL_MOVEMENT
                if (Input.GetMouseButtonDown(0))
                {
                    startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    isAiming = true;
                }

                if (isAiming)
                {
                    pullLineRenderer.gameObject.SetActive(true);
                    endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    pullLineRenderer.SetPosition(0, startPoint);
                    pullLineRenderer.SetPosition(1, endPoint);
                }

                if (isAiming && Input.GetMouseButtonUp(0))
                {
                    isAiming = false;
                    // Calculate direction and distance
                    direction = startPoint - endPoint;
                    distance = direction.magnitude;
                    pullLineRenderer.gameObject.SetActive(false);
                    // Following Force is the resultant of F = -kx formula (Hooke's Law)
                    // Where, k is the constant : releaseForce
                    // x is the distance the force was applied to : distance
                    // - is the direction to which the force was applied to : direction.normalized
                    // Apply force to the projectile
                    ballRigidBody.AddForce(direction.normalized * releaseForce * distance, ForceMode2D.Impulse);
                }
                #endregion
            }
            else
            {
                //Set Level to currentLevel and update objects to respective start positions
                if (levelSetStatus == 0)
                    StartCoroutine(UpdateLevel(currentLevel, 2f));

            }
        }
    }

    /// <summary>
    /// Update the next level wrt levelIndex within time
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator UpdateLevel(int levelIndex, float time)
    {

        levelSetStatus = 1;

        yield return new WaitForSeconds(2f);

        ballRigidBody.velocity = Vector2.zero;
        bucket.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ballRigidBody.bodyType = RigidbodyType2D.Kinematic;
        bucket.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        Vector3 ballStartPosition = ball.transform.position;
        Vector3 bucketStartPosition = bucket.transform.position;

        Quaternion ballStartRotation = ball.transform.rotation;
        Quaternion bucketStartRotation = bucket.transform.rotation;
        float elapsedTime = 0;

        levelStartPositions[currentLevel].levelParent.SetActive(false);
        levelIndex++;
        if (levelIndex > levelStartPositions.Count - 1) levelIndex = 0;
        currentLevel = levelIndex;
        levelStartPositions[currentLevel].levelParent.SetActive(true);

        while (elapsedTime < time)
        {
            float t = (elapsedTime / time);
            ball.transform.position = Vector3.Lerp(ballStartPosition, levelStartPositions[currentLevel].ballStartTransform.position, t);
            ball.transform.rotation = Quaternion.Slerp(ballStartRotation, Quaternion.Euler(Vector2.zero), t);
            bucket.transform.position = Vector3.Lerp(bucketStartPosition, levelStartPositions[currentLevel].bucketStartTransform.position, t);
            bucket.transform.rotation = Quaternion.Slerp(bucketStartRotation, Quaternion.Euler(Vector2.zero), t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ball.transform.position = levelStartPositions[currentLevel].ballStartTransform.position;
        bucket.transform.position = levelStartPositions[currentLevel].bucketStartTransform.position;

        ballRigidBody.bodyType = RigidbodyType2D.Dynamic;
        bucket.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        isGameComplete = false;
        levelSetStatus = 0;
        yield return null;
    }
}

