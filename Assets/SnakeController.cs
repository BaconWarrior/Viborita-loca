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
    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveSnake(_transform.position + _transform.forward * speed);
        float angle = Input.GetAxis("Horizontal") * 4;
        _transform.Rotate(0, angle, 0);
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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.CompareTag("food"))
        {
            Destroy(collision.gameObject);
            var bone = Instantiate(BonePrefab);
            Tails.Add(bone.transform);
        }
    }
}
