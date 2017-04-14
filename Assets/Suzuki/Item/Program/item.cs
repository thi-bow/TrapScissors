using UnityEngine;
using System.Collections;

public class item : MonoBehaviour {

    public float i_speed = 1.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(4.0f, 1.0f, 1.0f));
	}

    public void OnCollisionEnter2D(Collision2D collision)
    {
        var tag = collision.gameObject.tag;
        if (tag == "Player")
        {
            Destroy(gameObject);
        }

    }
}
