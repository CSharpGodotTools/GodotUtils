using Godot;

namespace GodotUtils;

/// <summary>
/// Factory helpers for GPU particles (2D).
/// </summary>
public static class GpuParticleFactory2D
{
    /// <summary>
    /// Instantiates a one-shot GPU particle and frees it when finished.
    /// </summary>
    public static GpuParticles2D OneShot(Node parent, PackedScene particleScene)
    {
        GpuParticles2D particles = particleScene.Instantiate<GpuParticles2D>();
        particles.OneShot = true;
        particles.Finished += particles.QueueFree;
        parent.AddChild(particles);
        return particles;
    }

    /// <summary>
    /// Instantiates a looping GPU particle without auto-free.
    /// </summary>
    public static GpuParticles2D Looping(Node parent, PackedScene particleScene)
    {
        GpuParticles2D particles = particleScene.Instantiate<GpuParticles2D>();
        particles.OneShot = false;
        parent.AddChild(particles);
        return particles;
    }
}
