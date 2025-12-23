using System.Collections.Generic;
using UnityEngine;

public class FreezeSnowOnCollision : MonoBehaviour
{
    ParticleSystem ps;
    List<ParticleCollisionEvent> collisionEvents;
    ParticleSystem.Particle[] particles;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    void OnParticleCollision(GameObject other)
    {
        int eventCount = ps.GetCollisionEvents(other, collisionEvents);
        int particleCount = ps.GetParticles(particles);

        for (int i = 0; i < eventCount; i++)
        {
            Vector3 hitPos = collisionEvents[i].intersection;

            // Find closest particle to collision point
            for (int p = 0; p < particleCount; p++)
            {
                if (Vector3.Distance(particles[p].position, hitPos) < 0.01f)
                {
                    particles[p].velocity = Vector3.zero;
                    particles[p].angularVelocity = 0f;
                    particles[p].rotation3D = Vector3.zero;
                    break;
                }
            }
        }

        ps.SetParticles(particles, particleCount);
    }
}