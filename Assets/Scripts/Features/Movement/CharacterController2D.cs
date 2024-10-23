using UnityEngine;

public class CharacterController2D : MonoBehaviour, ICharacterMovement2D
{
    [SerializeField]
    float moveSpeed = 5f;

    [SerializeField]
    Transform cameraTransform;

    CharacterController controller;
    SpriteRenderer spriteRenderer;
    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        Move();
    }

    public void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = (right * moveX + forward * moveZ).normalized;

        if (Input.anyKey)
        {
            velocity = moveSpeed * move;
            spriteRenderer.flipX = moveX < 0;
        }
        else
        {
            velocity /= 2;
        }

        controller.Move(velocity * Time.deltaTime);

        spriteRenderer.transform.rotation = Quaternion.LookRotation(
            cameraTransform.forward,
            cameraTransform.up
        );
    }
}
