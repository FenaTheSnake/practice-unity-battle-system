using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum GameCameraFollowing
{
    TRANSFORM,
    VECTOR3
}

public class GameCamera : MonoBehaviour
{
    [SerializeField] float distance = 7.0f;
    [SerializeField] float angle = 270.0f;
    [SerializeField] float angleOfAttack = 75.0f;
    [SerializeField] float maxDistanceBetweenCameraAndPlayer = 10.0f;

    public float anchorMaxRadius = 10.0f;

    Vector3 _curPos;

    GameCameraFollowing _followingMode;
    Transform _followingTransform;
    Vector3 _followingVec3;

    bool _hasCenteredOnPlayer = false;

    private void Awake()
    {
        //if (_followingTransform) _curPos = _followingTransform.position;
        //else _curPos = Vector3.zero;

        Application.targetFrameRate = 60;
    }

    void FixedUpdate()
    {
        Vector3 camPos = Vector3.zero;
        camPos.x = _curPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
        camPos.y = _curPos.y + Mathf.Sin(angleOfAttack * Mathf.Deg2Rad) * distance;
        camPos.z = _curPos.z + Mathf.Sin(angle * Mathf.Deg2Rad) * distance;

        transform.position = camPos;
        transform.LookAt(_curPos, Vector3.up);

        Vector3 anchor = _curPos;

        switch (_followingMode)
        {
            case GameCameraFollowing.TRANSFORM:
                _curPos = Vector3.Lerp(_curPos, _followingTransform.position, 0.05f);
                break;
            case GameCameraFollowing.VECTOR3:
                _curPos = Vector3.Lerp(_curPos, _followingVec3, 0.05f);
                //anchor = _followingVec3;
                break;
        }

        //float dist = Vector3.Distance(anchor, player.GetTransform().position);

        //float factor = dist * ((dist / anchorMaxRadius) * (dist / anchorMaxRadius));
        //_curPos = Vector3.Lerp(_curPos, Vector3.MoveTowards(anchor, player.GetTransform().position, factor), 0.05f);
    }

    public void SetFollowing(Transform transform)
    {
        //if (!transform) _followingTransform = _playerTransform;

        //_followingMode = GameCameraFollowing.TRANSFORM;
        //_followingTransform = transform;
    }
    public void SetFollowing(Vector3 vec3)
    {
        _followingMode = GameCameraFollowing.VECTOR3;
        _followingVec3 = vec3;
    }

    public void SetAnchorMaxRadius(float radius)
    {
        anchorMaxRadius = Mathf.Min(maxDistanceBetweenCameraAndPlayer, radius);
    }
}
