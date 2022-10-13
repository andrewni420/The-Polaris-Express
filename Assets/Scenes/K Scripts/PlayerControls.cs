using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerControls : MonoBehaviour {

    public Vector2 moveValue;
    public float speed;

    void FixedUpdate() {
        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);

        GetComponent<Rigidbody>().AddForce(movement * speed * Time.fixedDeltaTime); 
    }

    void OnMove(InputValue value) {
        moveValue = value.Get<Vector2>();
    }
}