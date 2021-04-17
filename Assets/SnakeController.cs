using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public List<Transform> Tails;
    [Range(0, 2)]
    public float bonesDistance;
    public GameObject BonePrefab;
    [Range(0, 4)]
    public float speed;
    private Transform _transform;
    public float angle;
    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //movimiento(angle);
    }

    public void movimiento(float angulo)
    {
        MoveSnake(_transform.position + _transform.forward * speed);
        //angle = Input.GetAxis("Horizontal") * 4;
        angle = angle * 4;
        _transform.Rotate(0, angulo, 0);
    }

    private void MoveSnake(Vector3 newPosition)
    {
        float sqrDistance = bonesDistance * bonesDistance;
        Vector3 previousPosition = _transform.position;


        foreach(var bone in Tails)
        {
            if((bone.position - previousPosition).sqrMagnitude > sqrDistance)
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
        _transform.position = newPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.CompareTag("food"))
        {
            //Destroy(collision.gameObject);
            var bone = Instantiate(BonePrefab);
            Tails.Add(bone.transform);
        }
    }
}
