using PathCreation;
using UnityEngine;

namespace Track {
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class AutoPathFollower : MonoBehaviour {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        private float _distanceTravelled;

        private void Start() {
            if (pathCreator != null) {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        private void FixedUpdate() {
            if (pathCreator == null) return;
            _distanceTravelled += speed * Time.fixedDeltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(_distanceTravelled, endOfPathInstruction) +
                                 new Vector3(0, 0.5f, 0);
            transform.rotation = pathCreator.path.GetRotationAtDistance(_distanceTravelled, endOfPathInstruction);
            transform.Rotate(0, 0, 90);
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        private void OnPathChanged() {
            _distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}