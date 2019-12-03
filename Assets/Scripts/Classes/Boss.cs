using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Boss : Enemy
{
    public virtual IEnumerator StartCutScene()
    {
        yield break;
    }

    public virtual IEnumerator EndCutScene()
    {
        yield break;
    }
}
