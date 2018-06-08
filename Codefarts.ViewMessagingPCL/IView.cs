namespace Codefarts.ViewMessaging
{
    using System;
    using System.Collections.Generic;

    public interface IView
    {
        string ViewPath { get; }
        Guid ViewId { get; }
        object ViewReference { get; }
        void SendMessage(IDictionary<string, string> args);
    }
}