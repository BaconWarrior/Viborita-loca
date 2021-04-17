using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class SnakeAgent : Agent
{
    public Transform Target;
    public bool chocaste;
    public List<Transform> Tails;
    [Range(0, 2)]
    public float bonesDistance;
    public GameObject BonePrefab;
    [Range(0, 4)]
    public float speed;
    public float angle;

    public void movimiento(float angulo)
    {
        MoveSnake(transform.position + transform.forward * speed);
        //angle = Input.GetAxis("Horizontal") * 4;
        angle = angle * 4;
        transform.Rotate(0, angulo, 0);
    }

    private void MoveSnake(Vector3 newPosition)
    {
        float sqrDistance = bonesDistance * bonesDistance;
        Vector3 previousPosition = transform.position;


        foreach (var bone in Tails)
        {
            if ((bone.position - previousPosition).sqrMagnitude > sqrDistance)
            {
                var temp = bone.position;
                bone.position = previousPosition;
                previousPosition = temp;
            }
            else
            {
                break;
            }
        }
        transform.position = newPosition;
    }
    public override void OnEpisodeBegin()
    {
        //Si el agente sale del area de entrenamiento se castiga
        if (this.chocaste)
        {
            this.chocaste = false;
            Debug.Log("Anarquia");
            this.transform.rotation = Quaternion.identity;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
            Tails.Clear();
        }
        //Mover Target
        Target.localPosition = new Vector3(Random.Range(-10.0f, 10.0f), 0.5f, Random.Range(-10.0f, 10.0f));
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //percibir las posiciones del agente y del objetivo
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        //rotacion
        sensor.AddObservation(this.transform.rotation);
        sensor.AddObservation(this.angle);
        //sensor.AddObservation(controlador.speed);
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //Vectores de accion
        float controlSignal = actionBuffers.ContinuousActions[0];

        movimiento(controlSignal);
        Debug.Log(controlSignal);
        //rb.AddForce(controlSignal * forceMultiplier);

        //Recompensas
        float distanceToTarger = Vector3.Distance(this.transform.position, Target.localPosition);

        //Distancia al objetivo
        if (distanceToTarger < 1.0f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        //Casitigo (Si choca con muros o su cola)
        else if (this.chocaste)
        {
            SetReward(-2.0f);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("muro")/* || other.transform.CompareTag("tail")*/)
        {
            this.chocaste = true;   
        }
    }
}
