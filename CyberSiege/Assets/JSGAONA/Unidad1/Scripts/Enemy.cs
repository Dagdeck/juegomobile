using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Assets.JSGAONA.Unidad1.Scripts {

    // Componentes requeridos para que funcione el script
    [RequireComponent(typeof(NavMeshAgent))]

    // Este script se emplea para gestionar la logica de los enemigos
    public class Enemy : MonoBehaviour {
        
        // Variables visibles desde el inspector de Unity
        [SerializeField] private float chaseDistance = 2f;
        [SerializeField] private float updateInterval  = 0.25f;
        [SerializeField] private float stopDistance = 2f;

        // Variables ocultas desde el inspector de Unity
        private bool isPlayerInRange = false;
        private bool isPlayerClose = false;
        private NavMeshAgent agent;
        private Transform player;
        public Transform Player { set => player = value; }
        private Vector3 initialPosition;




        // Metodo de llamada de Unity, se llama una unica vez al iniciar el app, es el primer
        // metodo en ejecutarse, se realiza la asignacion de componentes
        private void Awake() {
            agent = GetComponent<NavMeshAgent>();
            initialPosition = transform.position;
        }


        // Metodo de llamada de Unity, se llama una unica vez al iniciar el app, se ejecuta despues
        // de Awake, se realiza la asignacion de variables y configuracion del script
        private void Start() {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;

            agent.stoppingDistance = stopDistance;

            StartCoroutine(UpdateEnemyBehavior());



        }
        
        // Metodo de llamada de Unity, se llama en el momento de que el GameObject es destruido
        private void OnDestroy() {
            StopCoroutine(UpdateEnemyBehavior());
        }


        // Metodo de llamada de Unity, se activa cuando el renderizador del objeto entra en el campo
        // de vision de la camara activa
        private void OnBecameVisible() {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
        }


        // Metodo de llamada de Unity, se activa cuando el renderizador del objeto sale del campo de
        // vision de la camara.
        private void OnBecameInvisible() {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        }


        // Coroutine para optimizar las actualizaciones
        private IEnumerator UpdateEnemyBehavior() {
            // Mientras sea verdad, se ejecuta indefinidamente
            while (true) {

                float distance = Vector3.Distance(player.position, transform.position);
                isPlayerInRange = distance <= chaseDistance;
                isPlayerClose = distance <= stopDistance;

                yield return new WaitForSeconds(updateInterval);
            }

        



        // Metodo de llamada de Unity, se llama en cada frame del computador
        // Se realiza la logica de control del enemigo
      
        }
        private void Update()
        {
            if (player != null && isPlayerInRange && !isPlayerClose)
            {
                agent.SetDestination(player.position);
            }
            else if (player != null && isPlayerClose) agent.ResetPath();
            else agent.SetDestination(initialPosition);
        }


    }
}
