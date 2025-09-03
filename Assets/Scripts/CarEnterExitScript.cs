using System.Collections;
using UnityEngine;

public class CarEnterExitScript : MonoBehaviour
{
    public Transform CarEnterPos;
    public Transform CarSItPos;
    public Transform CarExitPos;
    public float moveSpeed = 4f;
    public GameObject car;

    public GameObject checkPoint;
    public void CarEnter()
    {
        CarEnterPos = GamePlayHandler.instance.Tps_player.GetComponent<PlayerManager>().CarEnterPos;
        CarExitPos = GamePlayHandler.instance.Tps_player.GetComponent<PlayerManager>().CarExitPos;
        CarSItPos = GamePlayHandler.instance.Tps_player.GetComponent<PlayerManager>().CarSitPos;
        car = PlayerManager.instance.NowPlayerInThisCar;
        checkPoint = PlayerManager.instance.checkpoint;
        PlayerManager.instance.CarPlayer.transform.position = CarEnterPos.position;
        PlayerManager.instance.CarPlayer.transform.rotation = CarEnterPos.rotation;

       
        PlayerManager.instance.CarPlayer.transform.SetParent(car.transform);

        PlayerManager.instance.CarPlayer.SetActive(true);
        GamePlayHandler.instance.Tps_player.SetActive(false);

        gameObject.GetComponent<Animator>().Play("EnteringCar");
        checkPoint.SetActive(false);
        StartCoroutine(MoveToTarget(CarSItPos));
    }

    public void CarExit()
    {
        gameObject.GetComponent<Animator>().Play("ExitingCar");

        PlayerManager.instance.CarPlayer.transform.SetParent(null);
        checkPoint.SetActive(true);

        StartCoroutine(MoveToTarget(CarExitPos));
    }


    IEnumerator MoveToTarget(Transform target)
    {
        while (Vector3.Distance(transform.position, target.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * moveSpeed);
            yield return null;
        }

        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
