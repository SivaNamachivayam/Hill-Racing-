using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RPMmeter : MonoBehaviour
{
    private const float max_SPEED_ANGLE = -43;
    private const float zero_SPEED_ANGLE = 225;

    private Transform needleTransfrom;
    private Transform speedLableTemplate;

    private float speedMax;
    private float speed;

    private void Awake()
    {
        needleTransfrom = transform.Find("needle");
        speedLableTemplate = transform.Find("speedLableTemplate");
        speedLableTemplate.gameObject.SetActive(false);

        speed = 0f;
        speedMax = 10;
        CreatingSpeedLables();
    }

    void Update()
    {
        /*speed += 30f * Time.deltaTime;
        if (speed > speedMax)
        {
            speed = speedMax;
        }*/
        HandlingPlayerInput();
        needleTransfrom.eulerAngles = new Vector3(0, 0, GettingSpeedRotation());
    }

    public void HandlingPlayerInput()
    {
        if (GameManager.Instance.GasBtnPressed)
        {
            float acceleration = 50f;
            speed += acceleration * Time.deltaTime;
        }
        else
        {
            float deceleration = 30f;
            speed -= deceleration * Time.deltaTime;
        }
        
        speed = Mathf.Clamp(speed, 0f, speedMax);
    }

    private void CreatingSpeedLables()
    {
        int lableAmount = 10;
        float totalAngleSize = zero_SPEED_ANGLE - max_SPEED_ANGLE;

        for (int i = 0; i <= lableAmount; i++)
        {
            Transform speedLableTransform = Instantiate(speedLableTemplate, transform);
            float lableSpeedNormalized = (float)i / lableAmount;
            float speedLabelAngle = zero_SPEED_ANGLE - lableSpeedNormalized * totalAngleSize;
            speedLableTransform.eulerAngles = new Vector3(0, 0, speedLabelAngle);
            speedLableTransform.Find("speedText").GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(lableSpeedNormalized * speedMax).ToString();
            speedLableTransform.Find("speedText").eulerAngles = Vector3.zero;
            speedLableTransform.gameObject.SetActive(true);
        }
        needleTransfrom.SetAsLastSibling();
    }

    public float GettingSpeedRotation()
    {
        float totalAngleSize = zero_SPEED_ANGLE - max_SPEED_ANGLE;

        float speedNormalized = speed / speedMax;

        return zero_SPEED_ANGLE - speedNormalized * totalAngleSize;
    }
}
