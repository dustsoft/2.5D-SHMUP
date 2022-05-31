using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f; // Powerup Item Speed
    [SerializeField] private int powerupID; // ID 0 = Tripleshot, ID 1 = Speedboost, ID 2 = Shields;
    [SerializeField] private AudioClip _clip;



    void Update()
    {
        //translates the transform component's vector3 down - moves down the screen
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y  < -4.5f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_clip, transform.position);

            if (player != null)
            {
                switch(powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                }
            }

            Destroy(this.gameObject);
        }

    }
}
