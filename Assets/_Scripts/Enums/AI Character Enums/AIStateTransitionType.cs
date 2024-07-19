public enum AIStateTransitionType{
    TargetInLOSForTargetTime,
    TargetOutLOSForLooseTime,
    TargetOutLOSForSearchTime,
    TargetInLOS,
    TargetInAttackRange,
    IdleTimeExpired,
    PatrolTimeExpired,
    ReachedMovementTarget,
    FollowCommandTriggered,
    StopFollowCommandTriggered,
}
