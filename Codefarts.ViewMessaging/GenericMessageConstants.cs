// <copyright file="GenericMessageConstants.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    /// <summary>
    /// Provides ready built generic but common message id strings.
    /// </summary>
    public static class GenericMessageConstants
    {
        /// <summary>
        /// Used for showing something.
        /// </summary>
        public const string Show = "Show_220D2A33-5674-4165-B1B1-D7924412F13B";

        /// <summary>
        /// Used for setting the model of a ui element in a MVVM scenario.
        /// </summary>
        public const string SetModel = "SetModel_B74E6645-9022-485C-A044-DF95818A2A8A";

        /// <summary>
        /// Used for showing dialog windows.
        /// </summary>
        public const string ShowDialog = "ShowDialog_0F14A97B-44DC-4708-995B-1CE4F1048732";

        /// <summary>
        /// Used for setting the parent control property.
        /// </summary>
        public const string SetParent = "SetParent_84C97073-3B10-4765-ACDE-55CDB9DEAA4D";

        /// <summary>
        /// Used to update a item.
        /// </summary>
        public const string Update = "Update_CB1AACFC-9588-4227-B9C0-517DD6FA3A2F";

        /// <summary>
        /// Used to refresh a item.
        /// </summary>
        /// <remarks>Sometimes this is needed to refresh data bindings for forcing a refresh.</remarks>
        public const string Refresh = "Refresh_3CA7E92F-4014-4096-AF23-59B889338CBC";

        /// <summary>
        /// Used as a common argument name. For example when sending a ShowDialog message.
        /// </summary>
        public const string ViewId = "ViewID_632CB0C4-6A75-44A0-853F-63B95BF3D4EA";
    }
}