public class RaiseMaxFear : ConsumableJob{
    private Fear playerFear;

    public override bool CheckValidConsumeJob(PlayerInventoryHandler playerInventoryHandler, out string invalidConsumeJobMessage){
        if(!playerInventoryHandler.gameObject.TryGetComponent(out playerFear)){
            invalidConsumeJobMessage = "ERROR: No Fear Found on Player!";
            return false;
        }

        invalidConsumeJobMessage = "";
        return true;
    }

    public override void PerformConsumeJob(PlayerInventoryHandler playerInventoryHandler, int _amount){
        if(!playerInventoryHandler.TryGetComponent(out playerFear)) return;
        playerFear.IncreaseMaxFearCells(_amount);
    }
}
