using Lucky.Inputs.VirtualInputs;

namespace Lucky.Inputs.VirtualInputSet
{
    /// <summary>
    /// VirtualInput 集合
    /// </summary>
    public class BasicInputSet : InputSet
    {
        public BasicInputSet(BindingSet bindingSet) : base(bindingSet)
        {
            Left = new VirtualButton(bindingSet.LeftBinding, 0);
            Right = new VirtualButton(bindingSet.RightBinding, 0);
            Up = new VirtualButton(bindingSet.UpBinding, 0);
            Down = new VirtualButton(bindingSet.DownBinding, 0);
            Restart = new VirtualButton(bindingSet.RestartBinding, 0);

            MenuLeft = new VirtualButton(bindingSet.MenuLeftBinding, 0).SetRepeat(0.6f, 0.02f);
            MenuRight = new VirtualButton(bindingSet.MenuRightBinding, 0).SetRepeat(0.6f, 0.02f);
            MenuUp = new VirtualButton(bindingSet.MenuUpBinding, 0).SetRepeat(0.6f, 0.02f);
            MenuDown = new VirtualButton(bindingSet.MenuDownBinding, 0).SetRepeat(0.6f, 0.02f);
            Backspace = new VirtualButton(bindingSet.BackspaceBinding, 0).SetRepeat(0.6f, 0.02f);
            Tab = new VirtualButton(bindingSet.TabBinding, 0).SetRepeat(0.6f, 0.02f);

            // MoveX = new VirtualIntegerAxis(BindingSet.LeftBinding, BindingSet.RightBinding);
        }

        public VirtualButton Left;
        public VirtualButton Right;
        public VirtualButton Up;
        public VirtualButton Down;
        public VirtualButton Restart;

        public VirtualButton MenuLeft;
        public VirtualButton MenuRight;
        public VirtualButton MenuUp;
        public VirtualButton MenuDown;
        public VirtualButton Backspace;

        public VirtualButton Tab;
        // public VirtualIntegerAxis MoveX;
    }
}