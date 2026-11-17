using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsDemo : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 8f;
    public int fontSize = 28;

    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer sr;

    ContactPoint2D[] contacts = new ContactPoint2D[8];
    Collider2D[] overlaps = new Collider2D[8];
    ContactFilter2D filter;
    GUIStyle style;

    float move;
    bool jumpQueued, grounded, mouseOver;
    int contactCount, overlapCount;
    string overlapNames = "";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        filter = new ContactFilter2D();
        filter.NoFilter();
        Time.fixedDeltaTime = 0.1f; // I slow physics so interpolation is visible
    }

    void OnDestroy()
    {
        Time.fixedDeltaTime = 0.02f;
    }

    void Update()
    {
        var k = Keyboard.current;
        move = (k.dKey.isPressed ? 1 : 0) - (k.aKey.isPressed ? 1 : 0);
        if (k.spaceKey.wasPressedThisFrame) jumpQueued = true;

        // I cycle None -> Interpolate -> Extrapolate with I
        if (k.iKey.wasPressedThisFrame)
            rb.interpolation = (RigidbodyInterpolation2D)(((int)rb.interpolation + 1) % 3);

        // I flip between slow and normal physics with T
        if (k.tKey.wasPressedThisFrame)
            Time.fixedDeltaTime = Time.fixedDeltaTime > 0.05f ? 0.02f : 0.1f;

        // OverlapPoint: is the mouse inside this rigidbody's colliders
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseOver = rb.OverlapPoint(mouse);

        // OverlapCollider: what is my collider overlapping right now
        overlapCount = Physics2D.OverlapCollider(col, filter, overlaps);
        overlapNames = "";
        for (int i = 0; i < overlapCount; i++) overlapNames += overlaps[i].name + " ";

        sr.color = mouseOver ? Color.yellow : (grounded ? Color.green : Color.white);
    }

    void FixedUpdate()
    {
        // GetContacts: I treat any contact pointing up as ground
        contactCount = rb.GetContacts(contacts);
        grounded = false;
        for (int i = 0; i < contactCount; i++)
            if (contacts[i].normal.y > 0.5f) grounded = true;

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
        if (jumpQueued && grounded) rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpQueued = false;
    }

    void OnGUI()
    {
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.fontSize = fontSize;
        }
        GUI.Label(new Rect(10, 10, 1200, 400),
            "Interpolation (I): " + rb.interpolation +
            "\nFixed timestep (T): " + Time.fixedDeltaTime +
            "\nGetContacts: " + contactCount + " grounded: " + grounded +
            "\nOverlapPoint (mouse over me): " + mouseOver +
            "\nOverlap hits: " + overlapCount + " " + overlapNames, style);
    }
}