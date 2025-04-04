using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float distance = 5f;
    
    private float _azimuthAngle;
    private float _polarAngle;

    private void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        Debug.Log($"## X: {mouseX} Y: {mouseY}");
        
        _polarAngle += mouseY * rotationSpeed * Time.deltaTime;
        _azimuthAngle += mouseX * rotationSpeed * Time.deltaTime;
        
        var cartesianPosition = GetCameraPosition(distance, _azimuthAngle, _polarAngle);
        var cameraPosition = target.position - cartesianPosition;
        
        transform.position = cameraPosition;
        transform.LookAt(target);
    }

    Vector3 GetCameraPosition(float r, float azimuthAngle, float polarAngle)
    {
        float b = r * Mathf.Cos(azimuthAngle * Mathf.Deg2Rad);
        float z = b * Mathf.Cos(polarAngle * Mathf.Deg2Rad);
        float y = r * Mathf.Sin(azimuthAngle * Mathf.Deg2Rad);
        float x = b * Mathf.Sin(polarAngle * Mathf.Deg2Rad);
        
        return new Vector3(x, y, z);
    }
}
