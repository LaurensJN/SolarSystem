using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Planet : MonoBehaviour
{
    // GameObject of planet
    GameObject planetGO;

    // Parameters
    public float diameter;              // Diameter of planet
    public float tilt;                  // Rotation of planet axis
    public float axialRotationDays;         // Days it takes to rotate around axis
    public float sunOrbit;              // Days it takes for one orbit around sun
    public float AU;                    // Relative distance semi-major axis compared to earth
    public float eccentricity;          // Eccentricity of rotation around the sun

    // Global properties
    SolarSystemSettings globalProperties;
    float planetScale;
    float distanceScale;
    float framesPerDay;

    // Derived properties orbit
    Vector3 center;                     // Center of orbit
    float semiMajorAxis;                // First axis of orbital ellipse
    float semiMinorAxis;                // Second axis of orbital ellipse
    float meanRadius;                   // Average radius of ellipse
    float phase;                        // Current position on ellipse (0-2 pi)
    float orbitalSpeedConstant;         // Constant transforming speed to phase change

    // Derived properties axial rotation
    // Set tilt and axial rotation of planet
    Vector3 deltaAxialRotation;


    // Start is called before the first frame update
    void Start()
    {
        globalProperties = GetComponentInParent<SolarSystemSettings>();
        globalProperties.PropertyChanged += OnGlobalPropertyChange;

        // Set global properties
        planetScale = globalProperties.PlanetScale;
        distanceScale = globalProperties.DistanceScale;
        framesPerDay = globalProperties.FramesPerDay;

        // Set planet properties
        planetGO = CreatePlanet(name, diameter, distanceScale, planetScale, transform);
        SetAxialProperties(transform, tilt, axialRotationDays, framesPerDay);

        // Set orbital properties
        SetOrbitalProperties(distanceScale, framesPerDay, sunOrbit, AU, eccentricity);
    }

    // Update is called once per frame
    void Update()
    {
        // Set new position of planet object around sun
        transform.position = CalculateNewPosition();

        // Set planets axial rotation
        planetGO.transform.Rotate(deltaAxialRotation, Space.Self);
    }

    private void SetAxialProperties(Transform transform, float tilt, float axialRotationDays, float framesPerDay)
    {
        // Set tilt and axial rotation of planet
        transform.rotation = Quaternion.Euler(0, 0, tilt);
        deltaAxialRotation = new Vector3(0, (360 / (axialRotationDays * framesPerDay)), 0);
    }
    
    void OnGlobalPropertyChange(object sender, PropertyChangedEventArgs property)
    {
        switch (property.PropertyName)
        {
            case "PlanetScale":
                float newPlanetScale = (sender as SolarSystemSettings).PlanetScale;
                planetGO.transform.localScale *= newPlanetScale / planetScale;
                Debug.Log(planetScale);
                planetScale = newPlanetScale;
                break;
            case "DistanceScale":
                distanceScale = (sender as SolarSystemSettings).DistanceScale;
                SetOrbitalProperties(distanceScale, framesPerDay, sunOrbit, AU, eccentricity);
                break;
            case "FramesPerDay":
                float newFramesPerDay = (sender as SolarSystemSettings).FramesPerDay;
                deltaAxialRotation *= framesPerDay / newFramesPerDay;
                orbitalSpeedConstant *= framesPerDay / newFramesPerDay;
                framesPerDay = newFramesPerDay;
                break;
        }

    }


    /// <summary>
    /// Calculate all orbital properties including:
    /// Semi-major and semiminor axis: http://hyperphysics.phy-astr.gsu.edu/hbase/Math/ellipse.html
    /// Center of ellipse rotation: http://hyperphysics.phy-astr.gsu.edu/hbase/Math/ellipse.html
    /// Mean radius of ellipse: https://www.vcalc.com/wiki/vCalc/Ellipse+-+Mean+Radius
    /// Orbital speed constant, or velocity phase change constant
    /// </summary>
    /// <param name="properties"></param>
    /// <param name="sunOrbit"></param>
    /// <param name="AU"></param>
    /// <param name="eccentricity"></param>
    void SetOrbitalProperties(float distanceScale, float framesPerDay, float sunOrbit, float AU, float eccentricity)
    {
        // Set semi-minor axis by major axis and eccentricity
        semiMajorAxis = AU * 149597871 * distanceScale;
        semiMinorAxis = Mathf.Sqrt(Mathf.Pow(semiMajorAxis, 2) * (1 - eccentricity));

        // Only if planet is not yet instantiated
        if (float.IsNaN(phase))
        {
            phase = UnityEngine.Random.Range(0, Mathf.PI);
        }

        // Set center of rotation
        center = new Vector3(semiMajorAxis * eccentricity, 0, 0);

        // Set starting position of orbit around sun
        transform.position = new Vector3(semiMajorAxis * Mathf.Cos(phase), 0, semiMinorAxis * Mathf.Sin(phase)) - center;

        // Set mean radius of ellipse
        meanRadius = (2 * semiMajorAxis + semiMinorAxis) / 3;

        // Calculate the velocity of phase change constant  
        orbitalSpeedConstant = CalculateOrbitalSpeedConstant(semiMajorAxis, sunOrbit, framesPerDay, distanceScale);
    }

    /// <summary>
    /// Returns new position based on the phase change
    /// Using Keppler's second law of motion and instanteneous velocity
    /// https://en.wikipedia.org/wiki/Kepler%27s_laws_of_planetary_motion
    /// https://en.wikipedia.org/wiki/Orbital_speed
    /// </summary>
    /// <returns></returns>
    private Vector3 CalculateNewPosition()
    {
        // Kepler's second law of motion
        double r = Radius(transform.position, globalProperties.DistanceScale);
        double a = semiMajorAxis / globalProperties.DistanceScale;

        // Use instantaneous velocity to calculate position in next frame
        float instantaneousVelocity = (float)Math.Sqrt((meanRadius * ((2 / r) - (1 / a))));
        phase += instantaneousVelocity * orbitalSpeedConstant;
        return new Vector3(semiMajorAxis * Mathf.Cos(phase), 0, semiMinorAxis * Mathf.Sin(phase)) - center;
    }

    /// <summary>
    /// Returns orbital speed constant using the mean velocity of the planet
    /// Mean velocity: https://en.wikipedia.org/wiki/Orbital_speed
    /// </summary>
    /// <param name="semiMajorAxis"></param>
    /// <param name="semiMinorAxis"></param>
    /// <param name="sunOrbit"></param>
    /// <param name="framesPerDay"></param>
    /// <param name="distanceScale"></param>
    /// <returns></returns>
    private float CalculateOrbitalSpeedConstant(float semiMajorAxis, float sunOrbit, float framesPerDay,  double distanceScale)
    {
        // Mean velocity
        float a = (float)(semiMajorAxis / distanceScale);
        float meanVelocity = Mathf.Sqrt(meanRadius / a);

        // Use mean velocity to calculte the velocity of phase change constant
        return ((1 / (sunOrbit * framesPerDay)) * 2 * Mathf.PI) / meanVelocity;
    }

    GameObject CreatePlanet(string name, float diameter, float distanceScale, float planetScale, Transform parent)
    {
        GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        planet.transform.parent = parent;
        planet.name = $"{name}Planet";
        planet.transform.localScale *= diameter * distanceScale * planetScale;
        Debug.Log(planetScale);

        // Load texture with same name  
        Texture2D texture = Resources.Load<Texture2D>($"Textures/{name}");
        Renderer renderer = planet.GetComponent<Renderer>();
        renderer.material.mainTexture = texture;
        return planet;
    }

    Func<Vector3, double, double> SquaredRadius = (position, scale) => Math.Pow(position.magnitude, 2) / scale;
    Func<Vector3, double, double> Radius = (position, scale) => position.magnitude / scale;

}
