using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private int ROWS = 8;
    private int COLS = 11;
    private float alphaReduction = 0.002f;
    private int blocksRemaining = 0;
    public Transform origin;
    public GameObject ForceField;
    public GameObject BlockPrefab;
    public GameObject BrokenBlockPrefab;
    public GameObject[] BrokenBlockPrefabs;
    private int brokenBlockPrefabCount;
    public GameObject SmokePuff;
    public GameObject SparksPrefab;

    private AudioSource crunchSound;
    private AudioSource hummingSound;

    // During play, collect up all the blocks to delete here and do it all at once.
    List<GameObject> toDelete = new List<GameObject>();

    void Start()
    {
        brokenBlockPrefabCount = BrokenBlockPrefabs.Length - 1;
        AudioSource[] sounds = GetComponents<AudioSource>();
        crunchSound = sounds[0];
        hummingSound = sounds[1];
        CreateBoard();
        blocksRemaining = FindObjectsOfType<BlockValues>().Length;
    }

    private void CreateBoard()
    {
        int rowCount = 0;
        float span = 0.5f;
        float gap = 0.0025f; // Makes block collapse unsticky.
        float z = origin.position.z;
        for (int row = 0; row < ROWS; row++)
        {
            float x = origin.position.x;
            float y = origin.position.y + span * rowCount;
            for (int col = 0; col < COLS; col++)
            {
                int val = Random.Range(1, 4);
                GameObject block = Instantiate(BlockPrefab, new Vector3(x, y + 0.002f * row, z), Quaternion.identity);
                block.GetComponent<BlockValues>().Init(val);
                x += span + gap;
            }
            rowCount++;
        }
    }

    public void Play(GameObject block)
    {
        crunchSound.Play();
        ClearBlockAndRecurse(block);
        // Destroy all the like-numbered blocks we found in the neighborhood.
        foreach (var blk in toDelete)
        {
            Explode(blk);
            blocksRemaining--;
        }
        toDelete.Clear();
    }


    private void Explode(GameObject block)
    {
        // Make the explosion position back behind the block's center to force pieces forward.
        Vector3 explosionPosition = new Vector3(block.transform.position.x, block.transform.position.y, block.transform.position.z + 0.5f);
        GameObject randomBrokenBlock = BrokenBlockPrefabs[(int)Random.Range(0, brokenBlockPrefabCount)];
        GameObject brokenBlock = Instantiate(randomBrokenBlock, block.transform.position, Quaternion.Euler(180,0,0));
        Transform t = brokenBlock.transform;
        foreach (Transform child in t)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRB))
            {
                childRB.AddExplosionForce(300f, explosionPosition, 1f);
            }
        }
        Instantiate(SmokePuff, block.transform.position, Quaternion.Euler(-180f,0,0));
        Destroy(block);
        Destroy(brokenBlock, 1f);
    }

    private void ClearBlockAndRecurse(GameObject block)
    {
        int value = block.GetComponent<BlockValues>().GetValue();
        block.GetComponent<BlockValues>().ClearValue();
        toDelete.Add(block);
        // Check left
        GameObject toLeft = ObtainNeighbor(block, -block.transform.right);
        if (toLeft && toLeft.GetComponent<BlockValues>().GetValue() == value)
            ClearBlockAndRecurse(toLeft);

        // Check right
        GameObject toRight = ObtainNeighbor(block, block.transform.right);
        if (toRight && toRight.GetComponent<BlockValues>().GetValue() == value)
            ClearBlockAndRecurse(toRight);

        // Check up
        GameObject upAbove = ObtainNeighbor(block, block.transform.up);
        if (upAbove && upAbove.GetComponent<BlockValues>().GetValue() == value)
            ClearBlockAndRecurse(upAbove);

        // Check down
        GameObject downBelow = ObtainNeighbor(block, -block.transform.up);
        if (downBelow && downBelow.GetComponent<BlockValues>().GetValue() == value)
            ClearBlockAndRecurse(downBelow);
    }

   private GameObject ObtainNeighbor(GameObject block, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(block.transform.position, direction, out hit, 0.75f))
        {
            if (hit.collider.gameObject.CompareTag("GameBlock"))
                return hit.collider.gameObject;
        }
        return null;
    }

    // debug with mouse here in Update
    private void Update()
    {
        MousePlay();
        ForceFieldStatus();
    }

    void ForceFieldStatus()
    {
        if (ForceField == null)
            return;

        if (blocksRemaining == 0)
        {
            float alpha = ForceField.GetComponent<Renderer>().material.color.a;
            if (alpha <= 0.01)
            {
                Destroy(ForceField);
                hummingSound.Stop();
            }
            else
            {
                Color c = ForceField.GetComponent<Renderer>().material.color;
                c.a -= alphaReduction;
                ForceField.GetComponent<Renderer>().material.color = c;
            }
        }
    }
    // You can play directly in unity without a headset, but it is not as much fun.
    private void MousePlay()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
            return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseHit = mouse.position.ReadValue();
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mouseHit);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("GameBlock"))
                {
                    Play(hit.collider.gameObject);
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
}
