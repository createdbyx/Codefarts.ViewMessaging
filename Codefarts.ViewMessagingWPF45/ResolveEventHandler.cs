namespace Codefarts.ViewMessaging
{
    using System;

    public delegate object ResolveEventHandler(object sender, ResolveEventArgs args);

    public class ResolveEventArgs : EventArgs
    {
        public ResolveEventArgs(Type type)
        {
            this.Type = type;
        }

        public Type Type
        {
            get; private set;
        }
    }
}