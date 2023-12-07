using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCharacter : MonoBehaviour
{
    private Rigidbody rbCharacter;
    private Animator CharacterAnimator;
    [SerializeField] private float speed;
    [SerializeField] private float speedRotation;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpForceForLongJump;
    public Transform RayCastEmpty;
    public LayerMask Default;
    private void Start()
    {
        //Get Component
        rbCharacter = GetComponent<Rigidbody>();
        CharacterAnimator = GetComponent<Animator>();

        //Set any values
        speed = 4f;
        speedRotation = 10f;
        jumpForce = 5f;
        jumpForceForLongJump = 10f;
    }
    private void FixedUpdate()
    {
        //Get Axis Input
        float hz = Input.GetAxis("Horizontal");
        float vt = Input.GetAxis("Vertical");

        //Are we in air?
        if(Physics.CheckSphere(RayCastEmpty.position, 0.4f))
        {
            CharacterAnimator.SetBool("IsInAir", false);
        }
        else
        {
            CharacterAnimator.SetBool("IsInAir", true);
        }

        //Direction Vector3
        Vector3 vectorOrientation = new Vector3(hz, 0, vt);

        if(vectorOrientation.magnitude > Mathf.Abs(0.05f)){
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(vectorOrientation), Time.deltaTime * speedRotation);
        }

        //Set speed value
        CharacterAnimator.SetFloat("speedMove", Vector3.ClampMagnitude(vectorOrientation, 1).magnitude);

        //Movement Direction
        Vector3 moveDir = Vector3.ClampMagnitude(vectorOrientation, 1) * speed;
        
        //Velocity Movement
        if (CharacterAnimator.GetBool("IsShift"))
        {
            rbCharacter.velocity = new Vector3(moveDir.x * 2, rbCharacter.velocity.y, moveDir.z * 2);
        }
        else
        {
            rbCharacter.velocity = new Vector3(moveDir.x, rbCharacter.velocity.y, moveDir.z);
        }

        //angularVelocity zeroing out
        rbCharacter.angularVelocity = Vector3.zero;
    }


    void Update()
    {
        //Jump
        if(Input.GetKeyDown(KeyCode.Space)){
            if(CharacterAnimator.GetBool("IsShift")){
                LongJumpMethod();
            }
            else{
                JumpMethod();
            }
        }
        if(Input.GetKeyDown(KeyCode.LeftShift)){
            CharacterAnimator.SetBool("IsShift", true);
        }
        if(Input.GetKeyUp(KeyCode.LeftShift)){
            CharacterAnimator.SetBool("IsShift", false);
        }
    }
    private void JumpMethod()
    {
        //Raycast hit
        if (Physics.Raycast(RayCastEmpty.position, Vector3.down, 0.4f)){
            CharacterAnimator.SetTrigger("Jump");
            rbCharacter.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    private void LongJumpMethod()
    {
        //Raycast hit for long jump
        if (Physics.Raycast(RayCastEmpty.position, Vector3.down, 0.4f)){
            CharacterAnimator.SetTrigger("Jump");
            rbCharacter.AddForce(new Vector3(0f, 0.7f, 2f) * jumpForceForLongJump, ForceMode.Impulse);
        }
    }
}
