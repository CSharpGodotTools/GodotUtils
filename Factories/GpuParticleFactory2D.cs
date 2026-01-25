using Godot;

namespace GodotUtils;

public static class GpuParticleFactory2D
{
    /// <summary>
    /// Instantiates a GPUParticles2D scene as a one-shot effect, frees it automatically when
    /// emission finishes, and adds it as a child of the given parent node.
    /// </summary>
    /// <param name="parent">The node that will own the instantiated particle node.</param>
    /// <param name="particleScene">A PackedScene that instantiates to a GPUParticles2D.</param>
    /// <returns>The instantiated and configured GPUParticles2D.</returns>
    public static GpuParticles2D OneShot(Node parent, PackedScene particleScene)
    {
        GpuParticles2D particles = particleScene.Instantiate<GpuParticles2D>();
        particles.OneShot = true;
        particles.Finished += particles.QueueFree;
        parent.AddChild(particles);
        return particles;
    }

    /// <summary>
    /// Instantiates a GPUParticles2D scene as a looping effect, does not automatically free it,
    /// and adds it as a child of the given parent node.
    /// </summary>
    /// <param name="parent">The node that will own the instantiated particle node.</param>
    /// <param name="particleScene">A PackedScene that instantiates to a GPUParticles2D.</param>
    /// <returns>The instantiated and configured GPUParticles2D.</returns>
    public static GpuParticles2D Looping(Node parent, PackedScene particleScene)
    {
        GpuParticles2D particles = particleScene.Instantiate<GpuParticles2D>();
        particles.OneShot = false;
        parent.AddChild(particles);
        return particles;
    }
}
