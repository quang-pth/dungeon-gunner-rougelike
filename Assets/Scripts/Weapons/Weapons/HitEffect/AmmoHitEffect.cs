using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
    ParticleSystem hitEffectParticleSystem;

    private void Awake() {
        hitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    public void SetHitEffect(AmmoHitEffectSO ammoHitEffectSO) {
        SetHitEffectColorGradient(ammoHitEffectSO.colorGradient);

        SetHitEffectParticleStartingValues(ammoHitEffectSO.duration, ammoHitEffectSO.startParticleSize, ammoHitEffectSO.startParticleSpeed,
                        ammoHitEffectSO.startLifeTime, ammoHitEffectSO.effectGravity, ammoHitEffectSO.maxParticleNumber);

        SetHitEffectParticleEmission(ammoHitEffectSO.emissionRate, ammoHitEffectSO.burstParticleNumber);

        SetHitEffectParticleSprite(ammoHitEffectSO.sprite);

        SetHitEffectVelocityOverLifeTime(ammoHitEffectSO.velocityOverLifeTimeMin, ammoHitEffectSO.velocityOverLifeTimeMax); 
    }

    private void SetHitEffectColorGradient(Gradient colorGradient) {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = hitEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = colorGradient;
    }

    private void SetHitEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed, 
            float startLifeTime, float effectGravity, int maxParticleNumber) 
    {
        ParticleSystem.MainModule mainModule = hitEffectParticleSystem.main;

        mainModule.duration = duration;
        mainModule.startSize = startParticleSize;
        mainModule.startSpeed = startParticleSpeed;
        mainModule.startLifetime = startLifeTime;
        mainModule.gravityModifier = effectGravity;
        mainModule.maxParticles = maxParticleNumber;
    }

    private void SetHitEffectParticleEmission(float emissionRate, int burstParticleNumber) {
        ParticleSystem.EmissionModule emissionModule = hitEffectParticleSystem.emission;

        emissionModule.rateOverTime = emissionRate;
        // Set burst effect
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);
    }
    
    private void SetHitEffectParticleSprite(Sprite sprite) {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = hitEffectParticleSystem.textureSheetAnimation;
        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    private void SetHitEffectVelocityOverLifeTime(Vector3 velocityOverLifeTimeMin, Vector3 velocityOverLifeTimeMax) {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = hitEffectParticleSystem.velocityOverLifetime;
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

