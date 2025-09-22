using Godot;

namespace GodotUtils;

public static class RigidBody2dExtensions
{
    /// <summary>
    /// Smoothly rotates a RigidBody2D towards a target position.
    /// </summary>
    /// <param name="rigidBody">The rigid body to rotate.</param>
    /// <param name="targetPosition">The world position to face.</param>
    /// <param name="rotationSpeed">How fast to rotate towards the target in radians.</param>
    /// <param name="maxAngularSpeed">Maximum rotational speed to rotate towards the target in radians.</param>
    /// <param name="smoothness">Smoothness factor.</param>
    public static void RotateTowards(this RigidBody2D rigidBody, Vector2 targetPosition, float rotationSpeed, float maxAngularSpeed, float smoothness = 0.1f)
    {
        // Compute direction and target angle
        Vector2 direction = targetPosition - rigidBody.GlobalPosition;
        float targetAngle = direction.Angle();

        // Shortest angle difference (-pi to pi)
        float angleDiff = Mathf.Wrap(targetAngle - rigidBody.Rotation, -Mathf.Pi, Mathf.Pi);

        // Desired angular velocity
        float desiredAngularVelocity = angleDiff * rotationSpeed;

        // Clamp angular velocity
        desiredAngularVelocity = Mathf.Clamp(desiredAngularVelocity, -maxAngularSpeed, maxAngularSpeed);

        // Smoothly apply angular velocity
        rigidBody.AngularVelocity = Mathf.Lerp(rigidBody.AngularVelocity, desiredAngularVelocity, smoothness);
    }
}
