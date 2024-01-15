using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class ZapperController : MonoBehaviour
{
    public InputActionReference zapperFire;
    private AudioSource zot;
    private AudioSource thud;
    public Transform barrelStartPoint;
    private Vector3 endPoint;
    private LineRenderer laserLine;
    public GameObject SparksPrefab;
    public GameObject redFilament;
    public GameObject greenFilament;
    private float lineWidth = 0.01f;
    private float laserLifetime = 0.2f;
    private float laserResetTime = 1.5f;
    private bool laserReady = true;
    public GameManager gameManager;
    RaycastHit hit;
    void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        laserLine.startWidth = lineWidth;
        laserLine.endWidth = lineWidth;
        zapperFire.action.performed += FireLaser;
        AudioSource[] sounds = GetComponents<AudioSource>();
        zot = sounds[0];
        SetBulbStatus(true);
    }

    private void FireLaser(InputAction.CallbackContext obj)
    {
        if (laserReady)
        {
            //int layerMask = ~0;
            if (Physics.Raycast(barrelStartPoint.position, barrelStartPoint.forward, out hit))
            {
                zot.Play();
                endPoint = hit.point;
                laserLine.enabled = true;
                laserLine.SetPosition(0, barrelStartPoint.position);
                laserLine.SetPosition(1, endPoint);
                laserReady = false;
                SetBulbStatus(false);
                StartCoroutine(ResetLaser());
                StartCoroutine(MaintainLaserBeam());
                if (hit.collider.CompareTag("GameBlock"))
                {
                    StartCoroutine(DelayPlay());
                }
                else if (hit.collider.CompareTag("Sparkable"))
                {
                    var normal = hit.normal;
                    var q = Quaternion.FromToRotation(Vector3.forward, normal);
                    Instantiate(SparksPrefab, hit.point, q);
                }
            }
        }
    }

    private void SetBulbStatus(bool status)
    {
        greenFilament.SetActive(status);
        redFilament.SetActive(!status);
    }

    // Delay playing just a brief time to let the sounds stretch a little.
    private IEnumerator DelayPlay()
    {
        yield return new WaitForSeconds(0.1f);
        gameManager.Play(hit.collider.gameObject);
    }

    private IEnumerator ResetLaser()
    {
        yield return new WaitForSeconds(laserResetTime);
        laserReady = true;
        SetBulbStatus(true);
    }

    private IEnumerator MaintainLaserBeam()
    {
        yield return new WaitForSeconds(laserLifetime);
        laserLine.enabled = false;
    }

}
