using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
public class GeneralFRCAcademy : Academy {

    public override void AcademyReset()
    {
        base.AcademyReset();
       // FindObjectOfType<MovementAgents>().lesson = (int)resetParameters["lesson"];
    }
}
