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
    bool goal;
    public List<Transform> Tails;
    [Range(0, 2)]
    public float bonesDistance;
    public GameObject BonePrefab;
    [Range(0, 4)]
    public float speed;
    public float angle;

    public int contador;
    public int puntaje;
    public void movimiento(float angulo)
    {
        MoveSnake(transform.position + transform.forward * speed);
        //angle = Input.GetAxis("Horizontal") * 4;
        this.transform.Rotate(0, angulo, 0);
        this.contador++;
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
        this.contador = 0;
        this.puntaje = 0;
        //Si el agente sale del area de entrenamiento se castiga
        if (this.chocaste)
        {
            this.chocaste = false;
            this.transform.rotation = Quaternion.identity;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
            for(int i = 1; i < Tails.Count; i++)
            {
                Tails.Remove(Tails[i]);
            }
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
        float controlSignal = 0.0f;
        controlSignal = actionBuffers.ContinuousActions[0];

        movimiento(Mathf.Clamp(controlSignal, -3.0f, 3.0f));
        //Debug.Log(controlSignal);
        //rb.AddForce(controlSignal * forceMultiplier);

        //Recompensas
        float distanceToTarger = Vector3.Distance(this.transform.position, Target.localPosition);

        //print(distanceToTarger);
        //Distancia al objetivo
        if (this.goal)
        {
            this.goal = false;
            SetReward(2.0f);
            contador = 0;
            puntaje++;
        }
        //Casitigo (Si choca con muros o su cola)
        if (this.chocaste)
        {
            SetReward(-3.0f);
            EndEpisode();
        }
        if (contador >= 1200)
        {
            SetReward(-0.5f);
            contador = 0;
        }
        if (puntaje >= 1)
        {
            SetReward(3.0f);
            
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("muro")/* || other.transform.CompareTag("tail")*/)
        {
            Debug.Log("Bonk!!!!!");
            this.chocaste = true;   
        }
        if (other.transform.CompareTag("food"))
        {
            Debug.Log("yum");
            this.goal = true;
            this.Target.localPosition = new Vector3(Random.Range(-10.0f, 10.0f), 0.5f, Random.Range(-10.0f, 10.0f));
        }
    }

}
