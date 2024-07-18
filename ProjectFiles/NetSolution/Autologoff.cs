#region Using directives
using UAManagedCore;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
using System;
#endregion

public class Autologoff : BaseNetLogic
{
    public override void Start()
    {

        //duration = LogicObject.GetVariable("Period");
        //Log.Info(duration.Value.ToString());

        enabledVariable = LogicObject.GetVariable("Enabled");
        if (enabledVariable == null)
            throw new CoreConfigurationException("Unable to find Enabled variable");

        CycleCounter = LogicObject.GetVariable("ElapsedTime_s");
        CycleCounter.Value = 0;

        enabledVariable.VariableChange += EnabledVariable_VariableChange;
        actionTask = new PeriodicTask(PeriodicMethodInvocation, 1000, LogicObject);

        if (enabledVariable.Value)
            actionTask.Start();
    }

    public override void Stop()
    {
        enabledVariable.VariableChange -= EnabledVariable_VariableChange;
    }

    private void EnabledVariable_VariableChange(object sender, VariableChangeEventArgs e)
    {
        if (!e.NewValue)
            StopPeriodicTask();
        else
            RestartPeriodicTask();

        CycleCounter.Value = 0;
    }

    private void StopPeriodicTask()
    {
        actionTask?.Dispose();
        actionTask = null;
    }

    private void RestartPeriodicTask()
    {
        StopPeriodicTask();
        actionTask = new PeriodicTask(PeriodicMethodInvocation, 1000, LogicObject);
        actionTask.Start();
    }

    private void PeriodicMethodInvocation()
    {
        if (Session.User.BrowseName.ToString() == "Anonymous")
            CycleCounter.Value=0;
        else
            CycleCounter.Value++;
    }

    private PeriodicTask actionTask;
    private IUAVariable duration;
    private IUAVariable enabledVariable;
    //private MethodInvocation actionMethodInvocation;
    private IUAVariable CycleCounter;


}
