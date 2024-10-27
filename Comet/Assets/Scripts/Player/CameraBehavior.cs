using Photon.Realtime;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    private Camera _camera;

    public float smoothFactor = 4;

    private bool isFollowing;
    private bool returnToRest;
    private Transform target;
    public Transform Target { get { return target; } }
    public Transform myPlayer;
    public Transform rotationPivot;


    [SerializeField]
    private float panSpeed;
    [SerializeField]
    private int quadrantSize;
    [SerializeField]
    private Vector3 camOffSet;
    private Vector3 targetPosition;
    private float defaultFOV;
    private float targetFOV;

    private float xPanInput;
    private float yPanInput;


    public enum cameraMode
    {
        followCam,
        freeCam,
        angledCam
    }

    cameraMode camMode;


    // Start is called before the first frame update
    void Start()
    {
        camMode = cameraMode.followCam;
        _camera = GetComponent<Camera>();
        defaultFOV = _camera.fieldOfView;
    }

    void Update()
    {
        if (camMode != cameraMode.followCam)
        {
            xPanInput = Input.GetAxis("Horizontal");
            yPanInput = Input.GetAxis("Vertical");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (camMode == cameraMode.followCam)
            UpdateFollowCam();
        else if (camMode == cameraMode.freeCam)
            UpdateFreeCam();
        else if (camMode == cameraMode.angledCam)
        {
            // to be implemented
        }
    }

    public void SetCamMode(cameraMode mode)
    {
        camMode = mode;

        switch (mode)
        {
            case cameraMode.followCam:
                transform.parent = null;
                break;
            case cameraMode.freeCam:
                transform.parent = null;
                break;
            case cameraMode.angledCam:
                // to be implemented
                break;
        }    
    }

    void UpdateAngledCam()
    {
        // to be implemented
    }

    void UpdateFreeCam()
    {
        transform.position = new Vector3(transform.position.x + xPanInput * panSpeed, transform.position.y, transform.position.z + yPanInput * panSpeed);
    }


    #region FollowCamBehavior

    private void UpdateFollowCam()
    {
        if ((transform.position - targetPosition).magnitude > 0.05f)
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothFactor);


        if (isFollowing)
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * smoothFactor);
        else
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * smoothFactor);
    }

    public void StartFollowing(Transform targetTransform)
    {
        isFollowing = true;
        target = targetTransform;
        targetPosition = new Vector3(target.position.x, camOffSet.y, target.position.z - 1);
        targetFOV = 45;

    }

    public void RestAtPlayerQuadrant()
    {
        isFollowing = false;
        target = null;

        Vector3 playerQuadrant = GetPlayerQuadrant(myPlayer);
        Debug.Log("player Quadrant" + playerQuadrant);


        targetPosition = new Vector3(playerQuadrant.x, camOffSet.y, playerQuadrant.z); ;

        Debug.Log("Target Position" + targetPosition);
        targetFOV = defaultFOV;
    }

    public Vector3 GetPlayerQuadrant(Transform target)
    {


        Vector3 quadrant = new Vector3();
        quadrant.z = 0;

        if (target.position.x / quadrantSize <= 1)
            quadrant.x = 8;
        else
            quadrant.x = (Mathf.Min((int)(target.position.x / quadrantSize), (GridManager.instance.GridSize.x / quadrantSize) - 1) * quadrantSize) + (quadrantSize / 2);


        if (target.position.z / quadrantSize <= 1)
            quadrant.z = 4;
        else
            quadrant.z = (Mathf.Min((int)(target.position.z / quadrantSize), (GridManager.instance.GridSize.y / quadrantSize) - 0.75f) * quadrantSize) + (quadrantSize / 3);


        return quadrant;
    }

    #endregion
}
