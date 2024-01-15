using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingLaser : MonoBehaviour
{
    public Transform aimStartPoint;
    public Transform barrelStartPoint;
    private Vector3 endPoint;
    private LineRenderer laserLine;
    public GameObject laserDot;
    private float lineWidth = 0.004f;
    void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        laserLine.startWidth = lineWidth;
        laserLine.endWidth = lineWidth;
    }

    void Update()
    {
        RaycastHit hit;
        int layerMask = ~0; // Intersect with everything.
        // Aim down barrel with ray, but draw from laser pointer origin instead.
        if (Physics.Raycast(barrelStartPoint.position, barrelStartPoint.forward, out hit))
        {
            endPoint = hit.point;
            laserDot.transform.position = endPoint;
            laserDot.SetActive(true);
            laserLine.enabled = true;
            laserLine.SetPosition(0, aimStartPoint.position);
            laserLine.SetPosition(1, endPoint);
        }
        else
        {
            laserDot.SetActive(false);
            laserLine.enabled = false;
        }
    }
}
