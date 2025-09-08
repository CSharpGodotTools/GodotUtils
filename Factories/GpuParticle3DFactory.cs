using Godot;

namespace GodotUtils;

public static class GpuParticle3DFactory
{
    public static GpuParticles3D OneShot(Node parent, PackedScene particleScene)
    {
        GpuParticles3D particles = particleScene.Instantiate<GpuParticles3D>();
        particles.OneShot = true;
        particles.Finished += particles.QueueFree;
        parent.AddChild(particles);
        return particles;
    }
}
