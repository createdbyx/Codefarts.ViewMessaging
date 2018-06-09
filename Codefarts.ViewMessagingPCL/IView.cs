namespace Codefarts.ViewMessaging
{
    using System;
    using System.Collections.Generic;

    public interface IView
    {
        string ViewPath { get; }
        string ViewId { get; }
        object ViewReference { get; }
        void SendMessage(IDictionary<string, object> args);
    }
}