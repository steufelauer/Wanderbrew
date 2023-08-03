using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickable
{

    bool IsPickedUp { get; }
    GameObject MyGameObject { get; }


    void PickUp();
    void Release();
    void UpdatePosition(Vector3 pos);

}
