﻿using UnityEngine;
using System.Collections;
using System;


public class RollerBladeMovement : MonoBehaviour
{
    // references
    public Character character;
    public CharMoveInfoHub move_infohub;
    public CharAimInfoHub aim_infohub;
    public Transform graphics_object;


    // movement
    public float speed = 20f;

    public float rotate_speed = 5f;
    private float rotation = 0; // radians

    private float drag = 0.5f, stunned_drag = 4f;
    private float break_drag = 6.5f; // amount of drag when breaking
    // the max speed at which a break turn can happen
    private float break_turn_speed_threshold = 8f;

    public Vector2 direction = new Vector2();
    private Vector2 velocity_last;


    // input
    private float input_turn = 0;
    private bool input_fwrd = false;
    private bool input_break = false;




    // PUBLIC MODIFIERS

    public void Start()
    {
        if (!graphics_object) Debug.LogWarning("No graphics object specified");

        // events
        aim_infohub.event_set_aim_with_direction += new EventHandler<EventArgs<Vector2>>(OnSetAim);
        aim_infohub.event_set_aim_with_rotation += new EventHandler<EventArgs<float>>(OnSetAim);
        move_infohub.event_knockback += new EventHandler<EventArgs<Vector2>>(OnKnockBack);
    }

    public void Update()
    {
        if (character.IsStunned() || !character.IsAlive()) return;


        // rotation
        rotation -= input_turn * rotate_speed * Time.deltaTime;
        graphics_object.localEulerAngles = new Vector3(0, 0, (rotation * Mathf.Rad2Deg) - 90);
        direction = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation));

        // inform infohub
        aim_infohub.InformAimDirection(direction);
        aim_infohub.InformAimRotation(rotation);

        // reset input variables
        input_turn = 0;
    }
    public void FixedUpdate()
    {
        velocity_last = rigidbody2D.velocity;


        // incapacitated character
        if (character.IsStunned() || !character.IsAlive())
        {
            // drag
            rigidbody2D.velocity /= 1 + stunned_drag * Time.deltaTime;
            return;
        }
        

        // regular movement
        if (input_break)
        {
            // breaking drag
            rigidbody2D.velocity /= 1 + break_drag * Time.deltaTime;
        }
        else
        {
            // faster than max roller blade speed, or not trying to move forward
            if (!input_fwrd || rigidbody2D.velocity.magnitude > speed)
            {
                // drag
                rigidbody2D.velocity /= 1 + drag * Time.deltaTime;
                rigidbody2D.velocity = direction * rigidbody2D.velocity.magnitude;
            }

            // trying to move forward
            else if (input_fwrd)
            {
                rigidbody2D.velocity = direction * Mathf.Max(speed, rigidbody2D.velocity.magnitude);
            }
        }


        // inform infohub
        move_infohub.InformVelocity(rigidbody2D.velocity);
        move_infohub.InformVelocityLastFrame(velocity_last);

        // reset input variables
        input_fwrd = false;
        input_break = false;
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "wall")
        {
            TurnAwayFromWall(collision.contacts[0]);
        }
    }


    public void OnEnable()
    {
        rigidbody2D.velocity = move_infohub.GetVelocity();


        if (!character.IsStunned())
        {
            // set rotation based on movement direction coming off a track
            rotation = Mathf.Atan2(rigidbody2D.velocity.y, rigidbody2D.velocity.x);
            SetAim(rotation);
        }
        else
        {
            // maintain aim direction
            rotation = aim_infohub.GetAimRotation();
            SetAim(rotation);
        }
    }
    public void OnDisable()
    {
        rigidbody2D.velocity = Vector2.zero;
    }

    // input
    public void MoveForward()
    {
        if (!character.IsStunned()) input_fwrd = true;
    }
    public void Break()
    {
        if (!character.IsStunned()) input_break = true;
    }
    public bool BreakTurn()
    {
        if (character.IsStunned()) return false;

        // only turn once and when moving slow enough
        if (rigidbody2D.velocity.magnitude <= break_turn_speed_threshold)
        {
            rotation += Mathf.PI;
            graphics_object.rotation = Quaternion.Euler(0, 0, (rotation * Mathf.Rad2Deg) - 90);
            return true;
        }
        return false;
    }
    public void Turn(float direction)
    {
        if (!character.IsStunned())
            input_turn = direction == 0 ? 0 : direction > 0 ? 1 : -1;
    }

    // setters
    public void SetAim(float rotation)
    {
        this.rotation = rotation;
        direction = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation));
        aim_infohub.InformAimRotation(rotation);
        aim_infohub.InformAimDirection(direction);
    }
    public void SetAim(Vector2 direction)
    {
        this.direction = direction;
        rotation = Mathf.Atan2(direction.y, direction.x);
        aim_infohub.InformAimRotation(rotation);
        aim_infohub.InformAimDirection(direction);
    }

    // other
    public void KnockBack(Vector2 force)
    {
        if (character.IsStunned())
        {
            rigidbody2D.velocity = force.normalized * Mathf.Min(force.magnitude, speed);
            move_infohub.InformVelocity(rigidbody2D.velocity);
        }
        else
        {

        }

    }


    // PRIVATE MODIFIERS

    private void TurnAwayFromWall(ContactPoint2D contact)
    {
        Vector2 perp = GeneralHelpers.Perpendicular(contact.normal);

        /*
        float dot = Vector2.Dot(direction, perp);
        float turn_dir = 1;
        turn_dir = dot == 0 ? turn_dir : dot > 0 ? 1 : -1;
        */
 
        direction = Vector2.Lerp(direction,  contact.normal, Time.deltaTime * 4f);
        SetAim(direction);
    }

    // events
    private void OnSetAim(object sender, EventArgs<float> e)
    {
        SetAim(e.Value);
    }
    private void OnSetAim(object sender, EventArgs<Vector2> e)
    {
        SetAim(e.Value);
    }
    private void OnKnockBack(object sender, EventArgs<Vector2> e)
    {
        KnockBack(e.Value);
    }


    // PUBLIC ACCESSORS

}
