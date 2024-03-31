using System;
using System.Collections;
using UnityEngine;

public class Fear : MonoBehaviour{
    [Header("Fear Variables")]
    [SerializeField] private int maxFearCells = 10;
    [SerializeField] private float safeSpaceBuildMaxPercent = 0.5f; 
    [SerializeField] private float fearBuildTimeInSeconds = 90f;
    [SerializeField] private float fearGracePeriodInSeconds = 30f;

    private const int fearCellsPerTick = 1;
    private WaitForSeconds fearTickWaitTimer;

    public event EventHandler<FearCellsChangedEventArgs> OnCurrentFearCellsChanged;
    public class FearCellsChangedEventArgs : EventArgs{
        public int currentFearCells;
        public FearCellsChangedEventArgs(int _currentFearCells){
            currentFearCells = _currentFearCells;
        }
    }
    public event EventHandler<FearCellsIncreasedEventArgs> OnMaxFearCellsIncreased;
    public class FearCellsIncreasedEventArgs : EventArgs{
        public int amountIncreased;
        public FearCellsIncreasedEventArgs(int _amountIncreased){
            amountIncreased =_amountIncreased;
        }
    }

    private int currentFearCells;

    private int maxFearCellsInSafeSpace;

    private IEnumerator currentFearBuildCoroutine;

    private void Awake() {
        currentFearCells = 0;
        fearTickWaitTimer = new WaitForSeconds(fearCellsPerTick);
        CalculateMaxFearCellsInSafeSpace();
        StartFearBuild();
    }

    private void OnDestroy() {
        StopAllCoroutines();    
    }

    public void IncreaseMaxFearCells(int amountIncreased){
        maxFearCells += amountIncreased;
        
        CalculateMaxFearCellsInSafeSpace();

        OnMaxFearCellsIncreased?.Invoke(this, new FearCellsIncreasedEventArgs(amountIncreased));
        StartCoroutine(FearCellGraceCoroutine());
    }

    public void LowerCurrentFearCells(int lowerCellAmount){
        if(currentFearCells == 0) return;

        currentFearCells -= lowerCellAmount;
        if(currentFearCells < 0) currentFearCells = 0;

        OnCurrentFearCellsChanged?.Invoke(this, new FearCellsChangedEventArgs(currentFearCells));
        StartCoroutine(FearCellGraceCoroutine());
    }

    public void RaiseCurrentFearCells(int raiseCellAmount){
        currentFearCells += raiseCellAmount;
        if(currentFearCells > maxFearCells){
            TriggerGameOver();
            return;
        } 

        OnCurrentFearCellsChanged?.Invoke(this, new FearCellsChangedEventArgs(currentFearCells));
    }

    public void RaiseCurrentFearCellsOverTime(int totalAmount, float timePerCell){
        StartCoroutine(RaiseFearOverTimeJob(totalAmount, timePerCell));
	}

    public void LowerCurrentFearCellsOverTime(int totalAmount, float timePerCell){
        StartCoroutine(LowerFearOverTimeJob(totalAmount, timePerCell));
    }

    public bool WillAmountTriggerGameOver(int amount){
        return currentFearCells + amount > maxFearCells;
    }

    public void OnEnterSafeSpace(){
        StopFearBuild();
        StartCalmFearBuild();
    }

    public void OnExitSafeSpace(){
        StopFearBuild();
        StartFearBuild();
    }

    public bool IsCurrentFearEmpty(){
        return currentFearCells == 0;
    }

    public bool IsCurrentFearFull(){
        return currentFearCells == maxFearCells;
    }

    private void CalculateMaxFearCellsInSafeSpace(){
        maxFearCellsInSafeSpace = Mathf.RoundToInt(maxFearCells * safeSpaceBuildMaxPercent);
    }

    private void StartCalmFearBuild(){
        currentFearBuildCoroutine = FearCellCalmBuildCoroutine();
        StartCoroutine(currentFearBuildCoroutine);
    }

    private void StartFearBuild(){
        currentFearBuildCoroutine = FearCellBuildCoroutine();
        StartCoroutine(currentFearBuildCoroutine);
    }

    private void StopFearBuild(){
        if(currentFearBuildCoroutine != null){
            StopCoroutine(currentFearBuildCoroutine);
            currentFearBuildCoroutine = null;
        }
    }

    private void TriggerGameOver(){
        Debug.Log("Game Over!");
    }

    private IEnumerator RaiseFearOverTimeJob(int totalAmount, float waitTimePerCell){
        while(totalAmount > 0){
			RaiseCurrentFearCells(1);
			yield return new WaitForSeconds(waitTimePerCell);
			totalAmount -= 1;
		}
    }

    private IEnumerator LowerFearOverTimeJob(int totalAmount, float waitTimePerCell){
        while(totalAmount > 0){
			LowerCurrentFearCells(1);
			yield return new WaitForSeconds(waitTimePerCell);
			totalAmount -= 1;
		}
    }

    private IEnumerator FearCellGraceCoroutine(){
        StopFearBuild();

        yield return new WaitForSeconds(fearGracePeriodInSeconds);

        StartFearBuild();
    }

    private IEnumerator FearCellCalmBuildCoroutine(){
        yield return new WaitForSeconds(fearBuildTimeInSeconds);
        if(currentFearCells > maxFearCellsInSafeSpace){
            currentFearCells--;
        }
        else if(currentFearCells < maxFearCellsInSafeSpace){
            currentFearCells++;
        }

        OnCurrentFearCellsChanged?.Invoke(this, new FearCellsChangedEventArgs(currentFearCells));

        if(currentFearCells == maxFearCellsInSafeSpace){
            StopFearBuild();
            yield break;
        }

        StartCalmFearBuild();
    }

    private IEnumerator FearCellBuildCoroutine(){
        yield return new WaitForSeconds(fearBuildTimeInSeconds);
        currentFearCells++;
        if(currentFearCells > maxFearCells){
            TriggerGameOver();
            yield break;
        }
        
        OnCurrentFearCellsChanged?.Invoke(this, new FearCellsChangedEventArgs(currentFearCells));
        StartFearBuild();
    }
}