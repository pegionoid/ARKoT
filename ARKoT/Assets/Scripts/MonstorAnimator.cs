namespace ARKoT
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonstorAnimator : StateMachineBehaviour
    {
        [SerializeField]
        public eDiceEye diceEye;

        // OnStateExit is called before OnStateExit is called on any state inside this state machine
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
            Debug.Log($"[Debug@OnStateExit]{animator.name}:{diceEye}");
            //switch (diceEye)
            //{
            //    case eDiceEye.Attack:
            //        animator.SetInteger("Attack", 0);
            //        break;

            //    case eDiceEye.Heal:
            //        animator.SetInteger("Heal", 0);
            //        break;

            //    case eDiceEye.Energy:
            //        animator.SetInteger("Charge", 0);
            //        break;

            //    case eDiceEye.OnePoint:
            //    case eDiceEye.TwoPoint:
            //    case eDiceEye.ThreePoint:
            //        animator.SetInteger("Destruct", 0);
            //        break;

            //    default:
            //        break;
            //}
        }

        //// OnStateMachineExit is called when exiting a state machine via its Exit Node
        override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            Debug.Log($"[Debug@OnStateMachineExit]{animator.name}:{diceEye}");
            switch (diceEye)
            {
                case eDiceEye.Attack:
                    animator.SetInteger("Attack", 0);
                    break;

                case eDiceEye.Heal:
                    animator.SetInteger("Heal", 0);
                    break;

                case eDiceEye.Energy:
                    animator.SetInteger("Charge", 0);
                    break;

                case eDiceEye.OnePoint:
                case eDiceEye.TwoPoint:
                case eDiceEye.ThreePoint:
                    animator.SetInteger("Destruct", 0);
                    break;

                default:
                    break;
            }

        }
    }
}

