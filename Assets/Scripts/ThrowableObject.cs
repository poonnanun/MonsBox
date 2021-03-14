using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    public static ThrowableObject Instance;

    private Vector3 startPos, endPos, direction;
    private float timeStart, timeEnd, timeInterval;
    private float throwForceXY = 0.25f;
    private float lastThrownZ;

    private Rigidbody rb;

    private bool allowThrow;

    public float Sensitivity { get => throwForceXY; set => throwForceXY = value; }

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        allowThrow = false;
    }
    public void StartTouch(Vector3 touchPos)
    {
        timeStart = Time.deltaTime;
        startPos = touchPos;
    }
    public void StopTouch(Vector3 touchPos, float zPos, float xPos)
    {
        lastThrownZ = zPos;
        timeEnd = Time.deltaTime;
        endPos = touchPos;
        timeInterval = timeEnd - timeStart;
        direction = startPos - endPos;

        rb.isKinematic = false;
        rb.AddForce((-direction.x * throwForceXY)+xPos, -direction.y * throwForceXY, zPos);
    }
    public  void DevDrop()
    {
        rb.isKinematic = false;
        rb.AddForce(Vector3.forward, ForceMode.Impulse);
    }
    public void SetAllowThrow(bool allow)
    {
        allowThrow = allow;
    }
    public void SetLocation()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }
    public float GetLastThrownZ()
    {
        return lastThrownZ;
    }
    public bool IsMoving()
    {
        return !rb.IsSleeping();
    }
}
