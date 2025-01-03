namespace Waldhari.Common.Enums
{
    // See https://docs.fivem.net/natives/?_0x15D3A79D4E44B913 for documentation
    public enum ENavigation
    {
        // Default flag
        Default = 0,

        // Will ensure the ped continues to move whilst waiting for the path
        // to be found, and will not slow down at the end of their route.
        NoStopping = 1,

        // Performs a slide-to-coord at the end of the task. This requires that the
        // accompanying NAVDATA structure has the 'SlideToCoordHeading' member set correctly.
        AdvSlideToCoordAndAchieveHeadingAtEnd = 2,

        // If the navmesh is not loaded in under the target position, then this will
        // cause the ped to get as close as is possible on whatever navmesh is loaded.
        // The navmesh must still be loaded at the path start.
        GoFarAsPossibleIfTargetNavmeshNotLoaded = 4,

        // Will allow navigation underwater - by default this is not allowed.
        AllowSwimmingUnderwater = 8,

        // Will only allow navigation on pavements. If the path starts or ends off
        // the pavement, the command will fail. Likewise if no pavement-only route
        // can be found even although the start and end are on pavement.
        KeepToPavements = 16,

        // Prevents the path from entering water at all.
        NeverEnterWater = 32,

        // Disables object-avoidance for this path. The ped may still make minor
        // steering adjustments to avoid objects, but will not pathfind around them.
        DontAvoidObjects = 64,

        // Specifies that the navmesh route will only be able to traverse up slopes
        // which are under the angle specified, in the MaxSlopeNavigable member of the accompanying NAVDATA structure.
        AdvancedUseMaxSlopeNavigable = 128,

        // Unused.
        StopExactly = 512,

        // The entity will look ahead in its path for a longer distance to make the
        // walk/run start go more in the right direction.
        AccurateWalkrunStart = 1024,

        // Disables ped-avoidance for this path while we move.
        DontAvoidPeds = 2048,

        // If target pos is inside the boundingbox of an object it will otherwise be pushed out.
        DontAdjustTargetPosition = 4096,

        // Turns off the default behaviour, which is to stop exactly at the target position.
        // Occasionally this can cause footsliding/skating problems.
        SuppressExactStop = 8192,

        // Prevents the path-search from finding paths outside of this search distance.
        // This can be used to prevent peds from finding long undesired routes.
        AdvancedUseClampMaxSearchDistance = 16384,

        // Pulls out the paths from edges at corners for a longer distance, to prevent peds walking into stuff.
        PullFromEdgeExtra = 32768
    }
}