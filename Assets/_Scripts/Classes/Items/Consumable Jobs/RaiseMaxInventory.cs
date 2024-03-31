public class RaiseMaxInventory : ConsumableJob{
    public override bool CheckValidConsumeJob(PlayerInventoryHandler playerInventoryHandler, out string invalidConsumeJobMessage){
        invalidConsumeJobMessage = null;
        return true;
    }

    public override void PerformConsumeJob(PlayerInventoryHandler playerInventoryHandler, int _amount){
        playerInventoryHandler.GetInventory().IncreaseMaxInventory(_amount);
    }
}