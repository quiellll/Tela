using UnityEngine;

/// <summary>
/// Clase que representa el viento en la escena.
/// Contiene par�metros para ajustar las fuerzas del viento.
/// Genera ruido mediante Perlin Noise para simular la variaci�n del viento.
/// </summary>
public class Wind
{
    private Vector3 _baseWindForce;                                                         // Fuerza base del viento.
    private Vector3 _noiseOffset;                                                           // Offset del ruido de Perlin.

    private float _windVariation;                                                           // Variaci�n de la fuerza del viento.

    public Wind (Vector3 baseWindForce, float windVariation = 0.5f)                         // Constructor de la clase Wind.
    {
        _baseWindForce = baseWindForce;
        _noiseOffset = new Vector3(Random.Range(0f, 100f), 
                                   Random.Range(0f, 100f), 
                                   Random.Range(0f, 100f));
        
        _windVariation = windVariation;
    }
    public void ModifyVariation(float newVariation) { _windVariation = newVariation; }      // Actualiza la variaci�n de la fuerza del viento.

    public void UpdateWind(float time)                                                      // Actualiza la fuerza del viento.
    {
        _baseWindForce.x = Mathf.PerlinNoise(time * _windVariation 
                                             + _noiseOffset.x, 0) * 2 - 1;
        _baseWindForce.y = Mathf.PerlinNoise(time * _windVariation 
                                             + _noiseOffset.y, 0) * 2 - 1;
        _baseWindForce.z = Mathf.PerlinNoise(time * _windVariation 
                                             + _noiseOffset.z, 0) * 2 - 1;

        float baseStrength = 1.0f;
        _baseWindForce *= baseStrength;
    }
}