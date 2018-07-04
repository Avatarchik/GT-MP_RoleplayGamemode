namespace Roleplay.Server.Enums
{
    public enum AnimationFlags
    {
        None = 0,
        Loop = 1 << 0,
        StopOnLastFrame = 1 << 1,
        OnlyAnimateUpperBody = 1 << 4,
        AllowPlayerControl = 1 << 5,
        Cancellable = 1 << 7,
        AllowRotation = 32,
        CancelableWithMovement = 128,
        RagdollOnCollision = 4194304
    }
}