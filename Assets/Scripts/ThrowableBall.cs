using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableBall : ThrowableObject
{
    private CatchingMinigame con;
    public void SetController(CatchingMinigame con)
    {
        this.con = con;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            con.OnCatch();
        }
        else
        {
            con.OnBallDropped();
        }
    }
}
