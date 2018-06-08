namespace Codefarts.ViewMessaging
{
    using System;
    using System.Collections.Generic;

    public class GenericMessageArguments
    {
        public static IDictionary<string, string> Show
        {
            get
            {
                var arguments = new Dictionary<string, string>();
                arguments["show"] = string.Empty;
                return arguments;
            }
        }

        public static IDictionary<string, string> ShowDialog(Guid viewId)
        {
            var arguments = new Dictionary<string, string>();
            arguments["showdialog"] = viewId.ToString();
            return arguments;
        }
    }
}