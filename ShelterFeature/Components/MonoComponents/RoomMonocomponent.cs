using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMonocomponent : MonoBehaviour
{
    [SerializeField] private Transform floorEdge;
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    public Transform FloorEdge => floorEdge;
    public Transform LeftEdge => leftEdge;
    public Transform RightEdge => rightEdge;
}
