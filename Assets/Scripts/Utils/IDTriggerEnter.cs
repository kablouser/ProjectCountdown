using UnityEngine;

public class IDTriggerEnter : IDComponent
{
    public Main main;

    public void OnTriggerEnter2D(Collider2D collider)
    {
        //main.ProcessTriggerEnter(this, collider);
    }
}
