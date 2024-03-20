using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private Collider2D innerCollider;
    [SerializeField] private GameObject aimMarker;
    [SerializeField] private Transform aimMarkerPivot;
    [SerializeField] private float aimMarkerRotateSpeed;

    [SerializeField] private bool isAiming = false;
    private Rigidbody2D ballRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        ballRigidBody = ball.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        #region COLLISION_DETECTION
        Debug.Log("TEST1: ISTOuching: " + ball.GetComponent<Collider2D>().IsTouching(innerCollider));
        #endregion

        #region AIM_MARKER_ROTATION
        if (isAiming)
        {
            aimMarkerPivot.Rotate(0f, 0f, aimMarkerRotateSpeed * Time.deltaTime);
        }
        #endregion
    }

    private void ShowAimMarker()
    {
        isAiming = true;
        LeanTween.alpha(aimMarker, 1, 0.25f);
    }
    private void HideAimMarker() 
    {
        isAiming = false;
        LeanTween.alpha(aimMarker, 0, 0.25f);
    }
}
