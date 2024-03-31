public class LowerFearOverTime : ConsumableJob{
    private Fear playerFear;

    private float fearLowerTime;

    public LowerFearOverTime(float _time){
        fearLowerTime = _time;
    }

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
        playerFear.LowerCurrentFearCellsOverTime(_amount, fearLowerTime);
    }
}
