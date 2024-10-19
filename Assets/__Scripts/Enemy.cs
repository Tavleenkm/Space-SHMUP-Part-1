using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The movement speed is 10m/s.
/// Seconds before this enemy explodes = 10.
/// Damage needed to destroy this enemy.
/// Points earned for destroying this enemy.
/// </summary>
[RequireComponent(typeof(BoundsCheck))]  // a

public class Enemy : MonoBehaviour {
    [Header("Inscribed")]
    public float speed = 10f;
    public float health = 10;
    public int score = 100;
    public float powerUpDropChance = 1f;   // Chance to drop a PowerUp (0-1)

    protected bool calledShipDestroyed = false;


    protected BoundsCheck bndCheck;

    void Awake() {
        bndCheck = GetComponent<BoundsCheck>();
    }

    // This is a Property: A method that acts like a field
    public Vector3 pos {
        get {
            return this.transform.position;
        }
        set {
            this.transform.position = value;
        }
    }

    void Update() {
        Move();  // b


        if (!bndCheck.isOnScreen) {  // d
            if (pos.y < bndCheck.camHeight - bndCheck.radius) {
                // We're off the bottom, so destroy this GameObject
                Destroy(gameObject);
            }
        }
    }

    public virtual void Move() {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;  // c
    }

   

    void OnCollisionEnter(Collision coll) {
        GameObject otherGO = coll.gameObject;

        // Check for collisions with ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null) {
            // Only damage this Enemy if it's on screen
            if (bndCheck.isOnScreen) {
                // Get the damage amount from the Main WEAP_DICT
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;
                if (health <= 0) {
                   // Tell Main that this ship was destroyed
                    if (!calledShipDestroyed) {
                        calledShipDestroyed = true;
                        Main.SHIP_DESTROYED( this );
                    }
                    // Destroy this Enemy
                    Destroy( this.gameObject );
                }
            }

            // Destroy the ProjectileHero regardless
            Destroy(otherGO);
        } else {
            print("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }
    }

}
