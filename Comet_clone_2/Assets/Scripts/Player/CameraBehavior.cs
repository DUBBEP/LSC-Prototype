using Photon.Realtime;
using System;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    private Camera _camera;

    public float smoothFactor = 4;

    private bool isFollowing;
    private bool returnToRestPosition;
    private Transform target;
    public Transform Target { get { return target; } }
    private Transform myPlayer;


    [SerializeField]
    private int quadrantSize;



    public Vector3 topDownOffSet;
    private Vector3 targetPosition;
    private float verticalOffSet;
    private float defaultFOV;
    private float targetFOV;
    

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        verticalOffSet = topDownOffSet.y;
        defaultFOV = _camera.fieldOfView;
        StopFollowing();
    }

    public void Initialize(PlayerBehavior localPlayer)
    {
        myPlayer = localPlayer.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFollowing)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothFactor);

            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * smoothFactor);

        }

        if (returnToRestPosition)
        {

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothFactor);

            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * smoothFactor);

        }
    }

    

    public void StartFollowing(Transform targetTransform)
    {
        Debug.Log("Start Following");
        isFollowing = true;
        returnToRestPosition = false;
        target = targetTransform;
        targetPosition = new Vector3(target.position.x, verticalOffSet, target.position.z - 1);
        targetFOV = 45;

    }

    public void StopFollowing()
    {
        isFollowing = false;
        returnToRestPosition = true;
        target = null;

        targetPosition = topDownOffSet + GetPlayerQuadrant(myPlayer);
        targetFOV = defaultFOV;
    }

    public Vector3 GetPlayerQuadrant(Transform target)
    {
        Vector3 quadrant = new Vector3();
        quadrant.z = 0;
        quadrant.x = (int)(target.position.x / quadrantSize);
        quadrant.z = (int)(target.position.z / quadrantSize);

        return quadrant * quadrantSize;
    }
}
