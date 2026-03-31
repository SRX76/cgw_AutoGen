public class FsmMachine
{
    public FsmMachine()
    {
        Init();
    }
    void Init()
    {
        EventCenter.OnUpdate += Update;
    }
    private IFsmStateBase State { get; set; }

    public void SwitchState(FsmStateBase state)
    {
        if (State != null)
        {
            State.Exit();
        }

        if (state != null)
        {
            state.Enter();
        }
        State = state;
    }

    public void Update(float dtTime)
    {
        State?.Update();
        State?.Update(dtTime);
    }
}

public interface IFsmStateBase
{
    void Enter();
    void Update();
    void Update(float dtTime);
    void Exit();
}
public class FsmStateBase : IFsmStateBase
{
    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Update(float dtTime)
    {

    }
}

