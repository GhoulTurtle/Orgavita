public enum AIStateTransitionType{
    TargetInLOSForTargetTime,
    TargetOutLOSForLooseTime,
    TargetInLOS,
    TargetInAttackRange,
    SearchTimeExpired,
    IdleTimeExpired,
    PatrolTimeExpired,
    ReachedMovementTarget,
    FollowCommandTriggered,
    StopFollowCommandTriggered,
}
