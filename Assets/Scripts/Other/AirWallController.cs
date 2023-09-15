using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirWallController : MonoBehaviour
{
    private BoxCollider2D col;
    [SerializeField] private string missionName;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        foreach (Mission mission in GameManager.Instance.missionList)
        {
            if (mission.missionName == missionName && mission.missionStatus == Mission.MissionStatus.Rewarded)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
