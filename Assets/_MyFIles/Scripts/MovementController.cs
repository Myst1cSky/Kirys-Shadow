using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]

public class MovementController : MonoBehaviour
{
    [SerializeField] private float mJumpSpeed = 7f;
    [SerializeField] private float mMaxMoveSpeed = 5f;
    [SerializeField] private float mGroundMoveSpeedAcceleration = 5f;
    [SerializeField] private float mAirMoveSpeedAcceleration = 5f;
    [SerializeField] private float mTurnLerpRate = 40f;
    [SerializeField] private float mMaxFallSpeed = 50f;
    [SerializeField] private float mAirCheckRadius = 0.5f;

    [SerializeField] LayerMask mAirCheckLayerMask = 1;

    private PlayerInputActions mPlayerInputActions;
    private CharacterController mCharacterController;
    private Animator mAnimator;

    private Vector3 mVerticalVelocity;
    private Vector3 mHorizontalVelocity;
    private Vector2 mMoveInput;

    private bool mShouldTryJump;
    private bool mIsInAir;

    //private PlayerInputActions mPlayerInputActions = new PlayerInputActions();

    public PlayerInputActions GetInputActions()
    {
        return mPlayerInputActions;
    }

    private void Awake()
    {
        mPlayerInputActions = new PlayerInputActions();
        mPlayerInputActions.Gameplay.Jump.performed += PerformJump;

        mPlayerInputActions.Gameplay.Move.performed += HandleMoveInput;
        mPlayerInputActions.Gameplay.Move.canceled += HandleMoveInput;

        mCharacterController = GetComponent<CharacterController>();
        mAnimator = GetComponent<Animator>();
        
    }

    private void HandleMoveInput(InputAction.CallbackContext context)
    {
        mMoveInput = context.ReadValue<Vector2>();
        Debug.Log($"Move Input is: {mMoveInput}");
    }

    private void PerformJump(InputAction.CallbackContext context)
    {
        Debug.Log($"Jumping");
        if (!mIsInAir)
        {
            mAnimator.SetTrigger("Jump");
            mShouldTryJump = true;    
        }
    }

    private bool IsInAir()
    {
        if (mCharacterController.isGrounded)
        {
            return false;
        }

        Collider[] airCheckColliders = Physics.OverlapSphere(transform.position, mAirCheckRadius, mAirCheckLayerMask);
        foreach(Collider collider in airCheckColliders)
        {
            if (collider.gameObject != gameObject)
            {
                return false;
            }
        }

        return true;
    }

    private void OnEnable()
    {
        mPlayerInputActions.Enable();
    }

    private void OnDisable()
    {
        mPlayerInputActions.Disable();
    }

    void Update()
    {
        mIsInAir = IsInAir();

        UpdateVerticalVelocity();
        UpdateHorizontalVelocity();
        UpdateTransform();
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        mAnimator.SetFloat("Speed", mHorizontalVelocity.magnitude);
        mAnimator.SetBool("Landed", !mIsInAir);
    }

    private void UpdateTransform()
    {
        mCharacterController.Move((mHorizontalVelocity + mVerticalVelocity) * Time.deltaTime);
        if (mHorizontalVelocity.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mHorizontalVelocity.normalized, Vector3.up),
            Time.deltaTime * mTurnLerpRate);
        }
    }

    private void UpdateVerticalVelocity()
    {
        // try jump first if should try jump is true
        if (mShouldTryJump && !mIsInAir)
        {
            mVerticalVelocity.y = mJumpSpeed;
            
            mShouldTryJump = false;
            return;
        }

        // we are on the ground, set the velocity to a small velocity going down
        if (mCharacterController.isGrounded)
        {
            mVerticalVelocity.y = -1f;
            return;
        }

        // free falling
        if (mVerticalVelocity.y > -mMaxFallSpeed)
        {
            mVerticalVelocity.y += Physics.gravity.y * Time.deltaTime;
        }
    }

    void UpdateHorizontalVelocity()
    {
        Vector3 moveDir = PlayerInputToWorldDir(mMoveInput);

        float acceleration = mCharacterController.isGrounded ? mGroundMoveSpeedAcceleration : mAirMoveSpeedAcceleration;
        if (moveDir.sqrMagnitude > 0)
        {
            mHorizontalVelocity += moveDir * mGroundMoveSpeedAcceleration * Time.deltaTime;
            mHorizontalVelocity = Vector3.ClampMagnitude(mHorizontalVelocity, mMaxMoveSpeed);
        }
        else
        {
            if (mHorizontalVelocity.sqrMagnitude > 0)
            {
                mHorizontalVelocity -= mHorizontalVelocity.normalized * acceleration * Time.deltaTime;
                if (mHorizontalVelocity.sqrMagnitude < 0.1)
                {
                    mHorizontalVelocity = Vector3.zero;
                }
            }
        }
    }

    Vector3 PlayerInputToWorldDir(Vector2 inputVal)
    {
        Vector3 rightDir = Camera.main.transform.right;
        Vector3 fwdDir = Vector3.Cross(rightDir, Vector3.up);

        return rightDir * inputVal.x + fwdDir * inputVal.y;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = mIsInAir ? Color.red : Color.green;
        Gizmos.DrawSphere(transform.position, mAirCheckRadius);
    }
}
