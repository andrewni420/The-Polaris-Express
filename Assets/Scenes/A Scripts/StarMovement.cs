using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMovement : MonoBehaviour
{
    public AnimationCurve xCurve;
    public AnimationCurve yCurve;
    public AnimationCurve zCurve;

    public Vector3 finalPosition;
    public Vector3 startPosition;
    public float animationLength;
    public float animationTimer=0;

    public float rotSpeed;
    public bool touched = false;

    void Start()
    {
        startPosition = transform.position;
        animationTimer = 0;
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, rotSpeed, 0), Space.World);
        if (touched)
        {
            if (animationTimer < animationLength)
            {
                float time = Time.deltaTime;
                animationTimer += time;
                float x = animationTimer / animationLength;
                float xPos = xCurve.Evaluate(x) * (finalPosition.x - startPosition.x)+startPosition.x;
                float yPos = yCurve.Evaluate(x) * (finalPosition.y - startPosition.y)+startPosition.y;
                float zPos = zCurve.Evaluate(x) * (finalPosition.z - startPosition.z)+startPosition.z;
                transform.position = new Vector3(xPos, yPos, zPos);
            }
            else
            {
                transform.position = finalPosition;
                animationTimer = animationLength;
            }
            
        }
    }

    public void onTouch()
    {
        touched = true;
        Debug.Log(touched);
    }


    public bool inAnimation()
    {
        return animationTimer < animationLength;
    }


}
