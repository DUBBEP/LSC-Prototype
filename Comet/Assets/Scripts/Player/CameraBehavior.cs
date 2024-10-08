using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    private Camera _camera;

    public float smoothFactor = 4;

    private bool isFollowing;
    private bool returnToDefaultPos;
    private Transform target;
    public Transform Target { get { return target; } }

    private Vector3 targetPosition;
    private float verticalOffSet;
    private float defaultFOV;
    private float targetFOV;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        verticalOffSet = GameManager.instance.camDefaultPos.y;
        defaultFOV = _camera.fieldOfView;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFollowing)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothFactor);

            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * smoothFactor);

        }

        if (returnToDefaultPos)
        {
            transform.position = Vector3.Lerp(transform.position, GameManager.instance.camDefaultPos, Time.deltaTime * smoothFactor);

            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * smoothFactor);

        }
    }


    public void StartFollowing(Transform targetTransform)
    {
        Debug.Log("Start Following");
        isFollowing = true;
        returnToDefaultPos = false;
        target = targetTransform;
        targetPosition = new Vector3(target.position.x, verticalOffSet, target.position.z - 1);
        targetFOV = 45;

    }

    public void StopFollowing()
    {
        isFollowing = false;
        returnToDefaultPos = true;
        target = null;
        targetPosition = Vector3.zero;
        targetFOV = defaultFOV;
    }
}
