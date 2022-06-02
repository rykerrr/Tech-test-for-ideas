using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = System.Object;
using EventSystem = UnityEngine.EventSystems.EventSystem;
using InputField = UnityEngine.UI.InputField;

#pragma warning disable 0649
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsObstacle;
    [SerializeField] private TrailRenderer sprintEffect;
    [SerializeField] private Transform rotateAroundSparkles;
    [SerializeField] private Transform[] sparksRotationalParent;
    [SerializeField] private ParticleSystem[] wheelSparksParticleSys;
    [SerializeField] private ParticleSystem jetpackJumpShockwave;
    [SerializeField] private Animator[] wheelAnimators;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float jumpForce = 1000f;
    [SerializeField] private float speedMult;
    [SerializeField] private float maxDistanceFromGround;
    [SerializeField] private float sprintMaxSpeed = 2f;
    [SerializeField] private float sprintChangeDelta = 20f;
    [SerializeField] private GunBase gun = null;

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

    private int wheelMovingAnimationHash;
    private int gunIsEquippedHash;

    // private int sprintAnimHash;
    // private int crouchAnimHash;

    private void Awake()
    {
        thisRb = GetComponent<Rigidbody>();
        // gun = GetComponent<GunBase>();

        mainCam = Camera.main;
        sprintEffect.emitting = false;

        wheelMovingAnimationHash = Animator.StringToHash("wheelIsMoving");
        gunIsEquippedHash = Animator.StringToHash("isEquipped");

        // sprintAnimHash = Animator.StringToHash("isSprinting");
        // crouchAnimHash = Animator.StringToHash("isCrouching");
    }

    private void FixedUpdate()
    {
        Vector3
            velocityVector /* = (Vector3.right * right + Vector3.forward * forth) * speedMult * Time.fixedDeltaTime*/ =
                Vector3.one;

        velocityVector = new Vector3(
            (mainCam.transform.right.x * right + mainCam.transform.forward.x * forth) * speedMult *
            Time.fixedDeltaTime * sprintMultiplier, thisRb.velocity.y,
            (mainCam.transform.right.z * right + mainCam.transform.forward.z * forth) * speedMult *
            Time.fixedDeltaTime * sprintMultiplier);

        thisRb.velocity = velocityVector;

        if (isGrounded)
        {
            if (space && thisRb.velocity.y <= 0.06f)
            {
                // jump

                Jump();
                jetpackJumpShockwave.Emit(1);

                if (isCrouching)
                {
                    TryCrouch();
                }
            }
        }
    }

    private void Jump()
    {
        Vector3 force = Vector3.up * (jumpForce * Time.fixedDeltaTime);
        thisRb.AddForce(force, ForceMode.Impulse);
    }

    private void Update()
    {
        bool isPointerOverUi = EventSystem.current.IsPointerOverGameObject(0);
        bool isReceivingUiInput = false;
        GameObject curSelectedUiObj = EventSystem.current.currentSelectedGameObject;

        // Debug.Log("" + Object.Equals(curSelectedUiObj, null)); // http://prntscr.com/vnwik7
        if (curSelectedUiObj != null)
        {
            if (curSelectedUiObj.GetComponent<InputField>())
            {
                isReceivingUiInput = true;
            }
            else
            {
                isReceivingUiInput = false;
            }
        }
        else
        {
            isReceivingUiInput = false;
        }

        // Move this into an onEquip event method
        if (gun != null)
        {
            if (playerAnimator.GetBool(gunIsEquippedHash) == false)
            {
                playerAnimator.SetBool(gunIsEquippedHash, true);
            }
        }
        else
        {
            if (playerAnimator.GetBool(gunIsEquippedHash) == true)
            {
                playerAnimator.SetBool(gunIsEquippedHash, false);
            }
        }

        if (isReceivingUiInput)
        {
            ResetMovementInput();
        }
        else
        {
            if (isPointerOverUi)
            {
                ResetMovementInput();
            }
            else
            {
                // Do this via an InputActions thing instead

                right = Keyboard.current.dKey.isPressed ? 1 :
                    Keyboard.current.aKey.isPressed ? -1 : 0; // Essentially receives raw input...
                forth = Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0;

                Vector3 movement = new Vector3(right, 0f, forth); // Animations would be handled here

                space = Keyboard.current.spaceKey.isPressed;
                leftShift = Keyboard.current.leftShiftKey.isPressed;
                c = Keyboard.current.cKey.wasPressedThisFrame;

                MovementEffects(movement.sqrMagnitude >= 1);

                //BodyFollowsMouse();

                // GetFiringState is gonna make problems but whatever
                // firing state is a bool not an enum, isFiring in GunBase
                if (leftShift && thisRb.velocity.y <= 0.06f && !gun.IsFiring
                ) // thisRb.velocity.y is there so you don't sprint while jumping
                {
                    // // only if you're falling
                    // if (isCrouching)
                    // {
                    //     TryCrouch();
                    // }
                    //
                    // Sprint();
                    //
                    // if (!sprintEffect.emitting)
                    // {
                    //     sprintEffect.emitting = true;
                    // }
                }
                else
                {
                    sprintMultiplier = 1f;

                    if (sprintEffect.emitting)
                    {
                        sprintEffect.emitting = false;
                    }

                    // if (thisAnimator.GetBool(sprintAnimHash))
                    // {
                    //     thisAnimator.SetBool(sprintAnimHash, false);
                    // }
                }

                if (c)
                {
                    TryCrouch();
                }
            }
        }
    }

    private void ResetMovementInput()
    {
        right = 0;
        forth = 0;

        space = false;
        leftShift = false;
        c = false;
    }

    private void TryCrouch()
    {
        isCrouching = !isCrouching;

        // thisAnimator.SetBool(crouchAnimHash, isCrouching);
        //
        //
        // if (thisAnimator.GetBool(sprintAnimHash))
        // {
        //     thisAnimator.SetBool(sprintAnimHash, false);
        // }

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

        // if (thisAnimator.GetBool(sprintAnimHash))
        // {
        // }
        // else
        // {
        //     thisAnimator.SetBool(sprintAnimHash, true);
        // }
    }

    private bool CheckIfGrounded() // use raycast from a to b method instead
    {
        Ray ray = new Ray {origin = transform.position, direction = Vector3.down};
        RaycastHit[] hits =
            Physics.RaycastAll(ray, transform.localScale.y / 2f + maxDistanceFromGround, whatIsObstacle);

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

    private void MovementEffects(bool enable)
    {
        RotateSparkParents();

        if (isGrounded)
        {
            for (int i = 0; i < wheelSparksParticleSys.Length; i++)
            {
                ParticleSystem.EmissionModule emission = wheelSparksParticleSys[i].emission;
                emission.enabled = enable;
                wheelAnimators[i].SetBool(wheelMovingAnimationHash, enable);
            }
        }
        else
        {
            for (int i = 0; i < wheelSparksParticleSys.Length; i++)
            {
                ParticleSystem.EmissionModule emission = wheelSparksParticleSys[i].emission;
                emission.enabled = false;
                wheelAnimators[i].SetBool(wheelMovingAnimationHash, false);
            }
        }
    }

    private void RotateSparkParents()
    {
        Vector3 dir = rotateAroundSparkles.TransformDirection(thisRb.velocity.normalized);

        Vector3 vel = thisRb.velocity.normalized;
        float heading = Mathf.Atan2(vel.x, vel.z);
        float headingAboutTurn = Mathf.Atan2(-vel.x, -vel.z);

        // foreach (Transform parent in sparksRotationalParent)
        // {
        //     // so uhm
        //     if (vel.sqrMagnitude > 0.1f)
        //     {
        //         Quaternion rotToSparksParent = Quaternion.Euler(0f, headingAboutTurn * Mathf.Rad2Deg, 0f);
        //         parent.rotation = Quaternion.RotateTowards(parent.rotation, rotToSparksParent, 0.5f);
        //     }
        //     else
        //     {
        //         parent.rotation = parent.rotation;
        //     }
        //
        //     // Quaternion rotTo = Quaternion.Euler(rotateAroundSparkles.localEulerAngles.x, tiltY, rotateAroundSparkles.localEulerAngles.z);
        //     // rotateAroundSparkles.localRotation = Quaternion.RotateTowards(rotateAroundSparkles.localRotation, rotTo, 1f);
        //
        //     // parent.right = Vector3.MoveTowards(parent.right, new Vector3(dir.x, 0f, dir.z), 0.5f);
        // }
    }

    public void HorridAnimationEvent_CallChildSpawnCase()
    {
        gun.SpawnMag();
    }
}
#pragma warning restore 0649