using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    SolarSystemSettings globalProperties;

    public float sunOrbit;              // Days it takes for one orbit around sun
    public float AU;                    // Relative distance semi-major axis compared to earth
    public float eccentricity;          // Eccentricity of rotation around the sun

    // Derived properties orbit
    Vector3 center;                     // Center of orbit
    float semiMajorAxis;                // First axis of orbital ellipse
    float semiMinorAxis;                // Second axis of orbital ellipse
    float meanRadius;                   // Average radius of ellipse
    float phase;                        // Current position on ellipse (0-2 pi)
    float orbitalSpeedConstant;         // Constant transforming speed to phase change


    // Start is called before the first frame update
    void Start()
    {
        globalProperties = GetComponentInParent<SolarSystemSettings>();
        SetOrbitalProperties(globalProperties, sunOrbit, AU, eccentricity);

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = CalculateNewPosition();
    }

    void SetOrbitalProperties(SolarSystemSettings properties, float sunOrbit, float AU, float eccentricity)
    {
        // Set semi-minor axis by major axis and eccentricity
        // http://hyperphysics.phy-astr.gsu.edu/hbase/Math/ellipse.html
        semiMajorAxis = (float)(AU * 149597871 * properties.distanceScale);
        semiMinorAxis = Mathf.Sqrt(Mathf.Pow(semiMajorAxis, 2) * (1 - eccentricity));
        phase = UnityEngine.Random.Range(0, Mathf.PI);

        // Set center of rotation
        // http://hyperphysics.phy-astr.gsu.edu/hbase/Math/ellipse.html
        center = new Vector3(semiMajorAxis * eccentricity, 0, 0);

        // Set starting position of orbit around sun
        transform.position = new Vector3(semiMajorAxis * Mathf.Cos(phase), 0, semiMinorAxis * Mathf.Sin(phase)) - center;

        // Set mean radius of ellipse
        // https://www.vcalc.com/wiki/vCalc/Ellipse+-+Mean+Radius
        meanRadius = (2 * semiMajorAxis + semiMinorAxis) / 3;

        // Calculate the velocity of phase change constant  
        orbitalSpeedConstant = CalculateOrbitalSpeedConstant(semiMajorAxis, semiMinorAxis, sunOrbit, properties.framesPerDay, properties.distanceScale);
    }

    private Vector3 CalculateNewPosition()
    {
        // Kepler's second law of motion
        // https://en.wikipedia.org/wiki/Kepler%27s_laws_of_planetary_motion
        double r = Radius(transform.position, globalProperties.distanceScale);
        double a = semiMajorAxis / globalProperties.distanceScale;

        // Use instantaneous velocity to calculate position in next frame
        // https://en.wikipedia.org/wiki/Orbital_speed
        float instantaneousVelocity = (float)Math.Sqrt((meanRadius * ((2 / r) - (1 / a))));
        phase += instantaneousVelocity * orbitalSpeedConstant;
        return new Vector3(semiMajorAxis * Mathf.Cos(phase), 0, semiMinorAxis * Mathf.Sin(phase)) - center;
    }

    private float CalculateOrbitalSpeedConstant(float semiMajorAxis, float semiMinorAxis, float sunOrbit, float framesPerDay,  double distanceScale)
    {
        // Mean velocity
        // https://en.wikipedia.org/wiki/Orbital_speed
        float a = (float)(semiMajorAxis / distanceScale);
        float meanVelocity = Mathf.Sqrt(meanRadius / a);

        // Use mean velocity to calculte the velocity of phase change constant
        return ((1 / (sunOrbit * framesPerDay)) * 2 * Mathf.PI) / meanVelocity;
    }

    Func<Vector3, double, double> SquaredRadius = (position, scale) => Math.Pow(position.magnitude, 2) / scale;
    Func<Vector3, double, double> Radius = (position, scale) => position.magnitude / scale;

}
