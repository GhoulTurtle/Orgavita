using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// A class that containes and coroutine and the runner of a coroutine. Can be used to keep track of corutines externally within another class.
/// Handles starting and disposing of the internal coroutine.
/// </summary>
public class CoroutineContainer{
    private MonoBehaviour coroutineRunner;
    private IEnumerator coroutineReference;
    private Coroutine coroutine;

    public EventHandler<CoroutineDisposedEventArgs> OnCoroutineDisposed;
    public class CoroutineDisposedEventArgs{
        public CoroutineContainer coroutineContainer;

        public CoroutineDisposedEventArgs(CoroutineContainer _coroutineContainer){
            coroutineContainer = _coroutineContainer;
        }
    }

    public CoroutineContainer(MonoBehaviour _coroutineRunner = null, IEnumerator _coroutine = null){
        coroutineRunner = _coroutineRunner;
        coroutineReference = _coroutine;
    }

    public void SetCoroutine(IEnumerator _coroutine){
        coroutineReference = _coroutine;
    }

    public void StartCoroutine(){
        if(coroutineReference == null || coroutineRunner == null) return;

        coroutine = coroutineRunner.StartCoroutine(coroutineReference);
    }

    public void TryStopCoroutine(){
        if(coroutine != null){
            coroutineRunner.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public void Dispose(){
        TryStopCoroutine();

        OnCoroutineDisposed?.Invoke(this, new CoroutineDisposedEventArgs(this));
    }

    public MonoBehaviour GetCoroutineRunner(){
        return coroutineRunner;
    }

    public IEnumerator GetCoroutineReference(){
        return coroutineReference;
    }

    public bool IsCoroutineRunning(){
        return coroutine != null;
    }
}