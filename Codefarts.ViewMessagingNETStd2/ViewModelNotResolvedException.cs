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

        public ViewModelNotResolvedException(string name, Exception innerException)
            : base(string.Empty, innerException)
        {
            this.Name = name;
        }

        public ViewModelNotResolvedException(string message, string name, Exception innerException)
            : base(message, innerException)
        {
            this.Name = name;
        }
    }
}
