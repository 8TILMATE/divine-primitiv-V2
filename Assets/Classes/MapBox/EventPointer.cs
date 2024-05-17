using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Examples;
using Mapbox.Utils;
using System.Globalization;
using System;

public class EventPointer : MonoBehaviour
{
    [SerializeField]
    private float RotationSpeed = 50f;

    [SerializeField]
    private float amplitude = 2.0f;

    [SerializeField]
    float frequency = 0.50f;

    LocationStatus playerLocation;
    [SerializeField]
    public Vector2d eventPose;

    void FloatAndRotatePointer()
    {
        transform.Rotate(Vector3.up, 
        RotationSpeed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x,
        (Mathf.Sin(Time.fixedTime * Mathf.PI*frequency)*amplitude)+15, 
         transform.position.z);
    }

    public void OnMouseDown()
    {
        playerLocation = GameObject.Find("Canvas").GetComponent<LocationStatus>();
        var currentPlayerLocation = new GeoCoordinatePortable.GeoCoordinate(playerLocation.GetLocationLat(),
        playerLocation.GetLocationLon());
        var eventLocation = new GeoCoordinatePortable.GeoCoordinate(eventPose[0], eventPose[1]);
        var distance = currentPlayerLocation.GetDistanceTo(eventLocation);
        Debug.Log("Distance Is: " + distance);
        Debug.Log("Clicked!");
    }
}
