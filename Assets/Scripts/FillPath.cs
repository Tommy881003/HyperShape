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
        }
        else
        {
            sr.DOFade(0, 0.5f);
            box.enabled = false;
            currentStatus = false;
        }
    }
}
