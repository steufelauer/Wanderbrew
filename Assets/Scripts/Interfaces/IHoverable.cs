using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoverable
{


    void InitiateHover(Vector3 pos);
    void InitiateHover(Vector3 pos, string txt);
    void UpdateHover(Vector3 pos);
    void DisableHover();
}
