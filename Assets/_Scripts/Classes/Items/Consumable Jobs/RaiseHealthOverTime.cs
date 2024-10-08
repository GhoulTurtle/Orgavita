public class RaiseHealthOverTime : ConsumableJob{
    private Health playerHealth;

    private float healTime;

    public RaiseHealthOverTime(float _time){
        healTime = _time;
    }

    public override bool CheckValidConsumeJob(PlayerInventoryHandler playerInventoryHandler, out string invalidConsumeJobMessage){
        if(!playerInventoryHandler.TryGetComponent(out playerHealth)){
            invalidConsumeJobMessage = "ERROR: No Health Found on Player!";
            return false;
        }

        if(playerHealth.IsHealthFull()){
            invalidConsumeJobMessage = "I feel fine.";
            return false;
        }

        if(playerHealth.IsActiveHealOverTimeJob()){
            invalidConsumeJobMessage = "Already healing.";
            return false;
        }

        invalidConsumeJobMessage = "";
        return true;
    }

    public override void PerformConsumeJob(PlayerInventoryHandler playerInventoryHandler, int _amount){
        if(!playerInventoryHandler.TryGetComponent(out playerHealth)) return;
        playerHealth.HealHealthOverTime(_amount, healTime);
    }
}