public class RaiseHealthInstant : ConsumableJob{
    private Health playerHealth;

    public override bool CheckValidConsumeJob(PlayerInventoryHandler playerInventoryHandler, out string invalidConsumeJobMessage){
        if(!playerInventoryHandler.TryGetComponent(out playerHealth)){
            invalidConsumeJobMessage = "ERROR: No Health Found on Player!";
            return false;
        }

        if(playerHealth.IsHealthFull()){
            invalidConsumeJobMessage = "I feel fine.";
            return false;
        }

        invalidConsumeJobMessage = "";
        return true;
    }

    public override void PerformConsumeJob(PlayerInventoryHandler playerInventoryHandler, int _amount){
        if(!playerInventoryHandler.TryGetComponent(out playerHealth)) return;
        playerHealth.HealHealth(_amount);
    }
}
