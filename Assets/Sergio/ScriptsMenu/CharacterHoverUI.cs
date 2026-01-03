using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class CharacterHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Referencias Visuales")]
    public GameObject glowObject;
    
    [Header("Referencias de Audio")]
    public AudioSource audioSource;
    public AudioClip soundEffect;

    private CanvasGroup glowCanvasGroup;
    private RectTransform glowRectTransform;
    private Coroutine currentAnimation;

    [Header("Configuración del Efecto")]
    public float targetScale = 4f; 
    public float scaleSpeed = 15f;
    public int flickerCount = 3;
    public float flickerSpeed = 0.05f;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        if (glowObject != null)
        {
            glowCanvasGroup = glowObject.GetComponent<CanvasGroup>();
            glowRectTransform = glowObject.GetComponent<RectTransform>();

            if (glowCanvasGroup == null) glowCanvasGroup = glowObject.AddComponent<CanvasGroup>();
            glowCanvasGroup.blocksRaycasts = false; 

            ResetGlow();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource != null && soundEffect != null)
        {
            audioSource.clip = soundEffect;
            audioSource.Play();
        }

        if (glowObject == null) return;
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(EncendidoEpico());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (glowObject == null) return;
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(ApagadoSuave());
    }

    IEnumerator EncendidoEpico()
    {
        glowObject.SetActive(true);
        glowCanvasGroup.alpha = 1;
        
        float currentScale = 0f;
        while (currentScale < targetScale) 
        {
            currentScale += Time.deltaTime * scaleSpeed;
            glowRectTransform.localScale = Vector3.one * currentScale;
            yield return null;
        }
        glowRectTransform.localScale = Vector3.one * targetScale;

        for (int i = 0; i < flickerCount; i++)
        {
            glowCanvasGroup.alpha = 0.2f;
            yield return new WaitForSeconds(flickerSpeed);
            glowCanvasGroup.alpha = 1f;
            yield return new WaitForSeconds(flickerSpeed);
        }

        float time = 0;
        float baseScale = targetScale;
        while (true)
        {
            time += Time.deltaTime * 2f;
            float pulse = baseScale + (Mathf.Sin(time) * (baseScale * 0.05f)); 
            glowRectTransform.localScale = Vector3.one * pulse;
            yield return null;
        }
    }

    IEnumerator ApagadoSuave()
    {
        while (glowCanvasGroup.alpha > 0)
        {
            glowCanvasGroup.alpha -= Time.deltaTime * 5f;
            glowRectTransform.localScale = Vector3.Lerp(glowRectTransform.localScale, Vector3.zero, Time.deltaTime * 10f);
            yield return null;
        }
        ResetGlow();
    }

    void ResetGlow()
    {
        glowObject.SetActive(false);
        if (glowCanvasGroup) glowCanvasGroup.alpha = 0;
        if (glowRectTransform) glowRectTransform.localScale = Vector3.zero;
    }
}