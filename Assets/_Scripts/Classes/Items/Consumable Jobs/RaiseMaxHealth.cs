public class RaiseMaxHealth : ConsumableJob{
    private Health playerHealth;

    public override bool CheckValidConsumeJob(PlayerInventoryHandler playerInventoryHandler, out string invalidConsumeJobMessage){
        if(!playerInventoryHandler.TryGetComponent(out playerHealth)){
            invalidConsumeJobMessage = "ERROR: No Health Found on Player!";
            return false;
        }

        invalidConsumeJobMessage = "";
        return true;
    }

    public override void PerformConsumeJob(PlayerInventoryHandler playerInventoryHandler, int _amount){
        if(!playerInventoryHandler.TryGetComponent(out playerHealth)) return;
        playerHealth.IncreaseMaxHealth(_amount);
    }
}