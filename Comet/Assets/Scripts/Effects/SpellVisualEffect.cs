using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


public class SpellVisualEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject effectPrefab;
    [SerializeField]
    private GameObject anchorPrefab;

    private Transform effectAnchor;
    private Animator animationController;
    private GameObject effectInstance;
    private bool isPlaying;

    void Update()
    {
        if (isPlaying)
            if (!AnimatorIsPlaying())
                DestroyInstance();
    }

    public void Play(Vector3 cords)
    {
        SpawnEffect(cords, Vector3.zero);
    }

    public void Play(Vector3 cords, Vector2Int direction)
    {
        Vector3 fireDirection = new Vector3();
        if (direction == Vector2Int.right)
            fireDirection = new Vector3(0, 90, 0);
        else if (direction == Vector2Int.left)
            fireDirection = new Vector3(0, 270, 0);
        else if (direction == Vector2Int.down)
            fireDirection = new Vector3(0, 180, 0);
        else if (direction == Vector2Int.up)
            fireDirection = new Vector3(0, 0, 0);

        SpawnEffect(cords, fireDirection);
    }

    void SpawnEffect(Vector3 cords, Vector3 direction)
    {
        effectAnchor = Instantiate(anchorPrefab, Vector3.zero, Quaternion.identity).transform;
        effectInstance = Instantiate(effectPrefab, Vector3.zero, Quaternion.identity);
        effectInstance.SetActive(true);
        effectInstance.transform.parent = effectAnchor.transform;
        
        effectAnchor.position = cords;
        effectAnchor.rotation = Quaternion.Euler(direction);
        
        animationController = effectInstance.GetComponent<Animator>();

        if (animationController == null)
            animationController = effectInstance.GetComponentInChildren<Animator>();

        isPlaying = true;
    }

    void DestroyInstance()
    {
        animationController = null;
        Destroy(effectInstance);
        Destroy(effectAnchor);
        isPlaying = false;
    }
    public bool AnimatorIsPlaying()
    {
        return animationController.GetCurrentAnimatorStateInfo(0).length >
               animationController.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

}
