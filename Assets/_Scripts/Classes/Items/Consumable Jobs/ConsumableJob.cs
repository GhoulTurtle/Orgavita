using UnityEngine;

[System.Serializable]
public class ConsumableJob{
    [SerializeField] private ConsumableJobType jobType;
    [SerializeField] private int amount;
    [SerializeField] private float time;

    public virtual bool CheckValidConsumeJob(PlayerInventoryHandler playerInventoryHandler, out string invalidConsumeJobMessage){
        ConsumableJob consumableJob = null;
        switch (jobType){
            case ConsumableJobType.None: 
            break;
            case ConsumableJobType.LowerFearInstant: consumableJob = new LowerFearInstant();
            break;
            case ConsumableJobType.LowerFearOverTime: consumableJob = new LowerFearOverTime(time);
            break;
            case ConsumableJobType.RaiseFearInstant: consumableJob = new RaiseFearInstant(amount);
            break;
            case ConsumableJobType.RaiseFearOverTime: consumableJob = new RaiseFearOverTime(amount, time);
            break;
            case ConsumableJobType.RaiseHealthInstant: consumableJob = new RaiseHealthInstant();
            break;
            case ConsumableJobType.RaiseHealthOverTime: consumableJob = new RaiseHealthOverTime(time);
            break;
            case ConsumableJobType.RaiseMaxFear: consumableJob = new RaiseMaxFear();
            break;
            case ConsumableJobType.RaiseMaxHealth: consumableJob = new RaiseMaxHealth();
            break;
            case ConsumableJobType.RaiseMaxInventory: consumableJob = new RaiseMaxInventory();
            break;
        }

        if(consumableJob == null){
            invalidConsumeJobMessage = "ERROR: Invalid Consume Job";
            return false;
        }
        
        return consumableJob.CheckValidConsumeJob(playerInventoryHandler, out invalidConsumeJobMessage);
    }

    public virtual void PerformConsumeJob(PlayerInventoryHandler playerInventoryHandler, int _amount = 0){
        ConsumableJob consumableJob = null;
        switch (jobType){
            case ConsumableJobType.None: 
            break;
            case ConsumableJobType.LowerFearInstant: consumableJob = new LowerFearInstant();
            break;
            case ConsumableJobType.LowerFearOverTime: consumableJob = new LowerFearOverTime(time);
            break;
            case ConsumableJobType.RaiseFearInstant: consumableJob = new RaiseFearInstant(amount);
            break;
            case ConsumableJobType.RaiseFearOverTime: consumableJob = new RaiseFearOverTime(amount, time);
            break;
            case ConsumableJobType.RaiseHealthInstant: consumableJob = new RaiseHealthInstant();
            break;
            case ConsumableJobType.RaiseHealthOverTime: consumableJob = new RaiseHealthOverTime(time);
            break;
            case ConsumableJobType.RaiseMaxFear: consumableJob = new RaiseMaxFear();
            break;
            case ConsumableJobType.RaiseMaxHealth: consumableJob = new RaiseMaxHealth();
            break;
            case ConsumableJobType.RaiseMaxInventory: consumableJob = new RaiseMaxInventory();
            break;
        }

        consumableJob?.PerformConsumeJob(playerInventoryHandler, amount);
    }
}