namespace Codefarts.ViewMessaging
{
    using System;

    public class ViewModelNotResolvedException : Exception
    {
        public string Name
        {
            get; private set;
        }

        public ViewModelNotResolvedException(string name)
            : base()
        {
            this.Name = name;
        }

        public ViewModelNotResolvedException(string message, string name)
            : base(message)
        {
            this.Name = name;
        }
    }
}
