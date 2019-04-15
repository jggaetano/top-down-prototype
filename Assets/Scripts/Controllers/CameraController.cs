using UnityEngine;

public class CameraController : MonoBehaviour {

    Area area
    {
        get { return AreaController.Instance.area; }
    }

    float xMargin = 0f; // Distance in the x axis the player can move before the camera follows.
    float yMargin = 0f; // Distance in the y axis the player can move before the camera follows.
    Vector2 maxXAndY; // The maximum x and y coordinates the camera can have.
    Vector2 minXAndY; // The minimum x and y coordinates the camera can have.
    float offset = 0.5f;

    private Transform m_Player; // Reference to the player's transform.


    void Start()
    {
        // Setting up the reference. TODO Make this dynamic based on area.activeHero
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        Camera.main.transform.position = new Vector3(m_Player.position.x + offset, m_Player.position.y + offset, -10);

        minXAndY = Vector2.zero;
        maxXAndY = new Vector2(area.Width, area.Height);
    }


    bool CheckXMargin()
    {
        // Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
        return Mathf.Abs(Camera.main.transform.position.x - m_Player.position.x) > xMargin;
    }


    bool CheckYMargin()
    {
        // Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
        return Mathf.Abs(Camera.main.transform.position.y - m_Player.position.y) > yMargin;
    }


    void Update()
    {
        TrackPlayer();
    }


    void TrackPlayer()
    {
        // By default the target x and y coordinates of the camera are it's current x and y coordinates.
        float targetX = Camera.main.transform.position.x;
        float targetY = Camera.main.transform.position.y;

        // If the player has moved beyond the x margin...
        if (CheckXMargin())
        {
            // ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
            targetX = m_Player.position.x + offset; // Mathf.Lerp(Camera.main.transform.position.x, m_Player.position.x, xSmooth * Time.deltaTime);
        }

        // If the player has moved beyond the y margin...
        if (CheckYMargin())
        {
            // ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
            targetY = m_Player.position.y + offset; // Mathf.Lerp(Camera.main.transform.position.y, m_Player.position.y, ySmooth * Time.deltaTime);
        }

        // The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
        targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
        targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);

        // Set the camera's position to the target position with the same z component.
        Camera.main.transform.position = new Vector3(targetX, targetY, -10);
    }
}

