using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Rampage")]
public class RampageAbility : Ability

{
    public float speedMult, healthMult;


    public override void Activate(GameObject parent)
    {
        PlayerHealth health = parent.GetComponent<PlayerHealth>();
        Movement movement = parent.GetComponent<Movement>();
        
        health.baseHealth = (int)(healthMult * health.baseHealth);
        movement.baseSpeed = (int)(speedMult * movement.baseSpeed);
        
        
    }

    public override void Deactivate(GameObject parent)
    {
        PlayerHealth health = parent.GetComponent<PlayerHealth>();
        Movement movement = parent.GetComponent<Movement>();
        
        
        health.baseHealth = (int)(health.baseHealth / healthMult);
        movement.baseSpeed = (int)(movement.baseSpeed / speedMult);
    }
}
