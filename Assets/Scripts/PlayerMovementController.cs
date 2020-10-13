using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsObstacle;
    [SerializeField] private Animator thisAnimator;
    [SerializeField] private TrailRenderer sprintEffect;
    [SerializeField] private float jumpForce = 1000f;
    [SerializeField] private float speedMult;
    [SerializeField] private float maxDistanceFromGround;
    [SerializeField] private float sprintMaxSpeed = 2f;
    [SerializeField] private float sprintChangeDelta = 20f;

    private PlayerCameraController camController;
    private Rotation fireControl;
    private Camera mainCam;
    private Rigidbody thisRb = null;
    private bool isGrounded => CheckIfGrounded();
    private bool isCrouching = false;

    private float sprintMultiplier = 1f;
    private float right;
    private float forth;

    private bool leftShift;
    private bool space;
    private bool c;

    private void Awake()
    {
        thisRb = GetComponent<Rigidbody>();
        camController = GetComponent<PlayerCameraController>();
        fireControl = GetComponent<Rotation>();
        thisAnimator = GetComponent<Animator>();

        mainCam = Camera.main;
    }

    private void FixedUpdate()
    {
        Vector3 velocityVector/* = (Vector3.right * right + Vector3.forward * forth) * speedMult * Time.fixedDeltaTime*/;

        velocityVector = new Vector3((mainCam.transform.right.x * right + mainCam.transform.forward.x * forth) * speedMult * Time.fixedDeltaTime * sprintMultiplier, thisRb.velocity.y,
            (mainCam.transform.right.z * right + mainCam.transform.forward.z * forth) * speedMult * Time.fixedDeltaTime * sprintMultiplier);

        thisRb.velocity = velocityVector;

        if (isGrounded)
        {
            if (space && thisRb.velocity.y <= 0.06f)
            {
                thisRb.AddForce(Vector3.up * jumpForce * Time.fixedDeltaTime, ForceMode.Impulse);

                if (isCrouching)
                {
                    TryCrouch();
                }
            }
        }
    }

    private void Update()
    {
        right = Input.GetAxisRaw("Horizontal"); // a and d
        forth = Input.GetAxisRaw("Vertical"); // w and s

        space = Input.GetKey(KeyCode.Space);
        leftShift = Input.GetKey(KeyCode.LeftShift);
        c = Input.GetKeyDown(KeyCode.C);

        //BodyFollowsMouse();

        if (leftShift && thisRb.velocity.y <= 0.06f && !fireControl.GetFiringState)
        {
            if (isCrouching)
            {
                TryCrouch();
            }

            Sprint();

            if (!sprintEffect.emitting)
            {
                sprintEffect.emitting = true;
            }
        }
        else
        {
            sprintMultiplier = 1f;

            if (sprintEffect.emitting)
            {
                sprintEffect.emitting = false;
            }

            if (thisAnimator.GetBool("isSprinting"))
            {
                thisAnimator.SetBool("isSprinting", false);
            }
            else
            {

            }
        }

        if (c)
        {
            TryCrouch();
        }
    }

    private void TryCrouch()
    {
        isCrouching = !isCrouching;

        thisAnimator.SetBool("isCrouching", isCrouching);


        if (thisAnimator.GetBool("isSprinting"))
        {
            thisAnimator.SetBool("isSprinting", false);
        }

        if (isCrouching)
        {
            sprintMultiplier = 0.5f;
        }
        else
        {
            sprintMultiplier = 1f;
        }
    }

    private void Sprint()
    {
        sprintMultiplier = Mathf.MoveTowards(sprintMultiplier, sprintMaxSpeed, sprintChangeDelta * Time.fixedDeltaTime);

        if (thisAnimator.GetBool("isSprinting"))
        {

        }
        else
        {
            thisAnimator.SetBool("isSprinting", true);
        }
    }

    private bool CheckIfGrounded()
    {
        Ray ray = new Ray { origin = transform.position, direction = Vector3.down };
        RaycastHit[] hits = Physics.RaycastAll(ray, transform.localScale.y / 2f + maxDistanceFromGround, whatIsObstacle);

        if (hits.Length > 0)
        {
            //Debug.Log("hits > 0");
            if (hits[0].collider.gameObject == gameObject)
            {
                //Debug.Log("hit self");
                if (hits.Length > 1)
                {
                    //Debug.Log("hits > 1");
                    if (hits[1].collider.gameObject)
                    {
                        //Debug.Log("hit other target");
                        return true;
                    }
                }

                //Debug.Log("hit nothing else");
                return false;
            }
            else
            {
                //Debug.Log("didnt hit self, hit other target only");
                return true;
            }
        }
        else
        {
            //Debug.Log("hit nothing");
            return false;
        }
    }

    private void BodyFollowsMouse()
    {
        Vector3 hitPos = new Vector3();
        Vector2 screenCursorPos = camController.actualCursorPos;
        Ray ray = mainCam.ScreenPointToRay(screenCursorPos);
        RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue);

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider)
                {
                    if (hits[i].collider.transform == transform || hits[i].collider.transform) // this check never runs for some reason
                    { // check if object itself is ignored or this
                        Debug.Log("hit ignroed obj or itself");
                        if (i == hits.Length - 1)
                        {
                            hitPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.farClipPlane));
                        }

                        continue;
                    }
                    else // didnt directly hit itself or ignored obj
                    { // if it isnt perform other checks
                        if (hits[i].collider.attachedRigidbody)
                        { // checks the main rigidbody (if the object has child colliders, then this is set to the parent rb
                            if (hits[i].collider.attachedRigidbody.gameObject == gameObject)
                            { // if the rigidbody's gameobject is the gameobject or ignored one
                                if (i == hits.Length - 1)
                                {
                                    hitPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.farClipPlane));
                                }

                                continue;
                            }
                            else
                            { // rigidbody isnt gameobject or ignored one
                                Debug.Log("set here 1");

                                hitPos = hits[i].rigidbody.transform.position;
                                break;
                            }
                        } // doesnt have an rb attached 
                        else
                        {
                            Debug.Log("set here 2");

                            hitPos = hits[i].point;
                            break;
                        }
                    }
                }

                if (i == hits.Length - 1)
                {
                    hitPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.farClipPlane));
                }
            }
        }
        else
        {
            hitPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.farClipPlane));
        }

        Vector3 dir = hitPos - transform.position;
        Vector3 originDir = hitPos - transform.position;
        transform.forward = new Vector3(dir.x, 0f, dir.z);
        Debug.DrawRay(transform.position, hitPos - transform.position, Color.green);
        //mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        //Debug.DrawLine(transform.position, mousePos);
    }
}
#pragma warning restore 0649
