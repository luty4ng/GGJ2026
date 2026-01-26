using UnityEngine;

namespace GameLogic
{
    public class StateA : StateBase<StateMachineExample>
    {
        public StateA(StateMachineExample context) : base(context) { }
        public override void Update() => Debug.Log("This is StateA");
        public override void OnEnter() { }
        public override void OnExit() { }
    }

    public class StateB : StateBase<StateMachineExample>
    {
        public StateB(StateMachineExample context) : base(context) { }
        public override void Update() => Debug.Log("This is StateB");
        public override void OnEnter() { }
        public override void OnExit() { }
    }

    public class StateMachineExample : MonoBehaviour
    {
        private StateMachine m_stateMachine;
        [SerializeField] private string m_currentState;

        private StateA m_stateA;
        private StateB m_stateB;

        private void Awake()
        {
            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            m_stateMachine = new StateMachine();
            m_stateA = new StateA(this);
            m_stateB = new StateB(this);

            System.Func<bool> aToB = () => Random.Range(0f, 1f) >= 0.3f;
            System.Func<bool> bToA = () => Random.Range(0f, 1f) >= 0.6f;

            m_stateMachine.AddTransition(m_stateA, m_stateB, aToB);
            m_stateMachine.AddTransition(m_stateB, m_stateA, bToA);

            m_stateMachine.SetState(m_stateA);
        }

        private void Update()
        {
            m_stateMachine.Update();
            m_currentState = m_stateMachine.GetCurrentState()?.ToString() ?? "None";
        }
    }

}
