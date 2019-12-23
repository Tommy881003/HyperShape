using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FillPath : MonoBehaviour
{
    private PlayerController player;
    private bool currentStatus = false;
    private SpriteRenderer sr;
    private BoxCollider2D box;
    private LineRenderer lr;
    private static Gradient gradient = null, battleGrad = null;

    private void Awake()
    {
        if (battleGrad == null)
        {
            battleGrad = new Gradient();
            battleGrad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
            );
        }
        else
            return;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        sr = this.transform.Find("Map").gameObject.GetComponent<SpriteRenderer>();
        BoxCollider2D[] boxes = this.transform.Find("Map").GetComponents<BoxCollider2D>();
        foreach(BoxCollider2D temp in boxes)
        {
            if (temp.usedByComposite == false)
            {
                box = temp;
                break;
            }
        }
        lr = GetComponent<LineRenderer>();
        if (gradient == null)
            gradient = lr.colorGradient;
        Invoke("DrawLine", 0.2f);
    }

    void DrawLine()
    {
        Vector3[] lines = new Vector3[4];
        lines[0] = new Vector3(transform.position.x + transform.localScale.x * 2, transform.position.y + transform.localScale.y * 2);
        lines[1] = new Vector3(transform.position.x - transform.localScale.x * 2, transform.position.y + transform.localScale.y * 2);
        lines[2] = new Vector3(transform.position.x - transform.localScale.x * 2, transform.position.y - transform.localScale.y * 2);
        lines[3] = new Vector3(transform.position.x + transform.localScale.x * 2, transform.position.y - transform.localScale.y * 2);
        lr.positionCount = 4;
        lr.SetPositions(lines);
        lr.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentStatus != player.isBattling)
            changeColor();
    }

    void changeColor()
    {
        if(currentStatus == false)
        {
            sr.DOFade(1, 0.5f);
            box.enabled = true;
            currentStatus = true;
            lr.colorGradient = battleGrad;
        }
        else
        {
            sr.DOFade(0, 0.5f);
            box.enabled = false;
            currentStatus = false;
            lr.colorGradient = gradient;
        }
    }
}
