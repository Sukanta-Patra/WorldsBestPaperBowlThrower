using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
 
    [Header("Values")]
    [SerializeField] private float releaseForce;

    [Header("References")]
    [SerializeField] private GameObject ball;
    [SerializeField] private Collider2D innerCollider;

    [SerializeField] private LineRenderer pullLineRenderer;    

    private bool isAiming = false;
    private Rigidbody2D ballRigidBody;

    private Vector2 startPoint; // Starting point of the drag
    private Vector2 endPoint; // Ending point of the drag
    private Vector2 direction; // Direction of the force
    private float distance; // Distance the drag traveled


    // Start is called before the first frame update
    void Start()
    {
        ballRigidBody = ball.GetComponent<Rigidbody2D>();
        pullLineRenderer.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        #region COLLISION_DETECTION
        //Debug.Log("TEST1: IsTouching: " + ball.GetComponent<Collider2D>().IsTouching(innerCollider));
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
}
