using System;
using System.Collections;
using UnityEngine;

public class Fear : MonoBehaviour{
    [Header("Fear Variables")]
    [SerializeField] private int maxFearCells = 10;
    [SerializeField] private float safeSpaceBuildMaxPercent = 0.5f; 
    [SerializeField] private float fearBuildTimeInSeconds = 90f;
    [SerializeField] private float fearGracePeriodInSeconds = 30f;

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

    public void OnEnterSafeSpace(){
        StopFearBuild();
        StartCalmFearBuild();
    }

    public void OnExitSafeSpace(){
        StopFearBuild();
        StartFearBuild();
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