public enum AIStateTransitionType{
    TargetInLOSForTargetTime,
    TargetOutLOSForLooseTime,
    TargetInLOS,
    TargetNotInLOS,
    TargetNotInAttackRange,
    SearchTimeExpired,
    IdleTimeExpired,
    PatrolTimeExpired,
    ReachedMovementTarget,
    CurrentAttackFinished,
    FollowCommandTriggered,
    StopFollowCommandTriggered,
}
