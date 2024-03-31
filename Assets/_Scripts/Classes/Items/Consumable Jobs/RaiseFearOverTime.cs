public class RaiseFearOverTime : ConsumableJob{
    private Fear playerFear;

    private int fearIncreasedAmount;
    private float fearIncreaseTime;

    public RaiseFearOverTime(int _amount, float _time){
        fearIncreasedAmount = _amount;
        fearIncreaseTime = _time;
    }

    public override bool CheckValidConsumeJob(PlayerInventoryHandler playerInventoryHandler, out string invalidConsumeJobMessage){
        if(!playerInventoryHandler.gameObject.TryGetComponent(out playerFear)){
            invalidConsumeJobMessage = "ERROR: No Fear Found on Player!";
            return false;
        }

        if(playerFear.IsCurrentFearFull() || playerFear.WillAmountTriggerGameOver(fearIncreasedAmount)){
            invalidConsumeJobMessage = "I can't use that, my fear levels are too high, I'll die.";
            return false;
        }

        invalidConsumeJobMessage = "";
        return true;    
    }

    public override void PerformConsumeJob(PlayerInventoryHandler playerInventoryHandler, int _amount){
        if(!playerInventoryHandler.TryGetComponent(out playerFear)) return;
        playerFear.RaiseCurrentFearCellsOverTime(_amount, fearIncreaseTime);
    }
}
