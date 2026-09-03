using UnityEngine;

public class Player_move : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    private void Start()
    {
       
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 0 = 待机，1 = 走路；Sprint 控制走路/跑步切换
        bool isMoving = horizontal != 0f || vertical != 0f;
        bool sprinting = isMoving && Input.GetKey(KeyCode.LeftShift);
        if (animator != null)
        {
            animator.SetFloat("Speed", isMoving ? 1f : 0f);
            animator.SetBool("Sprint", sprinting);
        }

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        Vector3 direction = (transform.right * horizontal + transform.forward * vertical).normalized;

      
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

       
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

      
        velocity.y += gravity * Time.deltaTime;

        // 合成这一帧的位移：水平速度 + 垂直速度，再乘时间
        Vector3 motion = direction * currentSpeed;
        motion.y = velocity.y;

        controller.Move(motion * Time.deltaTime);
    }
}
