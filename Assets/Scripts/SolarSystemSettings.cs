using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;


[ExecuteInEditMode]
public class SolarSystemSettings : MonoBehaviour, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    // [Range(0.00000000001f, 1)]
    float planetScale = 0.00001f;

    // [Range(0.00000001f, 1f)]
    float distanceScale = 0.001f;

    bool sqrtDistance;

    // [Range(0.0001f, 500)]
    float framesPerDay = 30f;

    public float PlanetScale
    {
        get { return planetScale; }
        set {   planetScale = value;
                OnPropertyChanged();
        }
    }

    public float DistanceScale
    {
        get { return distanceScale; }
        set
        {
            distanceScale = value;
            OnPropertyChanged();
        }
    }
    public float FramesPerDay
    {
        get { return framesPerDay; }
        set
        {
            framesPerDay = value;
            OnPropertyChanged();
        }
    }

    public void SpeedToFPS(float speed)
    {
        FramesPerDay = 500 / speed;
    }
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
