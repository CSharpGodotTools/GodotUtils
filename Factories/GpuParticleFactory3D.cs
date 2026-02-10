using Godot;

namespace GodotUtils;

/// <summary>
/// Factory helpers for GPU particles (3D).
/// </summary>
public static class GpuParticleFactory3D
{
    /// <summary>
    /// Instantiates a one-shot GPU particle and frees it when finished.
    /// </summary>
    public static GpuParticles3D OneShot(Node parent, PackedScene particleScene)
    {
        GpuParticles3D particles = particleScene.Instantiate<GpuParticles3D>();
        particles.OneShot = true;
        particles.Finished += particles.QueueFree;
        parent.AddChild(particles);
        return particles;
    }
}
