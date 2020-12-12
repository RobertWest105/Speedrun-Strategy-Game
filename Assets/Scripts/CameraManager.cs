using UnityEngine;

public class CameraManager : MonoBehaviour {

    float cameraSpeed = 5;
    float xMax, yMax;
    public static Vector3 originPos { get; private set; }

	// Use this for initialization
	void Start () {
	    originPos = Camera.main.ScreenToWorldPoint(Vector3.zero);
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKey(KeyCode.W)) {
            Camera.main.transform.Translate(Vector3.up * cameraSpeed * Time.deltaTime);
        }else if(Input.GetKey(KeyCode.A)) {
            Camera.main.transform.Translate(Vector3.left * cameraSpeed * Time.deltaTime);
        }else if(Input.GetKey(KeyCode.S)) {
            Camera.main.transform.Translate(Vector3.down * cameraSpeed * Time.deltaTime);
        }else if(Input.GetKey(KeyCode.D)) {
            Camera.main.transform.Translate(Vector3.right * cameraSpeed * Time.deltaTime);
        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, 0, xMax), Mathf.Clamp(transform.position.y, 0, yMax), -1);
    }

    //Set the end points of the camera's range so it can't go beyond the map
    public void setCameraLimits(Vector3 endTile, float tileSize) {
        Vector3 cameraEnd = Camera.main.ViewportToWorldPoint(new Vector3(1, 1));
        xMax = endTile.x - cameraEnd.x + tileSize;
        yMax = endTile.y - cameraEnd.y + tileSize;
    }
}