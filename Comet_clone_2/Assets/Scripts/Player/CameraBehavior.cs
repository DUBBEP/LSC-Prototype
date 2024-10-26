<<<<<<< HEAD
using Photon.Realtime;
using System;
=======
>>>>>>> parent of 3c48487 (Removed Clones)
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    private Camera _camera;

    public float smoothFactor = 4;

    private bool isFollowing;
<<<<<<< HEAD
    private bool returnToRestPosition;
    private Transform target;
    public Transform Target { get { return target; } }
    private Transform myPlayer;


    [SerializeField]
    private int quadrantSize;



    public Vector3 topDownOffSet;
=======
    private bool returnToDefaultPos;
    private Transform target;
    public Transform Target { get { return target; } }

>>>>>>> parent of 3c48487 (Removed Clones)
    private Vector3 targetPosition;
    private float verticalOffSet;
    private float defaultFOV;
    private float targetFOV;
<<<<<<< HEAD
    
=======
>>>>>>> parent of 3c48487 (Removed Clones)

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
<<<<<<< HEAD
        verticalOffSet = topDownOffSet.y;
        defaultFOV = _camera.fieldOfView;
    }

    public void Initialize(PlayerBehavior localPlayer)
    {
        myPlayer = localPlayer.transform;
    }

=======
        verticalOffSet = GameManager.instance.camDefaultPos.y;
        defaultFOV = _camera.fieldOfView;
    }

>>>>>>> parent of 3c48487 (Removed Clones)
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFollowing)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothFactor);

            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * smoothFactor);

        }

<<<<<<< HEAD
        if (returnToRestPosition)
        {

            transform.position = Vector3.Lerp(transform.position, topDownOffSet + GetPlayerQuadrant(myPlayer), Time.deltaTime * smoothFactor);
=======
        if (returnToDefaultPos)
        {
            transform.position = Vector3.Lerp(transform.position, GameManager.instance.camDefaultPos, Time.deltaTime * smoothFactor);
>>>>>>> parent of 3c48487 (Removed Clones)

            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * smoothFactor);

        }
    }

<<<<<<< HEAD
    
=======
>>>>>>> parent of 3c48487 (Removed Clones)

    public void StartFollowing(Transform targetTransform)
    {
        Debug.Log("Start Following");
        isFollowing = true;
<<<<<<< HEAD
        returnToRestPosition = false;
=======
        returnToDefaultPos = false;
>>>>>>> parent of 3c48487 (Removed Clones)
        target = targetTransform;
        targetPosition = new Vector3(target.position.x, verticalOffSet, target.position.z - 1);
        targetFOV = 45;

    }

    public void StopFollowing()
    {
        isFollowing = false;
<<<<<<< HEAD
        returnToRestPosition = true;
=======
        returnToDefaultPos = true;
>>>>>>> parent of 3c48487 (Removed Clones)
        target = null;
        targetPosition = Vector3.zero;
        targetFOV = defaultFOV;
    }
<<<<<<< HEAD

    public Vector3 GetPlayerQuadrant(Transform target)
    {
        Vector3 quadrant = new Vector3();

        quadrant.x = Mathf.RoundToInt(target.position.x / quadrantSize);
        quadrant.z = 0;
        quadrant.z = Mathf.RoundToInt(target.position.z / quadrantSize);

        return quadrant * quadrantSize;
    }
=======
>>>>>>> parent of 3c48487 (Removed Clones)
}
