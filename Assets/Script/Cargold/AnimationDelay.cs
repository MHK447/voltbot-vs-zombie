using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDelay : MonoBehaviour
{
    [SerializeField] private Animation anim = null;
    [SerializeField] private float delayMin = 0f, delayMax = 1f;
    private Coroutine cor;

    private void Awake()
    {
        this.anim.Stop();
    }

    private void Start()
    {
        this.cor = StartCoroutine(this.Wait_Cor());
    }

    private IEnumerator Wait_Cor()
    {
        float _delay = Random.Range(this.delayMin, this.delayMax);
        yield return new WaitForSeconds(_delay);

        this.anim.Play();
    }

    private void OnDestroy()
    {
        if (this.cor != null)
            StopCoroutine(this.cor);

        this.cor = null;
    }
    private void OnDisable()
    {
        if (this.cor != null)
            StopCoroutine(this.cor);

        this.cor = null;
    }

    public void CallAni_AniDone_Func()
    {
        this.cor = StartCoroutine(this.Wait_Cor());
    }
}
