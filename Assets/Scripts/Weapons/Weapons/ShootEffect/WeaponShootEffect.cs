using UnityEngine;

[DisallowMultipleComponent]
public class WeaponShootEffect : MonoBehaviour
{
    private ParticleSystem shootEffectParticleSystem;

    private void Awake() {
        shootEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    public void SetShootEffect(WeaponShootEffectSO weaponShootEffectSO, float aimAngle) {
        SetShootEffectColorGradient(weaponShootEffectSO.colorGradient);

        SetShootEffectParticleStartingValues(weaponShootEffectSO.duration, weaponShootEffectSO.startParticleSize, weaponShootEffectSO.startParticleSpeed,
                        weaponShootEffectSO.startLifeTime, weaponShootEffectSO.effectGravity, weaponShootEffectSO.maxParticleNumber);

        SetShootEffectParticleEmission(weaponShootEffectSO.emissionRate, weaponShootEffectSO.burstParticleNumber);

        SetEmitterRotation(aimAngle);

        SetShootEffectParticleSprite(weaponShootEffectSO.sprite);

        SetShootEffectVelocityOverLifeTime(weaponShootEffectSO.velocityOverLifeTimeMin, weaponShootEffectSO.velocityOverLifeTimeMax);
    }

    private void SetShootEffectColorGradient(Gradient colorGradient) {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = shootEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = colorGradient;
    }

    private void SetShootEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed, 
            float startLifeTime, float effectGravity, int maxParticleNumber) 
    {
        ParticleSystem.MainModule mainModule = shootEffectParticleSystem.main;

        mainModule.duration = duration;
        mainModule.startSize = startParticleSize;
        mainModule.startSpeed = startParticleSpeed;
        mainModule.startLifetime = startLifeTime;
        mainModule.gravityModifier = effectGravity;
        mainModule.maxParticles = maxParticleNumber;
    }

    private void SetShootEffectParticleEmission(int emissionRate, int burstParticleNumber) {
        ParticleSystem.EmissionModule emissionModule = shootEffectParticleSystem.emission;

        emissionModule.rateOverTime = emissionRate;
        // Set burst effect
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);
    }

    private void SetEmitterRotation(float aimAngle) {
        transform.eulerAngles = new Vector3(0f, 0f, aimAngle);
    }

    private void SetShootEffectParticleSprite(Sprite sprite) {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = shootEffectParticleSystem.textureSheetAnimation;
        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    private void SetShootEffectVelocityOverLifeTime(Vector3 velocityOverLifeTimeMin, Vector3 velocityOverLifeTimeMax) {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = shootEffectParticleSystem.velocityOverLifetime;
        // Define min max X velocity
        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants; // set mode to random between two constants
        minMaxCurveX.constantMin = velocityOverLifeTimeMin.x;
        minMaxCurveX.constantMax = velocityOverLifeTimeMax.x;
        velocityOverLifetimeModule.x = minMaxCurveX;
        // Define min max Y velocity
        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants; // set mode to random between two constants
        minMaxCurveY.constantMin = velocityOverLifeTimeMin.y;
        minMaxCurveY.constantMax = velocityOverLifeTimeMax.y;
        velocityOverLifetimeModule.y = minMaxCurveY;
        // Define min max Z velocity
        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants; // set mode to random between two constants
        minMaxCurveZ.constantMin = velocityOverLifeTimeMin.z;
        minMaxCurveZ.constantMax = velocityOverLifeTimeMax.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;
    }
}
