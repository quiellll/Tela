using UnityEngine;

public class Wind
{
    private Vector3 _baseWindForce = new(1.0f, 0f, 0f);
    private Vector3 _noiseOffset = Vector3.zero;

    private float _windVariation;

    public Wind (Vector3 baseWindForce, float windVariation = 0.5f)
    {
        _baseWindForce = baseWindForce;
        _noiseOffset = new Vector3(Random.Range(0f, 100f), Random.Range(0f, 100f), Random.Range(0f, 100f));
        
        _windVariation = windVariation;
    }

    public void UpdateWind(float time)
    {
        _baseWindForce.x = Mathf.PerlinNoise(time * _windVariation + _noiseOffset.x, 0) * 2 - 1;
        _baseWindForce.y = Mathf.PerlinNoise(time * _windVariation + _noiseOffset.y, 0) * 2 - 1;
        _baseWindForce.z = Mathf.PerlinNoise(time * _windVariation + _noiseOffset.z, 0) * 2 - 1;

        float baseStrength = 1.0f;
        _baseWindForce *= baseStrength;
    }

    public void UpdateVariation(float newVariation) { _windVariation = newVariation; }
}