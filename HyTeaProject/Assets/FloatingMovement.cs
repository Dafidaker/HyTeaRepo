using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingMovement : MonoBehaviour
{

    [SerializeField] private float _height, _rotation, _speed, _offset, _rotationSpeed;
    [SerializeField] private Transform _floatingObject;
    
    // Update is called once per frame
    void Update()
    {
        _floatingObject.localPosition = new Vector3(0, Mathf.Sin(Time.time*_speed+_offset)*_height, 0);
        _floatingObject.localEulerAngles = new Vector3(0, 0, Mathf.Sin(Time.time*_rotationSpeed+_offset) * _rotation);
    }
}
