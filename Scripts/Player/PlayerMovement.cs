
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AICharacterControl))]
[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
    ThirdPersonCharacter m_Character = null;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster = null;
    private Vector3 currentDestination;
    private Vector3 clickPoint;
    NavMeshAgent agent = null;

    GameObject walkTarget = null; //point needs to be a gameobject initially for ai method
    AICharacterControl aiCharContr = null;
    //layers to swap on
    [SerializeField] const int environment = 9;
    [SerializeField] const int enemy = 10;
    [SerializeField] const int walkable = 11;
    [SerializeField] const int terrain = 14;
    public bool currentlyControlled = true;
    public LayerMask movementMask;

    private void Awake()
    {
        //cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        m_Character = GetComponent<ThirdPersonCharacter>();
        currentDestination = transform.position;
        aiCharContr = GetComponent<AICharacterControl>();
      //  walkTarget = new GameObject("Walk Target");
       // cameraRaycaster.notifyMouseClickObservers += ProcessMouseClick;
        agent = GetComponent<NavMeshAgent>();
    }

    //same signature as notifyMouseClickObservers ... not working properly.
    //put back camera raycaster script if want to retest this.
    /*
    void ProcessMouseClick(RaycastHit raycastHit, int layerHit) {

        if (currentlyControlled) {
           switch (layerHit) {
                case (environment):
                    break;
                case (walkable):
                    walkTarget.transform.position = raycastHit.point;
                    aiCharContr.SetTarget(walkTarget.transform);
                    break;
                case (terrain):
                    walkTarget.transform.position = raycastHit.point;
                    //unity tut code:
                    agent.destination = (raycastHit.point); 
                  // aiCharContr.SetTarget(walkTarget.transform);
                    break;
                case (enemy):
                    GameObject enemygo = raycastHit.collider.gameObject;
                    aiCharContr.SetTarget(enemygo.transform);             
                    break;
                default:
                    Debug.LogWarning("Not sure how to handle mouse click in playermovement.");
                    break;

            }
        }  

    }
    */
    //sample unity code
    void Update() {

     // interact on rightclick
       

    }

    public void MoveHere() {
        RaycastHit hit;
        //using mask to filter movement from objects 
      
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
       
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, movementMask)) {
           // Debug.Log(agent.destination + "travelling to !" + hit.point);
           
            agent.destination = hit.point;
        }
        //Debug.Log(hit.point + " is dest");
    }
   

        //private void OnDrawGizmos() {

        //    //drawing movement gizmos
        //    Gizmos.color = Color.black;
        //    if (gameObject.tag == "Selected") {
        //        Gizmos.DrawLine(transform.position, currentDestination);
        //        Gizmos.DrawSphere(currentDestination, 0.1f);
        //        Gizmos.DrawSphere(clickPoint, 0.5f);
        //    }
        //}
        //TODO consider deregistering OnLayerChanged on leaving game scenes.
    }

