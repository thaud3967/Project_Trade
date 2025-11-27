using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movementInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = 0;
    }


    public void OnMove(InputValue value)
    {

        movementInput = value.Get<Vector2>();
    }

    void Update()
    {
        // 이동 방향 바라보기
        if (movementInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
        }
    }
}