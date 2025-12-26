using Godot;

namespace GodotUtils;

public static class GPUParticles2DExtensions
{
    /// <summary>
    /// Returns the ParticleProcessMaterial assigned to the given GPUParticles2D.
    /// </summary>
    /// <param name="particles">The GPUParticles2D instance to read the process material from.</param>
    /// <returns>The ParticleProcessMaterial used by the particles.</returns>
    public static ParticleProcessMaterial GetParticleProcessMaterial(this GpuParticles2D particles)
    {
        return (ParticleProcessMaterial)particles.ProcessMaterial;
    }
}
