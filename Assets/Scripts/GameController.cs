using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public GameObject player;
    public GameObject mainCamera;
    public Text uiDeaths;
    public GameObject background;
    
    private Vector2 playerPos2D
    {
        get { return new Vector2(player.transform.position.x, player.transform.position.y); }
    }

    private Vector3 velocity;
    private float smoothTime = 0.2f;

    void FixedUpdate()
    {

        var camZ = mainCamera.transform.position.z;
        var playerPos = playerPos2D;
        var camPos = new Vector3(playerPos.x, playerPos.y, camZ);
        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, camPos, ref velocity, smoothTime);

        var bgPos = new Vector3(playerPos.x, playerPos.y, 0);
        background.transform.position = Vector3.SmoothDamp(background.transform.position, bgPos, ref velocity, smoothTime);

    }
}
