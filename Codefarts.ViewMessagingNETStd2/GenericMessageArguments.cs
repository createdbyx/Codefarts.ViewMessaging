namespace Codefarts.ViewMessaging
{
    using System.Collections.Generic;

    public class GenericMessageArguments
    {
        public static ViewArguments Build(params IDictionary<string, object>[] parts)
        {
            var args = new Dictionary<string, object>();
            foreach (var dictionary in parts)
            {
                foreach (var pair in dictionary)
                {
                    args[pair.Key] = pair.Value;
                }
            }

            return new ViewArguments(args);
        }


        public static ViewArguments Show()
        {
            var arguments = new Dictionary<string, object>();
            arguments[GenericMessageConstants.Show] = string.Empty;
            return new ViewArguments(arguments);
        }

        public static ViewArguments ShowDialog(string viewId)
        {
            var arguments = new Dictionary<string, object>();
            arguments[GenericMessageConstants.ViewId] = viewId;
            return new ViewArguments(arguments);
        }

        public static ViewArguments SetModel(object model)
        {
            var arguments = new Dictionary<string, object>();
            arguments[GenericMessageConstants.SetModel] = model;
            return new ViewArguments(arguments);
        }

        public static ViewArguments SetParent(object model)
        {
            var arguments = new Dictionary<string, object>();
            arguments[GenericMessageConstants.SetParent] = model;
            return new ViewArguments(arguments);
        }

        public static ViewArguments Update()
        {
            var arguments = new Dictionary<string, object>();
            arguments[GenericMessageConstants.Update] = default;
            return new ViewArguments(arguments);
        }

        public static ViewArguments Refresh()
        {
            var arguments = new Dictionary<string, object>();
            arguments[GenericMessageConstants.Refresh] = default;
            return new ViewArguments(arguments);
        }

    }
}