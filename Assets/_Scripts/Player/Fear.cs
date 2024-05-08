using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fear : MonoBehaviour{
    [Header("Fear Variables")]
    [SerializeField] private int maxFearCells = 10;
    [SerializeField] private float safeSpaceBuildMaxPercent = 0.5f; 
    [SerializeField] private float fearBuildTimeInSeconds = 90f;
    [SerializeField] private float fearGracePeriodInSeconds = 30f;
    [SerializeField, Range(0f, 1f)] private float calmPercentCutoff = 0.25f;
	[SerializeField, Range(0f, 1f)] private float panicPercentCutoff = 0.5f;
	[SerializeField, Range(0f, 1f)] private float terrifiedPercentCutoff = 0.75f;

    public EventHandler<FearStateChangedEventArgs> OnFearStateChanged;
	public class FearStateChangedEventArgs : EventArgs{
		public FearState fearState;

		public FearStateChangedEventArgs(FearState _fearState){
			fearState = _fearState;
		}
	}

    private FearState currentFearState;

    private float calmStatusCutoff;
	private float panicStatusCutoff;
	private float terrifiedStatusCutoff;

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

        SetStatusCutoff();
        OnFearStateChanged?.Invoke(this, new FearStateChangedEventArgs(currentFearState));
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

        UpdateFearState();
        OnCurrentFearCellsChanged?.Invoke(this, new FearCellsChangedEventArgs(currentFearCells));
        StartCoroutine(FearCellGraceCoroutine());
    }

    public void RaiseCurrentFearCells(int raiseCellAmount){
        currentFearCells += raiseCellAmount;
        if(currentFearCells > maxFearCells){
            TriggerGameOver();
            return;
        } 

        UpdateFearState();
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

    private void SetStatusCutoff(){
        calmStatusCutoff = Mathf.RoundToInt(maxFearCells * calmPercentCutoff);
        panicStatusCutoff = Mathf.RoundToInt(maxFearCells * panicPercentCutoff);
        terrifiedStatusCutoff = Mathf.RoundToInt(maxFearCells * terrifiedPercentCutoff);
    }

    private void UpdateFearState(){
		FearState incomingFearState = FearState.Calm;
		switch(currentFearCells){
			case int fear when fear <= calmStatusCutoff:
				incomingFearState = FearState.Calm;
			break;
			case int fear when fear <= panicStatusCutoff: 
				incomingFearState = FearState.Panic;
			break;
			case int fear when fear <= terrifiedStatusCutoff:
				incomingFearState = FearState.Terrified;
			break;
		}

		if(currentFearState == incomingFearState) return;

		currentFearState = incomingFearState;

		OnFearStateChanged?.Invoke(this, new FearStateChangedEventArgs(incomingFearState));
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
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
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
        
        UpdateFearState();
        OnCurrentFearCellsChanged?.Invoke(this, new FearCellsChangedEventArgs(currentFearCells));
        StartFearBuild();
    }
}