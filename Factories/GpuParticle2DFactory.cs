using Godot;

namespace GodotUtils;

public static class GpuParticle2DFactory
{
    public static GpuParticles2D OneShot(Node parent, PackedScene particleScene)
    {
        GpuParticles2D particles = particleScene.Instantiate<GpuParticles2D>();
        particles.OneShot = true;
        particles.Finished += particles.QueueFree;
        parent.AddChild(particles);
        return particles;
    }

    public static GpuParticles2D Looping(Node parent, PackedScene particleScene)
    {
        GpuParticles2D particles = particleScene.Instantiate<GpuParticles2D>();
        particles.OneShot = false;
        parent.AddChild(particles);
        return particles;
    }
}
